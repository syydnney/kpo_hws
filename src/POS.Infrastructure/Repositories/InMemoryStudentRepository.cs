using POS.Application.Ports;
using POS.Domain.Models;
namespace POS.Infrastructure.Repositories;
public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<Guid, Student> _items = new();
    public void Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        _items.Add(student.Id, student);
    }
    public Student? Find(Guid id) => _items.GetValueOrDefault(id);
    public IReadOnlyCollection<Student> List() => _items.Values.ToArray();
}
