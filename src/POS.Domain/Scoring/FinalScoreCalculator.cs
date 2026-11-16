using POS.Domain.Models;

namespace POS.Domain.Scoring;

public sealed class FinalScoreCalculator
{
    public ScoreResult Calculate(Rubric rubric, Gradebook gradebook, IScoringStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        ArgumentNullException.ThrowIfNull(gradebook);
        ArgumentNullException.ThrowIfNull(strategy);
        var criteria = rubric.Criteria;
        if (criteria.Count == 0) throw new InvalidOperationException("Нельзя посчитать итог без критериев.");
        var evaluations = gradebook.Evaluations;

        var byCriterion = evaluations.ToDictionary(x => x.CriterionId);
        if (byCriterion.Keys.Any(id => criteria.All(c => c.Id != id)))
            throw new InvalidOperationException("В ведомости есть оценка для несуществующего критерия.");
        var denominator = strategy.Denominator(criteria, evaluations);
        if (denominator == 0)
            return new ScoreResult(0m, 0, criteria.Count);
        decimal weighted = 0m;
        foreach (var criterion in criteria)
        {
            if (!byCriterion.TryGetValue(criterion.Id, out var evaluation)) continue;
            if (evaluation.Score < 0 || evaluation.Score > criterion.MaxScore)
                throw new InvalidOperationException("Обнаружена недопустимая оценка.");
            weighted += evaluation.Score / criterion.MaxScore * criterion.Weight;
        }
        var percentage = decimal.Round(weighted / denominator * 100m, 2, MidpointRounding.AwayFromZero);
        return new ScoreResult(percentage, byCriterion.Count, criteria.Count);
    }
}
