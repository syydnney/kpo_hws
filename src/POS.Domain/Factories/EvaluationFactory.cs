using POS.Domain.Models;

namespace POS.Domain.Factories;

public sealed class EvaluationFactory
{
    public Evaluation Create(Guid studentId, Rubric rubric, Guid criterionId, decimal score)
    {
        if (studentId == Guid.Empty) throw new ArgumentException("Некорректный ID студента.", nameof(studentId));
        ArgumentNullException.ThrowIfNull(rubric);
        var criterion = rubric.GetCriterion(criterionId);
        if (score < 0 || score > criterion.MaxScore)
            throw new ArgumentOutOfRangeException(nameof(score), $"Балл должен быть от 0 до {criterion.MaxScore}.");
        return new Evaluation(studentId, criterion.Id, score);
    }
}
