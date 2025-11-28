using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(u => MapToReadDto(u));
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        return usuario is null ? null : MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        // Verifica se email já existe
        if (await _repository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException($"Email '{dto.Email}' já está cadastrado.");

        // Cria nova entidade
        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha), // Hash da senha
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        // Adiciona ao repositório
        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario is null)
            throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

        // Verifica se o novo email já existe (e é diferente do atual)
        if (usuario.Email != dto.Email && await _repository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException($"Email '{dto.Email}' já está cadastrado.");

        // Atualiza propriedades
        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario is null)
            return false;

        // SOFT DELETE: Marca como inativo ao invés de remover do banco
        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
    {
        return await _repository.EmailExistsAsync(email, ct);
    }

    // Método auxiliar para mapear Usuario → UsuarioReadDto
    private static UsuarioReadDto MapToReadDto(Usuario usuario)
    {
        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }
}
