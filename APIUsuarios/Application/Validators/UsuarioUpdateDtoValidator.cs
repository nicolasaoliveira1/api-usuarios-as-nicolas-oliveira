using Application.DTOs;
using Application.Interfaces;
using FluentValidation;

namespace Application.Validators;

public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioUpdateDtoValidator(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;

        // Nome: obrigatório, entre 3 e 100 caracteres
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.")
            .MinimumLength(3)
            .WithMessage("Nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(100)
            .WithMessage("Nome deve ter no máximo 100 caracteres.");

        // Email: obrigatório, formato válido
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.")
            .EmailAddress()
            .WithMessage("Email deve estar em um formato válido.");

        // DataNascimento: obrigatória, valida se é passado e maior de 18 anos
        RuleFor(x => x.DataNascimento)
            .NotEmpty()
            .WithMessage("Data de nascimento é obrigatória.")
            .LessThan(DateTime.Today.AddYears(-18))
            .WithMessage("Usuário deve ter no mínimo 18 anos de idade.");

        // Telefone: opcional, mas se preenchido deve ter formato brasileiro válido
        RuleFor(x => x.Telefone)
            .Matches(@"^\(\d{2}\) \d{5}-\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Telefone))
            .WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX.");

        // Email deve ser único (exceto o próprio usuário)
        // Este será validado no Service durante a atualização
    }
}
