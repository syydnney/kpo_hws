using POS.Domain.Models;
namespace POS.Application.Ports;
public interface IRubricRepository
{
    Rubric Get();
    void Save(Rubric rubric);
}
