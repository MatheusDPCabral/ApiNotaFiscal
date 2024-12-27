using ApiNotaFiscal.Data;
using ApiNotaFiscal.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<NotaFiscalContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<NotaFiscalService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Endpoint para salvar notas fiscais diretamente do XML
app.MapPost("/processar-xmls", async (NotaFiscalService service) =>
{
    try
    {
        string caminhoPasta = @"C:\Users\mathe\Downloads\XMLs";
        await service.ProcessarXmlsAsync(caminhoPasta);
        return Results.Ok("Arquivos XML processados e notas fiscais salvas com sucesso.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Erro ao processar XMLs: {ex.Message}");
    }
});

app.MapGet("/notas", async (NotaFiscalContext context) =>
{
    var notas = await context.NotasFiscais.ToListAsync();
    return Results.Ok(notas);
});


app.Run();
