using POS.Application.Ports;
using POS.Domain.Factories;
using POS.Domain.Models;
namespace POS.Application.Services;
public sealed class GradingService
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;
    private readonly IGradebookRepository _gradebooks;
    private readonly EvaluationFactory _factory;
    public GradingService(IStudentRepository students, IRubricRepository rubrics, IGradebookRepository gradebooks, EvaluationFactory factory)
    {
        _students = students;
        _rubrics = rubrics;
        _gradebooks = gradebooks;
        _factory = factory;
    }
    public Evaluation SetScore(Guid studentId, Guid criterionId, decimal score)
    {
        if (_students.Find(studentId) is null) throw new InvalidOperationException("Студент не найден.");
        var evaluation = _factory.Create(studentId, _rubrics.Get(), criterionId, score);
        var gradebook = _gradebooks.Find(studentId) ?? new Gradebook(studentId);
        gradebook.Record(evaluation, _rubrics.Get());
        _gradebooks.Save(gradebook);
        return evaluation;
    }
    public bool CancelScore(Guid studentId, Guid criterionId)
    {
        if (_students.Find(studentId) is null) throw new InvalidOperationException("Студент не найден.");
        _rubrics.Get().GetCriterion(criterionId); // критерий обязан существовать
        var gradebook = _gradebooks.Find(studentId);
        if (gradebook is null) return false;
        var removed = gradebook.Cancel(criterionId);
        if (removed) _gradebooks.Save(gradebook);
        return removed;
    }
    public IReadOnlyCollection<Evaluation> Scores(Guid studentId)
    {
        if (_students.Find(studentId) is null) throw new InvalidOperationException("Студент не найден.");
        return _gradebooks.Find(studentId)?.Evaluations ?? Array.Empty<Evaluation>();
    }
}
