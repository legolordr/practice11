using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

/// <summary>
/// Содержит примеры CRUD-операций для сущности <see cref="Student"/>.
/// </summary>
/// <remarks>
/// CRUD (Create, Read, Update, Delete) это термин по умолчанию для таких запросов
/// (из которых состоит большая часть работы большинства программ).
/// </remarks>
public class Crud
{
    /// <summary>
    /// Создаёт нового ученика и сохраняет его в БД.
    /// </summary>
    /// <returns>
    /// Сущность нового ученика. После сохранения в БД его свойство <see cref="Student.Id"/>
    /// будет содержать реальный ID из СУБД (не 0).
    /// </returns>
    public static async Task<Student> Create(string name, int age, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var student = new Student
        {
            Name = name,
            Age = age
        };
        // В этот момент Id равен 0, так как сущность не сохранена в БД
        db.Students.Add(student);
        await db.SaveChangesAsync(ct);
        // Здесь Id выставлен в корректное значение
        return student;
    }

    /// <summary>
    /// Получает список студентов с поиском по частичному совпадению имени.
    /// </summary>
    public static async Task<List<Student>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Students
            .Where(x => EF.Functions.Like(x.Name, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }

    /// <summary>
    /// Ищет конкретного ученика по его ID.
    /// </summary>
    /// <returns>
    /// Сущность найденного ученика или <see langword="null"/> если такого нет.
    /// </returns>
    public static async Task<Student?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Students.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    /// <summary>
    /// Обновляет сущность ученика в БД. Меняет данные в той же сущности, не создаёт новый инстанс.
    /// </summary>
    public static async Task Update(Student student, string name, int age, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        student.Name = name;
        student.Age = age;
        db.Students.Update(student);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Удаляет сущность студента из БД.
    /// </summary>
    public static async Task Delete(Student student, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Students.Remove(student);
        await db.SaveChangesAsync(ct);
    }
}