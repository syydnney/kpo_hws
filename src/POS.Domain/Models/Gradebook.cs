namespace POS.Domain.Models;

public sealed class Gradebook
{
    private readonly Dictionary<Guid, Evaluation> _evaluations = new();
    public Guid StudentId { get; }
    public IReadOnlyCollection<Evaluation> Evaluations => _evaluations.Values.ToArray();

    public Gradebook(Guid studentId)
    {
        if (studentId == Guid.Empty) throw new ArgumentException("Некорректный ID студента.", nameof(studentId));
        StudentId = studentId;
    }

    public void Record(Evaluation evaluation, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        ArgumentNullException.ThrowIfNull(rubric);
        var criterion = rubric.GetCriterion(evaluation.CriterionId);
        if (evaluation.Score < 0 || evaluation.Score > criterion.MaxScore)
            throw new InvalidOperationException("Оценка выходит за пределы критерия.");
        if (evaluation.StudentId != StudentId)
            throw new InvalidOperationException("Нельзя выставить оценку другого студента.");
        _evaluations[evaluation.CriterionId] = evaluation;
    }

    public bool Cancel(Guid criterionId) => _evaluations.Remove(criterionId);
}
