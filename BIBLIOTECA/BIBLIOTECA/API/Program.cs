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

app.MapPost("/api/livros", (BibliotecaDbContext dbContext, Livro livro) =>
{
    if (livro.Titulo?.Length < 3)
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    if (livro.Autor?.Length < 3)
    {
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
    }
    
    var categoria = dbContext.Categorias.Find(livro.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livro.Categoria = categoria;
    dbContext.Livros.Add(livro);
    dbContext.SaveChanges();
    
    return Results.Created($"/api/livros/{livro.Id}", livro);
});


app.Run();