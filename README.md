# Projeto — Sprint 0: Setup de Time, Stack e Projeto

## 🎯 Objetivo

Preparar ambiente inicial, stack definida e estrutura mínima do projeto, incluindo um CRUD simples de exemplo e CI básico.

## 🧱 Stack

- **Linguagem:** C# (.NET 8)
- **Framework:** ASP.NET Core Web API
- **Banco de Dados:** PostgreSQL (via Docker)
- **Testes:** NUnit / MSTest
- **CI/CD:** GitHub Actions (build + testes)

## 📁 Estrutura do Repositório

```
.
├── .github/workflows/build.yml     # pipeline CI
├── Controllers/                    # Controllers da API (ex: WeatherForecastController)
├── Properties/                     # Configurações locais
├── Program.cs                      # Entrypoint da aplicação
├── sarc.csproj                     # Projeto principal
├── sarc.sln                        # Solução
├── docker-compose.yml              # Orquestração com Postgres
├── appsettings.json                # Configurações padrão
├── appsettings.Development.json    # Configurações de desenvolvimento
└── README.md                       # Este arquivo
```

## ▶️ Como Rodar com Docker

Pré-requisitos: [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) instalados.

```bash
# subir containers
docker compose up --build

# parar containers
docker compose down
```

- API disponível em: **http://localhost:5000**
- Banco disponível em: **localhost:5432**

## ▶️ Como Rodar Localmente (sem Docker)

Pré-requisitos:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- PostgreSQL rodando localmente

Passos:

```bash
# restaurar dependências
dotnet restore

# rodar aplicação
dotnet run
```

Por padrão, a API sobe em:

- `https://localhost:5001`
- `http://localhost:5000`

## 🧩 CRUD de User

> Obs.: O template inicial gera `WeatherForecastController`.  
> Nesta sprint, o domínio de exemplo deve ser `User`, com operações CRUD.

**Exemplo de endpoints:**

| Método | Rota              | Descrição               |
| ------ | ----------------- | ----------------------- |
| GET    | `/api/users`      | Lista todos os usuários |
| GET    | `/api/users/{id}` | Busca usuário por ID    |
| POST   | `/api/users`      | Cria um novo usuário    |
| PUT    | `/api/users/{id}` | Atualiza um usuário     |
| DELETE | `/api/users/{id}` | Remove um usuário       |

## 🧪 Testes

Rodar os testes automatizados:

```bash
dotnet test
```

## 🤖 CI — GitHub Actions

Arquivo: `.github/workflows/build.yml`  
Pipeline executa automaticamente em cada **push** ou **pull request**:

- Restore das dependências
- Build do projeto
- Execução dos testes

```yaml
name: Build & Test

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
```
