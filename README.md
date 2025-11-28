# API de Gerenciamento de Usuários

## Descrição

Esta é uma API RESTful desenvolvida em ASP.NET Core 9.0 para gerenciamento de usuários, seguindo os princípios de Clean Architecture e boas práticas de desenvolvimento. A aplicação implementa operações CRUD (Create, Read, Update, Delete) completas com validações robustas, segurança de dados (hash de senhas com BCrypt) e soft delete.

O projeto foi desenvolvido como trabalho acadêmico para demonstrar o conhecimento em desenvolvimento de APIs modernas com .NET, utilizando padrões de projeto consagrados como Repository Pattern, Service Pattern e Dependency Injection.

A API fornece endpoints para criar, listar, buscar, atualizar e remover usuários, com validações de negócio incluindo unicidade de email, idade mínima de 18 anos e normalização de dados.

## Tecnologias Utilizadas

- **.NET 9.0** - Framework principal
- **C# 12.0** - Linguagem de programação
- **ASP.NET Core** - Framework web com Minimal APIs
- **Entity Framework Core 8.0** - ORM para persistência de dados
- **SQLite** - Banco de dados relacional
- **FluentValidation 11.7** - Validações de dados fluentes
- **BCrypt.Net-Next 4.0.3** - Hash seguro de senhas
- **Swashbuckle.AspNetCore 6.5** - Documentação automática com Swagger

## Padrões de Projeto Implementados

- **Repository Pattern** - Abstração da camada de persistência de dados com `IUsuarioRepository` e `UsuarioRepository`
- **Service Pattern** - Lógica de negócio centralizada com `IUsuarioService` e `UsuarioService`
- **DTO Pattern** - Transfer Objects para comunicação entre camadas (`UsuarioCreateDto`, `UsuarioReadDto`, `UsuarioUpdateDto`)
- **Dependency Injection** - Injeção de dependências configurada no `Program.cs`
- **Minimal APIs** - Endpoints RESTful implementados como delegados no `Program.cs`
- **Clean Architecture** - Separação em camadas: Domain, Application, Infrastructure

## Como Executar o Projeto

### Pré-requisitos

- .NET SDK 9.0 ou superior instalado
- Git para clonar o repositório
- Um editor de código (VS Code, Visual Studio, etc.)

### Passos

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/nicolasaoliveira1/api-usuarios-as-nicolas-oliveira.git
   cd api-usuarios-as-nicolas-oliveira
   ```

2. **Navegue até a pasta do projeto:**
   ```bash
   cd APIUsuarios
   ```

3. **Restaure as dependências:**
   ```bash
   dotnet restore
   ```

4. **Execute as migrations (cria o banco de dados):**
   ```bash
   dotnet ef database update
   ```

5. **Execute a aplicação:**
   ```bash
   dotnet run
   ```

6. **Acesse a API:**
   - API: `https://localhost:5001/api/usuarios`
   - Swagger: `https://localhost:5001/swagger`

## Exemplos de Requisições

### 1. Listar todos os usuários
```bash
GET /api/usuarios
```

**Resposta (200 OK):**
```json
[
  {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@email.com",
    "dataNascimento": "2000-05-15T00:00:00",
    "telefone": "(11) 98765-4321",
    "ativo": true,
    "dataCriacao": "2025-11-28T10:30:00"
  }
]
```

### 2. Obter usuário por ID
```bash
GET /api/usuarios/1
```

**Resposta (200 OK):**
```json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@email.com",
  "dataNascimento": "2000-05-15T00:00:00",
  "telefone": "(11) 98765-4321",
  "ativo": true,
  "dataCriacao": "2025-11-28T10:30:00"
}
```

### 3. Criar novo usuário
```bash
POST /api/usuarios
Content-Type: application/json

{
  "nome": "Maria Santos",
  "email": "maria@email.com",
  "senha": "Senha123",
  "dataNascimento": "2000-03-20",
  "telefone": "(21) 99876-5432"
}
```

**Resposta (201 Created):**
```json
{
  "id": 2,
  "nome": "Maria Santos",
  "email": "maria@email.com",
  "dataNascimento": "2000-03-20T00:00:00",
  "telefone": "(21) 99876-5432",
  "ativo": true,
  "dataCriacao": "2025-11-28T11:00:00"
}
```

### 4. Atualizar usuário
```bash
PUT /api/usuarios/1
Content-Type: application/json

{
  "nome": "João Silva Atualizado",
  "email": "joao.novo@email.com",
  "dataNascimento": "2000-05-15",
  "telefone": "(11) 99999-9999",
  "ativo": true
}
```

**Resposta (200 OK):**
```json
{
  "id": 1,
  "nome": "João Silva Atualizado",
  "email": "joao.novo@email.com",
  "dataNascimento": "2000-05-15T00:00:00",
  "telefone": "(11) 99999-9999",
  "ativo": true,
  "dataCriacao": "2025-11-28T10:30:00"
}
```

### 5. Deletar usuário (Soft Delete)
```bash
DELETE /api/usuarios/1
```

**Resposta (204 No Content)**

## Estrutura do Projeto

```
APIUsuarios/
├── Domain/
│   └── Entities/
│       └── Usuario.cs                    # Entidade principal do domínio
│
├── Application/
│   ├── DTOs/
│   │   ├── UsuarioCreateDto.cs          # DTO para criação
│   │   ├── UsuarioReadDto.cs            # DTO para leitura
│   │   └── UsuarioUpdateDto.cs          # DTO para atualização
│   │
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs         # Contrato do repositório
│   │   └── IUsuarioService.cs            # Contrato do serviço
│   │
│   ├── Services/
│   │   └── UsuarioService.cs             # Lógica de negócio
│   │
│   └── Validators/
│       ├── UsuarioCreateDtoValidator.cs  # Validação de criação
│       └── UsuarioUpdateDtoValidator.cs  # Validação de atualização
│
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs               # Contexto do EF Core
│   │
│   └── Repositories/
│       └── UsuarioRepository.cs          # Implementação do repositório
│
├── Migrations/
│   └── ...                               # Migrações do EF Core
│
├── Program.cs                            # Configuração e endpoints
├── appsettings.json                      # Configurações da aplicação
└── APIUsuarios.csproj                    # Arquivo do projeto
```

### Descrição das Camadas

- **Domain**: Contém as entidades e regras de negócio fundamentais
- **Application**: DTOs, interfaces, serviços e validadores (lógica de aplicação)
- **Infrastructure**: Implementações de repositórios e persistência (EF Core)
- **Program.cs**: Configuração de dependências e endpoints (Minimal APIs)

## Validações Implementadas

- **Nome**: Obrigatório, entre 3 e 100 caracteres
- **Email**: Obrigatório, formato válido, deve ser único
- **Senha**: Obrigatória, mínimo 6 caracteres, deve conter maiúscula, minúscula e número
- **Data de Nascimento**: Obrigatória, usuário deve ter mínimo 18 anos
- **Telefone**: Opcional, formato brasileiro `(XX) XXXXX-XXXX`
- **Soft Delete**: Usuário marcado como inativo ao invés de ser removido

## Segurança

- Senhas são hasheadas com **BCrypt** antes de serem armazenadas
- Email é normalizado (convertido para lowercase) para evitar duplicatas
- Índice único no banco de dados garante unicidade de email
- Validação automática com FluentValidation em todos os endpoints
- Tratamento de exceções com status codes HTTP apropriados

## Autor

**Nicolas Oliveira**  
Curso: Análise e Desenvolvimento de Sistemas

