# NutritionBase API

API REST para gerenciamento de pacientes e planos alimentares de nutricionistas, desenvolvida como teste técnico para vaga de Desenvolvedor Pleno .NET.

---

## Stack e Tecnologias

- **.NET 8**
- **SQL Server** — banco de dados relacional
- **Entity Framework Core 8** — ORM com Fluent API e Migrations
- **CQRS com MediatR** — separação de comandos e consultas
- **JWT Bearer** — autenticação stateless *(diferencial)*
- **SecureIdentity** — hashing de senha (BCrypt)
- **Swagger (Swashbuckle)** — documentação interativa com suporte a JWT
- **xUnit + Moq + FluentAssertions** — testes unitários *(diferencial)*

---

## Diferenciais Implementados

| Diferencial         | Status |
|---------------------|--------|
| MediatR             | ✅     |
| Autenticação JWT    | ✅     |
| Testes unitários    | ✅     |

---

## Arquitetura

O projeto segue os princípios de **DDD (Domain-Driven Design)** com **CQRS**, organizado em camadas com separação clara de responsabilidades:

```
src/
├── NutritionBase.Domain/          # Entidades, Value Objects, Interfaces, Exceções
├── NutritionBase.Application/     # Commands, Queries, Handlers, DTOs
├── NutritionBase.Infrastructure/  # DbContext, Repositórios, TokenService, Migrations
├── NutritionBase.Api/             # Controllers, Middlewares, DI, JWT, Swagger
└── NutritionBase.UnitTests/       # Testes unitários de Domain e Application
```

### Dependências entre camadas

```
Domain         ← sem dependências internas
Application    ← Domain
Infrastructure ← Domain + Application
Api            ← Application + Infrastructure
UnitTests      ← Domain + Application
```

---

## Por que existe a entidade Nutricionista?

A entidade `Nutritionist` foi adicionada para suportar **autenticação JWT** como diferencial do teste. Cada nutricionista possui credenciais próprias (email + senha hasheada via BCrypt) e todos os pacientes e planos alimentares estão vinculados ao nutricionista autenticado, garantindo isolamento de dados entre profissionais.

### Fluxo de autenticação

```
POST /api/nutritionists/register  →  Cria o nutricionista com senha hasheada
POST /api/nutritionists/login     →  Valida credenciais e retorna JWT
Authorization: Bearer {token}     →  Todos os endpoints protegidos exigem este header
```

O `NutritionistId` é extraído automaticamente do token em cada requisição. O logout é **client-side** — por ser JWT stateless, não há endpoint de logout no servidor. O token expira em 8 horas.

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server 2019+ (local ou remoto)

---

## Como Rodar Localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/MathMantovan/NutritionBase.git
cd NutritionBase
```

### 2. Configurar o banco de dados

Copie o arquivo de exemplo e preencha com suas configurações:

```bash
cp src/NutritionBase.Api/appsettings.example.json src/NutritionBase.Api/appsettings.json
```

Edite o `appsettings.json`:

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

> O banco de dados e as tabelas são criados automaticamente na primeira execução via `db.Database.Migrate()` no startup.

### 3. Rodar a API

```bash
dotnet run --project src/NutritionBase.Api
```

Acesse o Swagger na URL exibida no console — geralmente `https://localhost:7xxx/swagger` ou `http://localhost:5xxx/swagger`.

### 4. Rodar os testes

Os testes são unitários e **não precisam de banco de dados**.

```bash
dotnet test src/NutritionBase.UnitTests
```

---

## Migrations

As migrations rodam automaticamente no startup. Para gerenciar manualmente:

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration \
  --project src/NutritionBase.Infrastructure/NutritionBase.Infrastructure.csproj \
  --startup-project src/NutritionBase.Api/NutritionBase.Api.csproj

# Aplicar migrations manualmente
dotnet ef database update \
  --project src/NutritionBase.Infrastructure/NutritionBase.Infrastructure.csproj \
  --startup-project src/NutritionBase.Api/NutritionBase.Api.csproj
