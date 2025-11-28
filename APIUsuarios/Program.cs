using Infrastructure.Persistence;
using Application.Interfaces;
using Infrastructure.Repositories;
using Application.Services;
using Application.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra o Repository e o Service com injeção de dependência
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Registra os validadores do FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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


// GET /api/usuarios - Lista todos os usuários
app.MapGet("/api/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuarios = await service.ListarAsync(ct);
        return Results.Ok(usuarios);
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("ListarUsuarios")
.WithOpenApi();

// GET /api/usuarios/{id} - Obtém um usuário por ID
app.MapGet("/api/usuarios/{id}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuario = await service.ObterAsync(id, ct);
        
        if (usuario is null)
            return Results.NotFound(new { mensagem = $"Usuário com ID {id} não encontrado" });
        
        return Results.Ok(usuario);
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("ObterUsuario")
.WithOpenApi();

// POST /api/usuarios - Cria um novo usuário
app.MapPost("/api/usuarios", async (UsuarioCreateDto dto, IUsuarioService service, IValidator<UsuarioCreateDto> validator, CancellationToken ct) =>
{
    try
    {
        // Valida com FluentValidation
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        // Normaliza email para lowercase
        var dtoNormalizado = dto with { Email = dto.Email.ToLower().Trim() };
        
        var usuarioCriado = await service.CriarAsync(dtoNormalizado, ct);
        
        return Results.Created($"/api/usuarios/{usuarioCriado.Id}", usuarioCriado);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { mensagem = ex.Message });
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("CriarUsuario")
.WithOpenApi();

// PUT /api/usuarios/{id} - Atualiza um usuário
app.MapPut("/api/usuarios/{id}", async (int id, UsuarioUpdateDto dto, IUsuarioService service, IValidator<UsuarioUpdateDto> validator, CancellationToken ct) =>
{
    try
    {
        // Valida com FluentValidation
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new { erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        // Normaliza email para lowercase
        var dtoNormalizado = dto with { Email = dto.Email.ToLower().Trim() };
        
        var usuarioAtualizado = await service.AtualizarAsync(id, dtoNormalizado, ct);
        
        return Results.Ok(usuarioAtualizado);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { mensagem = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { mensagem = ex.Message });
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("AtualizarUsuario")
.WithOpenApi();

// DELETE /api/usuarios/{id} - Remove um usuário (soft delete)
app.MapDelete("/api/usuarios/{id}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var removido = await service.RemoverAsync(id, ct);
        
        if (!removido)
            return Results.NotFound(new { mensagem = $"Usuário com ID {id} não encontrado" });
        
        return Results.NoContent();
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("RemoverUsuario")
.WithOpenApi();

app.Run();
