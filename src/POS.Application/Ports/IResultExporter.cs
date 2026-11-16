using POS.Application.Reporting;
namespace POS.Application.Ports;
public interface IResultExporter
{
    string Export(StudentReport report, string filePath);
}
