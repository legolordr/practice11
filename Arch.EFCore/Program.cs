// See https://aka.ms/new-console-template for more information

using Arch.EFCore;

Console.WriteLine("Hello, World!");
await using var db = new DataContext();
await db.Database.EnsureCreatedAsync();