using POS.Domain.Models;
using POS.Domain.Scoring;
namespace POS.Application.Reporting;
public sealed record GradeRow(string CriterionName, decimal MaxScore, decimal Weight, decimal? Score);
public sealed record StudentReport(Student Student, ScoreResult Result, string Policy, IReadOnlyList<GradeRow> Rows);
