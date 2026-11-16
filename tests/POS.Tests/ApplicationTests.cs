using POS.Application.Services;
using POS.Domain.Factories;
using POS.Domain.Scoring;
using POS.Infrastructure.Export;
using POS.Infrastructure.Repositories;
using Xunit;
namespace POS.Tests;
public sealed class ApplicationTests
{
    private static PosFacade MakeFacade()
    {
        var students = new InMemoryStudentRepository();
        var rubrics = new InMemoryRubricRepository();
        var gradebooks = new InMemoryGradebookRepository();
        return new PosFacade(
            new CatalogService(students, rubrics),
            new GradingService(students, rubrics, gradebooks, new EvaluationFactory()),
            new ReportingService(students, rubrics, gradebooks, new FinalScoreCalculator(), new CsvResultExporter()));
    }
    [Fact]
    public void FullScenarioAndExport()
    {
        var app = MakeFacade(); var student = app.AddStudent("Ivan Ivanov");
        var c = app.AddCriterion("Exam", 10, 1);
        app.SetScore(student.Id, c.Id, 7);
        Assert.Equal(70m, app.Report(student.Id, new MissingAsZeroStrategy()).Result.Percentage);
        app.SetScore(student.Id, c.Id, 9);
        Assert.Equal(90m, app.Report(student.Id, new MissingAsZeroStrategy()).Result.Percentage);
        var path = Path.Combine(Path.GetTempPath(), $"pos-test-{Guid.NewGuid()}.csv");
        try
        {
            Assert.Equal(Path.GetFullPath(path), app.Export(student.Id, new MissingAsZeroStrategy(), path));
            var csv = File.ReadAllText(path);
            Assert.Contains("Ivan Ivanov", csv);
            Assert.Contains("90", csv);
        }
        finally { if (File.Exists(path)) File.Delete(path); }
        Assert.True(app.CancelScore(student.Id, c.Id));
        Assert.Empty(app.Scores(student.Id));
        Assert.Equal(0m, app.Report(student.Id, new MissingAsZeroStrategy()).Result.Percentage);
    }
    [Fact]
    public void CannotGradeUnknownStudent()
    {
        var app = MakeFacade(); var c = app.AddCriterion("Exam", 10, 1);
        Assert.Throws<InvalidOperationException>(() => app.SetScore(Guid.NewGuid(), c.Id, 5));
    }
    [Fact]
    public void CannotGradeCriterionOutsideRubric()
    {
        var app = MakeFacade(); var s = app.AddStudent("Ivan");
        Assert.Throws<InvalidOperationException>(() => app.SetScore(s.Id, Guid.NewGuid(), 5));
    }
    [Fact]
    public void CannotCancelUnknownCriterion()
    {
        var app = MakeFacade(); var s = app.AddStudent("Ivan");
        Assert.Throws<InvalidOperationException>(() => app.CancelScore(s.Id, Guid.NewGuid()));
    }
}
