using POS.Application.Ports;
using POS.Domain.Models;
namespace POS.Infrastructure.Repositories;
public sealed class InMemoryRubricRepository : IRubricRepository
{
    private Rubric _rubric = new();
    public Rubric Get() => _rubric;
    public void Save(Rubric rubric) => _rubric = rubric ?? throw new ArgumentNullException(nameof(rubric));
}
