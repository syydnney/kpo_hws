namespace POS.Domain.Models;

public sealed class Student
{
    public Guid Id { get; }
    public string FullName { get; }

    public Student(Guid id, string fullName)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID студента не может быть пустым.", nameof(id));
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Укажите имя студента.", nameof(fullName));
        Id = id;
        FullName = fullName.Trim();
    }
}
