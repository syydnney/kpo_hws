using System.Globalization;
using System.Text;
using POS.Application.Ports;
using POS.Application.Reporting;
namespace POS.Infrastructure.Export;

public sealed class CsvResultExporter : IResultExporter
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    public string Export(StudentReport report, string filePath)
    {
        ArgumentNullException.ThrowIfNull(report);
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Укажите путь CSV.", nameof(filePath));
        var fullPath = Path.GetFullPath(filePath);
        using var writer = new StreamWriter(fullPath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        writer.WriteLine("StudentId;Student;Criterion;Score;MaxScore;Weight;Policy;FinalPercent");
        foreach (var row in report.Rows)
        {
            var cells = new[] {
                report.Student.Id.ToString(), report.Student.FullName, row.CriterionName,
                row.Score?.ToString(Invariant) ?? "", row.MaxScore.ToString(Invariant),
                row.Weight.ToString(Invariant), report.Policy, report.Result.Percentage.ToString(Invariant)
            };
            writer.WriteLine(string.Join(';', cells.Select(Escape)));
        }
        return fullPath;
    }
    private static string Escape(string text)
    {
        // Не даём пользовательскому названию превратиться в формулу при открытии CSV в таблицах.
        if (text.TrimStart().StartsWith('=') || text.TrimStart().StartsWith('+') ||
            text.TrimStart().StartsWith('-') || text.TrimStart().StartsWith('@')) text = "'" + text;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }
}
