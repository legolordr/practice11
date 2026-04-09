using Arch.EFCore;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CrudNote.Tests;

public class CrudNoteTest : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    public CrudNoteTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public async Task InitializeAsync()
    {
        await using var db = new DataContext();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var db = new DataContext();
        await  db.Database.EnsureDeletedAsync();
    }

    [Theory]
    [InlineData("Text note","2026-03-27 14:30:00")]
    [InlineData("41213212321","2026-03-27 09:31:32")]
    [InlineData("*_D+#($KD$94","2026-12-12 09:31:32")]
    public async Task Create_TextTimeCreated_Success(string text, DateTimeOffset created)
    {
        //Act
        var user = await CrudUser.Create("Invoker");
        var note = await Arch.EFCore.CrudNote.Create(text, created,user.Id);

        //Assert
        Assert.NotNull(note);
        Assert.Equal(text, note.Text);
        Assert.Equal(created, note.Created);
    }
    /// <summary>
    /// Поиск по <see cref="Note.Text"/>
    /// </summary>
    [Fact]
    public async Task Read_AnySearch_Success()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);

        //Act
        var resultHello = await Arch.EFCore.CrudNote.Read("hello");
        var resultTest = await Arch.EFCore.CrudNote.Read("test");

        //Assert
        Assert.Contains(resultHello, r => r.Text.Contains("hello world"));
        Assert.Contains(resultTest, r => r.Text.Contains("test note"));
    }

    [Fact]
    public async Task Read_Ct_OperationCanceledException()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        //Assert
        Assert.True(cts.Token.IsCancellationRequested);
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await Arch.EFCore.CrudNote.Read("test", cts.Token));
    }

    /// <summary>
    /// Поиск по <see cref="Note.Text"/>
    /// </summary>
    [Fact]
    public async Task Read_AnySearch_ListIsEmpty()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);

        //Act
        var resultHello = await Arch.EFCore.CrudNote.Read("zxc");
        var resultTest = await Arch.EFCore.CrudNote.Read("asd");

        //Assert
        Assert.Empty(resultHello);
        Assert.Empty(resultTest);
    }


    /// <summary>
    /// Поиск по <see cref="Note.Id"/>
    /// </summary>
    [Fact]
    public async Task Read_AnyId_SpecificNote()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        var hw  = await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        var tn = await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);

        //Act
        var resultHw = await Arch.EFCore.CrudNote.Read(hw.Id);
        var resultTn = await Arch.EFCore.CrudNote.Read(tn.Id);

        //Assert
        Assert.Equal(hw.Id, resultHw!.Id);
        Assert.Equal(tn.Id, resultTn!.Id);
    }

    /// <summary>
    /// Поиск по <see cref="Note.Id"/>
    /// </summary>
    [Fact]
    public async Task Read_AnyId_NullNote()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);

        //Act
        var resultHw = await Arch.EFCore.CrudNote.Read(321412);
        var resultTn = await Arch.EFCore.CrudNote.Read(342123131);

        //Assert
        Assert.Null(resultHw);
        Assert.Null(resultTn);
    }

    [Fact]
    public async Task Update_NoteWithParams_Success()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        var hw= await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        var tn = await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);

        //Act
        await Arch.EFCore.CrudNote.Update(hw, "new text",DateTimeOffset.Parse("2026-02-27 14:30:00"));
        await Arch.EFCore.CrudNote.Update(tn, "snickers",DateTimeOffset.Parse("2026-02-27 14:30:00"));

        //Assert
        Assert.Contains("new text", hw.Text);
        Assert.Contains("snickers", tn.Text);
        Assert.Contains("2026-02-27 14:30:00", hw.Created.ToString("yyyy-MM-dd HH:mm:ss"));
        Assert.Contains("2026-02-27 14:30:00", tn.Created.ToString("yyyy-MM-dd HH:mm:ss"));
    }
    [Fact]
    public async Task Delete_Note_Success()
    {
        //Arrange
        var user = await CrudUser.Create("Invoker");
        var hw = await Arch.EFCore.CrudNote.Create("hello world", DateTimeOffset.UtcNow,user.Id);
        var tn = await Arch.EFCore.CrudNote.Create("test note", DateTimeOffset.UtcNow,user.Id);
        //Act
        await  Arch.EFCore.CrudNote.Delete(hw);
        await Arch.EFCore.CrudNote.Delete(tn);

        var resultHw = await Arch.EFCore.CrudNote.Read(hw.Id);
        var resultTn = await Arch.EFCore.CrudNote.Read(tn.Id);

        //Assert
        Assert.Null(resultHw);
        Assert.Null(resultTn);
    }

    [Fact]
    public async Task Delete_NoteNotFromDB_InvalidOperationException()
    {
        //Arrange
        Note alienNote =  new Note();

        // ITestOutputHelper
        var exception = await Record.ExceptionAsync(async () => await Arch.EFCore.CrudNote.Delete(alienNote));
        _output.WriteLine($"Тип исключения: {exception.GetType().FullName}");

        //Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await Arch.EFCore.CrudNote.Delete(alienNote));


    }
}

