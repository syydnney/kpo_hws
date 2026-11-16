using System.Globalization;
using POS.Application.Services;
using POS.Domain.Models;
using POS.Domain.Scoring;

namespace POS.ConsoleAppHost;

public sealed class ConsoleApp
{
    private readonly PosFacade _pos;
    public ConsoleApp(PosFacade pos) => _pos = pos;

    public void Run()
    {
        while (true)
        {
            System.Console.WriteLine("\n=== ПОС: Преподавательская оценка студентов ===");
            System.Console.WriteLine("1. Добавить студента  2. Список студентов");
            System.Console.WriteLine("3. Добавить критерий 4. Список критериев");
            System.Console.WriteLine("5. Выставить/исправить оценку 6. Отменить оценку");
            System.Console.WriteLine("7. Оценки студента 8. Итог студента 9. Экспорт CSV 0. Выход");
            System.Console.Write("Команда: ");
            var command = System.Console.ReadLine();
            if (command is null || command == "0") return;
            try
            {
                switch (command.Trim())
                {
                    case "1": AddStudent(); break;
                    case "2": PrintStudents(); break;
                    case "3": AddCriterion(); break;
                    case "4": PrintCriteria(); break;
                    case "5": SetScore(); break;
                    case "6": CancelScore(); break;
                    case "7": ShowScores(); break;
                    case "8": ShowResult(); break;
                    case "9": Export(); break;
                    default: System.Console.WriteLine("Неизвестная команда."); break;
                }
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException or OverflowException)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private void AddStudent()
    {
        var s = _pos.AddStudent(ReadText("ФИО: "));
        System.Console.WriteLine($"Добавлен: {s.FullName} (ID: {s.Id})");
    }
    private void PrintStudents()
    {
        var students = _pos.Students();
        if (students.Count == 0) System.Console.WriteLine("Пока нет студентов.");
        foreach (var s in students) System.Console.WriteLine($"{s.Id} | {s.FullName}");
    }
    private void AddCriterion()
    {
        var name = ReadText("Критерий: ");
        var max = ReadDecimal("Максимальный балл (>0): ");
        var weight = ReadDecimal("Вес (>0; например 0.4): ");
        var c = _pos.AddCriterion(name, max, weight);
        System.Console.WriteLine($"Добавлен: {c.Name} (ID: {c.Id})");
    }
    private void PrintCriteria()
    {
        var criteria = _pos.Criteria();
        if (criteria.Count == 0) System.Console.WriteLine("Рубрикатор пока пуст.");
        foreach (var c in criteria)
            System.Console.WriteLine($"{c.Id} | {c.Name} | максимум {c.MaxScore} | вес {c.Weight}");
    }
    private void SetScore()
    {
        var student = ChooseStudent();
        var criterion = ChooseCriterion();
        var score = ReadDecimal($"Оценка 0..{criterion.MaxScore}: ");
        _pos.SetScore(student.Id, criterion.Id, score);
        System.Console.WriteLine("Оценка выставлена (или исправлена). ");
    }
    private void CancelScore()
    {
        var student = ChooseStudent();
        var criterion = ChooseCriterion();
        System.Console.WriteLine(_pos.CancelScore(student.Id, criterion.Id) ? "Оценка отменена." : "Такой оценки нет.");
    }
    private void ShowScores()
    {
        var student = ChooseStudent();
        var criteria = _pos.Criteria().ToDictionary(x => x.Id);
        var grades = _pos.Scores(student.Id);
        if (grades.Count == 0) System.Console.WriteLine("Оценок пока нет.");
        foreach (var e in grades)
            System.Console.WriteLine($"{criteria[e.CriterionId].Name}: {e.Score}/{criteria[e.CriterionId].MaxScore}");
    }
    private void ShowResult()
    {
        var student = ChooseStudent();
        var report = _pos.Report(student.Id, ChooseStrategy());
        PrintReport(report);
    }
    private void Export()
    {
        var student = ChooseStudent();
        var policy = ChooseStrategy();
        var path = ReadText("CSV-файл (например result.csv): ");
        var saved = _pos.Export(student.Id, policy, path);
        System.Console.WriteLine($"Экспортирован отчёт: {saved}");
    }
    private void PrintReport(POS.Application.Reporting.StudentReport report)
    {
        System.Console.WriteLine($"Итог: {report.Student.FullName} — {report.Result.Percentage:F2}% ({report.Policy})");
        foreach (var r in report.Rows)
            System.Console.WriteLine($"{r.CriterionName}: {(r.Score?.ToString() ?? "нет оценки")}/{r.MaxScore}; вес {r.Weight}");
    }
    private Student ChooseStudent()
    {
        var students = _pos.Students().ToArray();
        if (students.Length == 0) throw new InvalidOperationException("Сначала добавьте студента.");
        for (var i = 0; i < students.Length; i++) System.Console.WriteLine($"{i+1}. {students[i].FullName}");
        return students[ReadIndex(students.Length)];
    }
    private Criterion ChooseCriterion()
    {
        var criteria = _pos.Criteria().ToArray();
        if (criteria.Length == 0) throw new InvalidOperationException("Сначала добавьте критерий.");
        for (var i = 0; i < criteria.Length; i++) System.Console.WriteLine($"{i+1}. {criteria[i].Name} (макс. {criteria[i].MaxScore})");
        return criteria[ReadIndex(criteria.Length)];
    }
    private static IScoringStrategy ChooseStrategy()
    {
        System.Console.WriteLine("1. Отсутствующие оценки считаются нулём (по всем критериям)");
        System.Console.WriteLine("2. Средний результат только по выставленным оценкам");
        while (true)
        {
            System.Console.Write("Правило расчёта [1/2]: ");
            switch (System.Console.ReadLine())
            {
                case "1": return new MissingAsZeroStrategy();
                case "2": return new GradedOnlyStrategy();
                case null: throw new InvalidOperationException("Ввод завершён.");
                default: System.Console.WriteLine("Введите 1 или 2."); break;
            }
        }
    }
    private static int ReadIndex(int length)
    {
        while (true)
        {
            System.Console.Write("Номер из списка: ");
            var value = System.Console.ReadLine();
            if (value is null) throw new InvalidOperationException("Ввод завершён.");
            if (int.TryParse(value, out var n) && n >= 1 && n <= length) return n - 1;
            System.Console.WriteLine($"Введите число от 1 до {length}.");
        }
    }
    private static string ReadText(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var s = System.Console.ReadLine();
            if (s is null) throw new InvalidOperationException("Ввод завершён.");
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            System.Console.WriteLine("Значение не может быть пустым.");
        }
    }
    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            System.Console.Write(prompt);
            var s = System.Console.ReadLine();
            if (s is null) throw new InvalidOperationException("Ввод завершён.");
            if (decimal.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var n)) return n;
            System.Console.WriteLine("Введите число, например 7.5.");
        }
    }
}
