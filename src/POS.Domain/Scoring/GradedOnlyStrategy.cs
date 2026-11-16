using POS.Domain.Models;

namespace POS.Domain.Scoring;

public sealed class GradedOnlyStrategy : IScoringStrategy
{
    public string Name => "Только оценённые критерии";

    public decimal Denominator(IReadOnlyCollection<Criterion> criteria, IReadOnlyCollection<Evaluation> evaluations)
    {
        var graded = evaluations.Select(x => x.CriterionId).ToHashSet();
        return criteria.Where(x => graded.Contains(x.Id)).Sum(x => x.Weight);
    }
}
