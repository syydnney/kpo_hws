using POS.Application.Ports;
using POS.Application.Reporting;
using POS.Domain.Models;
using POS.Domain.Scoring;
namespace POS.Application.Services;
public sealed class ReportingService
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;
    private readonly IGradebookRepository _gradebooks;
    private readonly FinalScoreCalculator _calculator;
    private readonly IResultExporter _exporter;
    public ReportingService(IStudentRepository students, IRubricRepository rubrics, IGradebookRepository gradebooks, FinalScoreCalculator calculator, IResultExporter exporter)
    {
        _students = students;
        _rubrics = rubrics;
        _gradebooks = gradebooks;
        _calculator = calculator;
        _exporter = exporter;
    }
    public StudentReport Report(Guid studentId, IScoringStrategy policy)
    {
        var student = _students.Find(studentId) ?? throw new InvalidOperationException("Студент не найден.");
        var rubric = _rubrics.Get();
        var gradebook = _gradebooks.Find(studentId) ?? new Gradebook(studentId);
        var result = _calculator.Calculate(rubric, gradebook, policy);
        var grades = gradebook.Evaluations.ToDictionary(x => x.CriterionId);
        var rows = rubric.Criteria.Select(c =>
            new GradeRow(c.Name, c.MaxScore, c.Weight, grades.TryGetValue(c.Id, out var e) ? e.Score : null)).ToArray();
        return new StudentReport(student, result, policy.Name, rows);
    }
    public string Export(Guid studentId, IScoringStrategy policy, string path) =>
        _exporter.Export(Report(studentId, policy), path);
}