```

---

## Endpoints

### Nutricionistas

| Método | Rota                            | Descrição                    | Auth |
|--------|---------------------------------|------------------------------|------|
| POST   | `/api/nutritionists/register`   | Cadastrar nutricionista      | —    |
| POST   | `/api/nutritionists/login`      | Login — retorna JWT          | —    |
| GET    | `/api/nutritionists/{id}`       | Buscar nutricionista por ID  | ✓    |
| PUT    | `/api/nutritionists`            | Atualizar nome e email       | ✓    |

### Pacientes

| Método | Rota                          | Descrição                          | Auth |
|--------|-------------------------------|------------------------------------|------|
| POST   | `/api/patients`               | Cadastrar paciente                 | ✓    |
| GET    | `/api/patients`               | Listar pacientes do nutricionista  | ✓    |
| GET    | `/api/patients/{id}`          | Buscar paciente por ID             | ✓    |
| GET    | `/api/patients/name/{name}`   | Buscar paciente por nome           | ✓    |
| PUT    | `/api/patients/{id}`          | Atualizar paciente                 | ✓    |
| DELETE | `/api/patients/{id}`          | Remover paciente                   | ✓    |

### Planos Alimentares

| Método | Rota                                           | Descrição                         | Auth |
|--------|------------------------------------------------|-----------------------------------|------|
| POST   | `/api/mealplans`                               | Criar plano alimentar             | ✓    |
| GET    | `/api/mealplans/{id}`                          | Buscar plano completo             | ✓    |
| GET    | `/api/mealplans/patient/{patientId}`           | Listar planos de um paciente      | ✓    |
| POST   | `/api/mealplans/{id}/meals`                    | Adicionar refeição com alimentos  | ✓    |
| DELETE | `/api/mealplans/{mealPlanId}/meals/{mealId}`   | Remover refeição                  | ✓    |

---

## Como Testar via Swagger

1. `POST /api/nutritionists/register` — crie um nutricionista
2. `POST /api/nutritionists/login` — copie o `token` retornado
3. Clique em **Authorize** no canto superior direito do Swagger
4. Informe `Bearer {token}` e confirme
5. Todos os endpoints protegidos funcionam automaticamente a partir daí

### Exemplos de body

**Registrar nutricionista**
```json
{
  "name": "Dr. Carlos Silva",
  "email": "carlos@nutri.com",
  "password": "Senha@123"
}
```

**Cadastrar paciente**
```json
{
  "name": "Ana Souza",
  "email": "ana@email.com",
  "areaCode": "11",
  "phoneNumber": "987654321",
  "birthDate": "1990-05-15",
  "weight": 65.5,
  "height": 1.68
}
```

**Criar plano alimentar**
```json
{
  "patientId": "{id-do-paciente}",
  "name": "Plano Emagrecimento",
  "objective": "Perda de peso saudável",
  "startDate": "2026-06-01",
  "endDate": "2026-08-31"
}
```

**Adicionar refeição com alimentos**
```json
{
  "name": "Café da Manhã",
  "mealTime": "07:00:00",
  "foodItems": [
    {
      "name": "Aveia",
      "quantity": 50,
      "unit": 0,
      "calories": 180
    },
    {
      "name": "Suco de Laranja",
      "quantity": 200,
      "unit": 1,
      "calories": 90
    },
    {
      "name": "Ovo Cozido",
      "quantity": 2,
      "unit": 2,
      "calories": 140
    }
  ]
}
```

> `unit`: `0` = Gramas, `1` = Mililitros, `2` = Unidades

---

## Regras de Negócio

- Email de paciente não pode duplicar por nutricionista
- Peso e altura do paciente devem ser maiores que zero
- Data de nascimento deve ser no passado
- Data final do plano alimentar deve ser maior que a data de início
- Nome do plano alimentar não pode duplicar para o mesmo paciente
- Nome da refeição não pode duplicar dentro do mesmo plano alimentar
- Quantidade e calorias dos alimentos devem ser maiores que zero
- Erros de regra de negócio retornam **HTTP 400** com mensagem descritiva
- Endpoints protegidos sem token retornam **HTTP 401**

---

## Testes Unitários

Cobrem Domain e Application sem dependência de banco de dados:

```
Domain/
├── ValueObjects/   EmailTests, PhoneTests
└── Entities/       PatientTests, NutritionistTests, MealPlanTests, MealTests, FoodItemTests

Application/
├── Patients/       Create, Update, Remove, GetById, List, GetByName
├── MealPlans/      Create, AddMeal, RemoveMeal, GetById, ListByPatient
└── Nutritionists/  Create, Login, Update, GetById
```
