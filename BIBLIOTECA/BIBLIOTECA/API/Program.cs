using API.Modelos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotecaDbContext>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();