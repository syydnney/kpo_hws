using POS.Domain.Models;
namespace POS.Application.Ports;
public interface IGradebookRepository
{
    Gradebook? Find(Guid studentId);
    void Save(Gradebook gradebook);
}
