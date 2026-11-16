using POS.Domain.Models;
namespace POS.Application.Ports;
public interface IStudentRepository
{
    void Add(Student student);
    Student? Find(Guid id);
    IReadOnlyCollection<Student> List();
}
