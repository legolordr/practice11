using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class CrudUser
{
    public static async Task<User> Create(string name, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var user = new User { Name = name };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public static async Task<List<User>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result= await db.Users
            .Where(u => EF.Functions.Like(u.Name, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }

    public static async Task<User?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        return result;
    }

    public static async Task Update(User user, string name, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        user.Name = name;
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    public static async Task Delete(User user, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }

    public static async Task<List<Note>> GetNotesUser(int userId, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var notes = await db.Notes.Where(n => n.UserId == userId).ToListAsync(ct);
        return notes;
    }
}