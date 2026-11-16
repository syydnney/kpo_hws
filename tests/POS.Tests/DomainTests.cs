using POS.Domain.Factories;
using POS.Domain.Models;
using POS.Domain.Scoring;
using Xunit;
namespace POS.Tests;
public sealed class DomainTests
{
    [Fact]
    public void StudentRejectsBlankName() => Assert.Throws<ArgumentException>(() => new Student(Guid.NewGuid(), " "));
    [Fact]
    public void CriterionRejectsNonPositiveMax() => Assert.Throws<ArgumentOutOfRangeException>(() => new Criterion(Guid.NewGuid(), "Exam", 0, 1));
    [Fact]
    public void CriterionRejectsNonPositiveWeight() => Assert.Throws<ArgumentOutOfRangeException>(() => new Criterion(Guid.NewGuid(), "Exam", 10, 0));
    [Fact]
    public void RubricRejectsDuplicateNameIgnoringCase()
    {
        var r = new Rubric(); r.AddCriterion("Exam", 10, 1);
        Assert.Throws<InvalidOperationException>(() => r.AddCriterion(" exam ", 10, 1));
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void FactoryRejectsScoreOutsideCriterion(decimal value)
    {
        var r = new Rubric(); var c = r.AddCriterion("Exam", 10, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => new EvaluationFactory().Create(Guid.NewGuid(), r, c.Id, value));
    }
    [Fact]
    public void FactoryRejectsUnknownCriterion()
    {
        Assert.Throws<InvalidOperationException>(() => new EvaluationFactory().Create(Guid.NewGuid(), new Rubric(), Guid.NewGuid(), 5));
    }
    [Fact]
    public void GradebookRejectsCriterionOutsideCurrentRubric()
    {
        var first = new Rubric(); var a = first.AddCriterion("Course A", 10, 1);
        var second = new Rubric(); second.AddCriterion("Course B", 10, 1);
        var id = Guid.NewGuid();
        var grade = new EvaluationFactory().Create(id, first, a.Id, 5);
        Assert.Throws<InvalidOperationException>(() => new Gradebook(id).Record(grade, second));
    }
    [Fact]
    public void ScoreBoundariesZeroAndMaximumAreAllowed()
    {
        var rubric = new Rubric(); var c = rubric.AddCriterion("Exam", 10, 1);
        var id = Guid.NewGuid(); var factory = new EvaluationFactory();
        Assert.Equal(0m, factory.Create(id, rubric, c.Id, 0).Score);
        Assert.Equal(10m, factory.Create(id, rubric, c.Id, 10).Score);
    }
    [Fact]
    public void GradebookRejectsOtherStudentsGrade()
    {
        var r = new Rubric(); var c = r.AddCriterion("Exam", 10, 1);
        var e = new EvaluationFactory().Create(Guid.NewGuid(), r, c.Id, 5);
        var other = new Gradebook(Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => other.Record(e, r));
    }
    [Fact]
    public void ReplacingAndCancelingGradeWorks()
    {
        var id = Guid.NewGuid(); var r = new Rubric(); var c = r.AddCriterion("Exam", 10, 1);
        var f = new EvaluationFactory(); var book = new Gradebook(id);
        book.Record(f.Create(id, r, c.Id, 4), r); book.Record(f.Create(id, r, c.Id, 8), r);
        Assert.Single(book.Evaluations); Assert.Equal(8m, Assert.Single(book.Evaluations).Score);
        Assert.True(book.Cancel(c.Id)); Assert.Empty(book.Evaluations);
        Assert.False(book.Cancel(c.Id));
    }
    [Fact]
    public void WeightedPoliciesDifferWhenGradeMissing()
    {
        var id = Guid.NewGuid(); var r = new Rubric();
        var a = r.AddCriterion("HW", 10, 4); r.AddCriterion("Exam", 20, 6);
        var book = new Gradebook(id);
        book.Record(new EvaluationFactory().Create(id, r, a.Id, 8), r);
        var calc = new FinalScoreCalculator();
        Assert.Equal(32m, calc.Calculate(r, book, new MissingAsZeroStrategy()).Percentage);
        Assert.Equal(80m, calc.Calculate(r, book, new GradedOnlyStrategy()).Percentage);
    }
    [Fact]
    public void WeightedResultUsesCriterionMaxAndWeight()
    {
        var id = Guid.NewGuid(); var r = new Rubric();
        var a = r.AddCriterion("HW", 10, 0.4m); var b = r.AddCriterion("Exam", 20, 0.6m);
        var book = new Gradebook(id); var f = new EvaluationFactory();
        book.Record(f.Create(id, r, a.Id, 8), r); book.Record(f.Create(id, r, b.Id, 18), r);
        Assert.Equal(86m, new FinalScoreCalculator().Calculate(r, book, new MissingAsZeroStrategy()).Percentage);
    }
    [Fact]
    public void NoRubricCannotCalculate()
    {
        Assert.Throws<InvalidOperationException>(() => new FinalScoreCalculator().Calculate(new Rubric(), new Gradebook(Guid.NewGuid()), new MissingAsZeroStrategy()));
    }
    [Fact]
    public void GradedOnlyWithoutGradesHasNoDenominator()
    {
        var r = new Rubric(); r.AddCriterion("Exam", 10, 1);
        var result = new FinalScoreCalculator().Calculate(r, new Gradebook(Guid.NewGuid()), new GradedOnlyStrategy());
        Assert.Equal(0m, result.Percentage);
        Assert.Equal(0, result.EvaluatedCriteria);
    }
}
