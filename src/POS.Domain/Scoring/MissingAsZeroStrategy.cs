using POS.Domain.Models;

namespace POS.Domain.Scoring;

public sealed class MissingAsZeroStrategy : IScoringStrategy
{
    public string Name => "Отсутствующие оценки = 0";

    public decimal Denominator(IReadOnlyCollection<Criterion> criteria, IReadOnlyCollection<Evaluation> evaluations)
        => criteria.Sum(x => x.Weight);
}
