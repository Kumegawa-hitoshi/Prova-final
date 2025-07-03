using API.Modelos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotecaDbContext>();

var app = builder.Build();

app.MapGet("/api/livros", (BibliotecaDbContext dbContext) =>
{
    var livros = dbContext.Livros.Include(l => l.Categoria).ToList();
    return Results.Ok(livros);
});

app.Run();