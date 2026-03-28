using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class CrudNote
{
    public static async Task<Note> Create(string text, DateTimeOffset created)
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync();
        var note = new Note
        {
            Text = text,
            Created = created
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return note;
    }
    
    public static async Task<List<Note>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync();
        var result = await db.Notes.Where(x => EF.Functions.
            Like(x.Text, $"%{search}%")).ToListAsync(ct);
        return result;
    }

    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Notes.FirstOrDefaultAsync(x => x.Id == id, ct);
        return result;
    }

    public static async Task Update(Note note, string text, DateTimeOffset created, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Text = text;
        note.Created = created;
        db.Notes.Update(note);
        await db.SaveChangesAsync(ct);
    }

    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Notes.Remove(note);
        await db.SaveChangesAsync(ct);
    }
}