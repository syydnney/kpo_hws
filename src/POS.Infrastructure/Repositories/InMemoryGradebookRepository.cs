using POS.Application.Ports;
using POS.Domain.Models;
namespace POS.Infrastructure.Repositories;
public sealed class InMemoryGradebookRepository : IGradebookRepository
{
    private readonly Dictionary<Guid, Gradebook> _books = new();
    public Gradebook? Find(Guid studentId) => _books.GetValueOrDefault(studentId);
    public void Save(Gradebook gradebook)
    {
        ArgumentNullException.ThrowIfNull(gradebook);
        _books[gradebook.StudentId] = gradebook;
    }
}
