# NutritionBase API

API REST para gerenciamento de pacientes e planos alimentares de nutricionistas. Desenvolvida como teste técnico para vaga de Desenvolvedor Pleno .NET.

---

## Stack e Tecnologias

- **.NET 8**
- **SQL Server** — banco de dados relacional
- **Entity Framework Core 8** — ORM com Fluent API e Migrations
- **CQRS com MediatR** — separação de comandos e consultas
- **JWT Bearer** — autenticação stateless
- **SecureIdentity** — hashing de senha (BCrypt)
- **Swagger (Swashbuckle)** — documentação interativa da API
- **xUnit + Moq + FluentAssertions** — testes unitários

---

## Arquitetura

O projeto segue os princípios de **DDD (Domain-Driven Design)** com **CQRS**, organizado em camadas:

```
src/
├── NutritionBase.Domain/          # Entidades, Value Objects, Interfaces, Exceções
├── NutritionBase.Application/     # Commands, Queries, Handlers, DTOs
├── NutritionBase.Infrastructure/  # DbContext, Repositórios, TokenService
├── NutritionBase.Api/             # Controllers, Middlewares, DI, JWT
└── NutritionBase.UnitTests/       # Testes unitários de Handlers e Domain
```

### Dependências entre camadas

```
Domain        ← sem dependências internas
Application   ← Domain
Infrastructure← Domain + Application
Api           ← Application + Infrastructure
UnitTests     ← Domain + Application
```

---

## Por que existe a entidade Nutricionista?

A entidade `Nutritionist` foi criada para suportar **autenticação JWT**, funcionando como o usuário do sistema. Cada nutricionista possui suas próprias credenciais (email + senha hasheada) e todos os pacientes e planos alimentares estão vinculados ao nutricionista autenticado.

### Fluxo de autenticação

**Registro:**
```
POST /api/nutritionists/register
→ Cria o nutricionista com senha hasheada via BCrypt (SecureIdentity)
```

**Login:**
```
POST /api/nutritionists/login
→ Verifica as credenciais
→ Retorna um JWT com o NutritionistId na claim
```

**Uso do token:**
```
Authorization: Bearer {token}
→ Todos os endpoints de Pacientes e Planos Alimentares exigem este header
→ O NutritionistId é extraído automaticamente do token em cada request
```

**Logout:**

Por ser JWT stateless, não há endpoint de logout no servidor. O logout é feito **no lado do cliente** descartando o token armazenado (localStorage, cookie, etc.). O token expira automaticamente após 8 horas.

---

## Como Reproduzir o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server local (ou acesso a uma instância remota)

### 1. Configurar o banco de dados

Copie o arquivo de exemplo e preencha com suas configurações:

```bash
cp src/NutritionBase.Api/appsettings.example.json src/NutritionBase.Api/appsettings.json
```

Edite o `appsettings.json` com sua connection string e uma chave JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=NutritionBase;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES",
    "Issuer": "NutritionBase.Api",
    "Audience": "NutritionBase.Client"
  }
}
```

> O banco de dados e as tabelas são criados automaticamente na primeira execução via migrations.

### 2. Rodar a API

```bash
dotnet run --project src/NutritionBase.Api
```

### 3. Acessar o Swagger

Abra no navegador a URL exibida no console (geralmente `https://localhost:7xxx/swagger` ou `http://localhost:5xxx/swagger`).

### 4. Rodar os testes

```bash
dotnet test src/NutritionBase.UnitTests
```

---

## Endpoints

### Nutricionistas

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/nutritionists/register` | Cadastrar nutricionista | — |
| POST | `/api/nutritionists/login` | Login — retorna JWT | — |
| GET | `/api/nutritionists/{id}` | Buscar nutricionista por ID | ✓ |
| PUT | `/api/nutritionists` | Atualizar nome e email | ✓ |

### Pacientes

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/patients` | Cadastrar paciente | ✓ |
| GET | `/api/patients` | Listar pacientes do nutricionista | ✓ |
| GET | `/api/patients/{id}` | Buscar paciente por ID | ✓ |
| GET | `/api/patients/name/{name}` | Buscar paciente por nome | ✓ |
| PUT | `/api/patients/{id}` | Atualizar paciente | ✓ |
| DELETE | `/api/patients/{id}` | Remover paciente | ✓ |

### Planos Alimentares

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/mealplans` | Criar plano alimentar | ✓ |
| GET | `/api/mealplans/{id}` | Buscar plano com refeições | ✓ |
| GET | `/api/mealplans/patient/{patientId}` | Listar planos de um paciente | ✓ |
| POST | `/api/mealplans/{id}/meals` | Adicionar refeição com alimentos | ✓ |
| DELETE | `/api/mealplans/{mealPlanId}/meals/{mealId}` | Remover refeição | ✓ |

---

## Como usar o Swagger com autenticação

1. Chame `POST /api/nutritionists/register` para criar um nutricionista
2. Chame `POST /api/nutritionists/login` e copie o `token` retornado
3. Clique em **Authorize** (canto superior direito do Swagger)
4. Informe `Bearer {token}` e confirme
5. Todos os endpoints protegidos passam a funcionar automaticamente

---

## Regras de Negócio

- Email de paciente não pode duplicar por nutricionista
- Peso e altura do paciente devem ser maiores que zero
- Data de nascimento deve ser no passado
- Data final do plano alimentar deve ser maior que a data de início
- Nome do plano alimentar não pode duplicar para o mesmo paciente
- Nome da refeição não pode duplicar dentro do mesmo plano alimentar
- Quantidade e calorias dos alimentos devem ser maiores que zero
- Erros de regra de negócio retornam HTTP 400 com a mensagem descritiva
