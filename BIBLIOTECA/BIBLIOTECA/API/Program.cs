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

app.MapGet("/api/livros/{id}", (BibliotecaDbContext dbContext, int id) =>
{
    var livro = dbContext.Livros.Include(l => l.Categoria).FirstOrDefault(l => l.Id == id);
    if (livro == null)
    {
        return Results.NotFound($"Livro com ID {id} não encontrado.");
    }
    return Results.Ok(livro);
});

app.MapPut("/api/livros/{id}", (BibliotecaDbContext dbContext, int id, Livro livroAtualizado) =>
{
    var livroExistente = dbContext.Livros.Find(id);
    if (livroExistente == null)
    {
        return Results.NotFound($"Livro com ID {id} não encontrado para atualização.");
    }

    if (livroAtualizado.Titulo?.Length < 3)
    {
        return Results.BadRequest("Título deve ter no mínimo 3 caracteres.");
    }
    if (livroAtualizado.Autor?.Length < 3)
    {
        return Results.BadRequest("Autor deve ter no mínimo 3 caracteres.");
    }

    var categoria = dbContext.Categorias.Find(livroAtualizado.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria inválida. O ID da categoria fornecido não existe.");
    }

    livroExistente.Titulo = livroAtualizado.Titulo;
    livroExistente.Autor = livroAtualizado.Autor;
    livroExistente.CategoriaId = livroAtualizado.CategoriaId;
    livroExistente.Categoria = categoria;

    dbContext.SaveChanges();
    
    var livroComCategoria = dbContext.Livros.Include(l => l.Categoria).FirstOrDefault(l => l.Id == id);
    return Results.Ok(livroComCategoria);
});


app.MapDelete("/api/livros/{id}", (BibliotecaDbContext dbContext, int id) =>
{
    var livro = dbContext.Livros.Find(id);
    if (livro == null)
    {
        return Results.NotFound($"Livro com ID {id} não encontrado para remoção.");
    }

    dbContext.Livros.Remove(livro);
    dbContext.SaveChanges();
    return Results.NoContent();
});

app.Run();