namespace POS.Domain.Scoring;

public sealed record ScoreResult(decimal Percentage, int EvaluatedCriteria, int TotalCriteria);
