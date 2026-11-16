namespace POS.Domain.Models;

public sealed class Rubric
{
    private readonly Dictionary<Guid, Criterion> _criteria = new();
    public IReadOnlyCollection<Criterion> Criteria => _criteria.Values.ToArray();

    public Criterion AddCriterion(string name, decimal maxScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Укажите название критерия.", nameof(name));
        if (_criteria.Values.Any(x => string.Equals(x.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Критерий с таким названием уже существует.");
        var criterion = new Criterion(Guid.NewGuid(), name, maxScore, weight);
        _criteria.Add(criterion.Id, criterion);
        return criterion;
    }

    public Criterion GetCriterion(Guid criterionId) =>
        _criteria.TryGetValue(criterionId, out var criterion)
            ? criterion
            : throw new InvalidOperationException("Такого критерия нет в рубрикаторе.");
}
