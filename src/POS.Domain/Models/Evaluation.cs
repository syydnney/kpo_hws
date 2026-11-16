namespace POS.Domain.Models;

public sealed record Evaluation
{
    public Guid StudentId { get; }
    public Guid CriterionId { get; }
    public decimal Score { get; }

    internal Evaluation(Guid studentId, Guid criterionId, decimal score)
    {
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
    }
}
