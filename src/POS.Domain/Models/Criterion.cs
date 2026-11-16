namespace POS.Domain.Models;

public sealed class Criterion
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal MaxScore { get; }
    public decimal Weight { get; }

    public Criterion(Guid id, string name, decimal maxScore, decimal weight)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID критерия не может быть пустым.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Укажите название критерия.", nameof(name));
        if (maxScore <= 0) throw new ArgumentOutOfRangeException(nameof(maxScore), "Максимум должен быть положительным.");
        if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть положительным.");
        Id = id;
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }
}
