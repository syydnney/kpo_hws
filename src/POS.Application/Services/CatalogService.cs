using POS.Application.Ports;
using POS.Domain.Models;
namespace POS.Application.Services;
public sealed class CatalogService
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubrics;
    public CatalogService(IStudentRepository students, IRubricRepository rubrics)
    {
        _students = students;
        _rubrics = rubrics;
    }
    public Student AddStudent(string name)
    {
        var student = new Student(Guid.NewGuid(), name);
        _students.Add(student);
        return student;
    }
    public IReadOnlyCollection<Student> Students() => _students.List();
    public Criterion AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var rubric = _rubrics.Get();
        var criterion = rubric.AddCriterion(name, maxScore, weight);
        _rubrics.Save(rubric);
        return criterion;
    }
    public IReadOnlyCollection<Criterion> Criteria() => _rubrics.Get().Criteria;
}
