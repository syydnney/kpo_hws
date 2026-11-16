using POS.Application.Reporting;
using POS.Domain.Models;
using POS.Domain.Scoring;
namespace POS.Application.Services;

// GoF Facade: одна точка доступа UI к каталогу, выставлению оценок и отчётности.
public sealed class PosFacade
{
    private readonly CatalogService _catalog;
    private readonly GradingService _grading;
    private readonly ReportingService _reporting;
    public PosFacade(CatalogService catalog, GradingService grading, ReportingService reporting)
    {
        _catalog = catalog;
        _grading = grading;
        _reporting = reporting;
    }
    public Student AddStudent(string name) => _catalog.AddStudent(name);
    public IReadOnlyCollection<Student> Students() => _catalog.Students();
    public Criterion AddCriterion(string name, decimal maxScore, decimal weight) => _catalog.AddCriterion(name, maxScore, weight);
    public IReadOnlyCollection<Criterion> Criteria() => _catalog.Criteria();
    public Evaluation SetScore(Guid studentId, Guid criterionId, decimal score) => _grading.SetScore(studentId, criterionId, score);
    public bool CancelScore(Guid studentId, Guid criterionId) => _grading.CancelScore(studentId, criterionId);
    public IReadOnlyCollection<Evaluation> Scores(Guid studentId) => _grading.Scores(studentId);
    public StudentReport Report(Guid studentId, IScoringStrategy policy) => _reporting.Report(studentId, policy);
    public string Export(Guid studentId, IScoringStrategy policy, string filePath) => _reporting.Export(studentId, policy, filePath);
}
