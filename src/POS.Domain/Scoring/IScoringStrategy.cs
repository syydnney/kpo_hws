using POS.Domain.Models;

namespace POS.Domain.Scoring;

public interface IScoringStrategy
{
    string Name { get; }
    decimal Denominator(IReadOnlyCollection<Criterion> criteria, IReadOnlyCollection<Evaluation> evaluations);
}
