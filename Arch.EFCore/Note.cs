namespace Arch.EFCore;

public class Note
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Created { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

}