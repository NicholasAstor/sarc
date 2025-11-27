# Room Scheduling API – Reservas, Salas e Usuários

API para gerenciamento de usuários, salas e reservas. Usuários autenticados podem criar e cancelar suas próprias reservas, enquanto administradores gerenciam salas e têm permissões ampliadas. A autenticação é feita via JWT emitido pelo AWS Cognito, seguindo controle de acesso por papéis (RBAC).

## 🧠 Domínio e Fluxos de Negócio

O sistema permite gerenciar salas e reservas feitas por usuários autenticados. O domínio é composto por três entidades principais:

- **User** – representa o usuário autenticado via Cognito.
- **Room** – sala que pode ser reservada.
- **Schedule** – reserva vinculada a um usuário e a uma sala, com horário de início e fim.

### Fluxo 1 — Agendar Sala (User)

1. Usuário autentica e obtém JWT.
2. Seleciona sala, data e horário.
3. API valida conflito de horário e cria o `Schedule`.

### Fluxo 2 — Cancelar Reserva (User/Admin)

1. Usuário acessa suas reservas (`GET /api/v1/schedules/my`).
2. Cancela uma reserva.
3. Apenas o dono ou admin pode cancelar.

### Fluxo 3 — Administração de Salas (Admin)

1. Admin autentica com role `admin`.
2. Cria, altera, lista e remove salas.
3. Endpoints protegidos via `[Authorize(Policy = "AdminOnly")]`.

## 📋 Requisitos

- .NET 8.0 SDK
- Docker & Docker Compose
- AWS Account (para Cognito)
- Terraform (para infraestrutura)

## 🚀 Início Rápido

### 1. Configurar Infraestrutura

```bash
cd infrastructure/
terraform init
terraform plan
terraform apply
```

Após o apply, o Terraform exibirá os outputs necessários:

- `user_pool_id`
- `user_pool_client_id`
- `user_pool_endpoint`

### 2. Configurar Variáveis de Ambiente

Copie o arquivo `.env.example` para `.env` e preencha com os valores do Terraform:

```bash
cp .env.example .env
```

Edite o `.env`:

```bash
JWT_ISSUER=https://cognito-idp.us-east-1.amazonaws.com/us-east-1_XXXXXXXXX
JWT_AUDIENCE=seu-app-client-id
JWKS_URI=https://cognito-idp.us-east-1.amazonaws.com/us-east-1_XXXXXXXXX/.well-known/jwks.json
```

### 3. Criar Usuários no Cognito

Via AWS CLI:

```bash
# Criar usuário admin
aws cognito-idp admin-create-user \
  --user-pool-id us-east-1_XXXXXXXXX \
  --username admin \
  --user-attributes Name=email,Value=admin@example.com \
  --temporary-password TempPass123! \
  --message-action SUPPRESS

# Adicionar ao grupo admin
aws cognito-idp admin-add-user-to-group \
  --user-pool-id us-east-1_XXXXXXXXX \
  --username admin \
  --group-name admin

# Criar usuário regular
aws cognito-idp admin-create-user \
  --user-pool-id us-east-1_XXXXXXXXX \
  --username user \
  --user-attributes Name=email,Value=user@example.com \
  --temporary-password TempPass123! \
  --message-action SUPPRESS

# Adicionar ao grupo user
aws cognito-idp admin-add-user-to-group \
  --user-pool-id us-east-1_XXXXXXXXX \
  --username user \
  --group-name user
```

### 4. Executar a API

#### Com Docker Compose

```bash
docker-compose up --build
```

A API estará disponível em `http://localhost:5000`

#### Desenvolvimento Local

```bash
dotnet restore
dotnet run
```

## 🔑 Como Obter um Access Token

### Método 1: Hosted UI (Recomendado para testes)

1. Acesse o Hosted UI do Cognito:

```
https://{your-domain}.auth.{region}.amazoncognito.com/login?client_id={client_id}&response_type=code&scope=openapi+profile+email&redirect_uri={redirect_uri}
```

2. Faça login com as credenciais do usuário criado

3. Após o redirect, use o código para obter o token:

```bash
curl -X POST https://{your-domain}.auth.{region}.amazoncognito.com/oauth2/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=authorization_code" \
  -d "client_id={client_id}" \
  -d "code={authorization_code}" \
  -d "redirect_uri={redirect_uri}"
```

### Método 2: Via AWS CLI (Admin Flow)

```bash
# Iniciar autenticação
aws cognito-idp admin-initiate-auth \
  --user-pool-id us-east-1_XXXXXXXXX \
  --client-id seu-app-client-id \
  --auth-flow ADMIN_NO_SRP_AUTH \
  --auth-parameters USERNAME=admin,PASSWORD=sua-senha

# O comando retornará um IdToken e AccessToken
```

### Método 3: Script Python Helper

Crie um arquivo `get_token.py`:

```python
import boto3
import base64
import hashlib
import hmac

client = boto3.client('cognito-idp', region_name='us-east-1')

response = client.admin_initiate_auth(
    UserPoolId='us-east-1_XXXXXXXXX',
    ClientId='seu-app-client-id',
    AuthFlow='ADMIN_NO_SRP_AUTH',
    AuthParameters={
        'USERNAME': 'admin',
        'PASSWORD': 'sua-senha'
    }
)

print(f"Access Token: {response['AuthenticationResult']['AccessToken']}")
```

Execute:

```bash
python get_token.py
```

## 📝 Testando a API

## 🔧 Exemplos de chamadas usando cURL

Abaixo estão exemplos reais de chamadas para cada recurso da API.  
Lembre-se de definir previamente o token JWT:

```bash
TOKEN="eyJraWQiOiJ..."   # token de admin ou user, dependendo do exemplo
```

## 👥 Users (Admin / User)

### Listar todos os usuários (admin only)

```bash
curl -X GET \
  http://localhost:5000/api/v1/users \
  -H "Authorization: Bearer $TOKEN"
```

### Buscar usuário específico (admin ou o próprio user)

```bash
curl -X GET \
  http://localhost:5000/api/v1/users/{id} \
  -H "Authorization: Bearer $TOKEN"
```

### Atualizar usuário (admin ou o próprio user)

```bash
curl -X PUT \
  http://localhost:5000/api/v1/users/{id} \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "new@example.com",
    "fullName": "Updated Name"
  }'
```

### Deletar usuário (admin only)

```bash
curl -X DELETE \
  http://localhost:5000/api/v1/users/{id} \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🏢 Rooms (Admin only)

### Listar salas com paginação e filtro

```bash
curl -X GET \
  "http://localhost:5000/api/v1/rooms?page=1&pageSize=10&minCapacity=5" \
  -H "Authorization: Bearer $TOKEN"
```

### Criar sala

```bash
curl -X POST \
  http://localhost:5000/api/v1/rooms \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sala 101",
    "capacity": 12
  }'
```

### Editar sala

```bash
curl -X PUT \
  http://localhost:5000/api/v1/rooms/{roomId} \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sala 101A",
    "capacity": 15,
    "isActive": true
  }'
```

### Deletar sala

```bash
curl -X DELETE \
  http://localhost:5000/api/v1/rooms/{roomId} \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📅 Schedules (User/Admin)

### Criar uma reserva (User ou Admin)

```bash
curl -X POST \
  http://localhost:5000/api/v1/schedules \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": "ROOM_ID_AQUI",
    "startAt": "2025-12-01T14:00:00Z",
    "endAt": "2025-12-01T15:00:00Z"
  }'
```

### Listar minhas reservas (User)

```bash
curl -X GET \
  http://localhost:5000/api/v1/schedules/my \
  -H "Authorization: Bearer $TOKEN"
```

### Cancelar reserva (owner ou admin)

```bash
curl -X DELETE \
  http://localhost:5000/api/v1/schedules/{scheduleId} \
  -H "Authorization: Bearer $TOKEN"
```

### 2. Com Swagger UI

1. Acesse `http://localhost:5000`
2. Clique em "Authorize"
3. Cole o token no campo (sem "Bearer")
4. Teste os endpoints

### 3. Com Postman

1. Importe a collection OpenAPI: `http://localhost:5000/swagger/v1/swagger.json`
2. Configure Authorization → Bearer Token
3. Teste os endpoints

## 🔒 Controle de Acesso (RBAC)

A autenticação é feita via JWT emitido pelo AWS Cognito. As permissões são aplicadas via claims de role (`cognito:groups`) e por ownership (um usuário só acessa recursos que pertencem a ele).

### 🧑‍💼 admin — Acesso total

Administradores podem gerenciar **usuários**, **salas** e **todas as reservas**.

- `/api/v1/users/*` — CRUD completo de usuários
- `/api/v1/rooms/*` — CRUD completo de salas
- `/api/v1/schedules/*` — pode cancelar qualquer reserva
- Pode acessar dados de qualquer usuário

### 🙍 user — Acesso limitado

Usuários autenticados interagem **somente com seus próprios dados** e **suas próprias reservas**.

- `GET/PUT/PATCH /api/v1/users/{id}` — apenas o próprio usuário
- `POST /api/v1/schedules` — criar reserva
- `GET /api/v1/schedules/my` — listar suas reservas
- `DELETE /api/v1/schedules/{id}` — cancelar **apenas reservas próprias**

### 🔐 Ownership

Para recursos vinculados a um usuário (ex.: reservas), a API aplica:

> O usuário só pode acessar ou modificar itens cujo `UserId` seja igual ao `sub` / `NameIdentifier` do token.

Administradores ignoram essa restrição.

## 🧪 Testes

### Executar Testes Unitários

```bash
dotnet test
```

### Executar com Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ver Relatório de Coverage

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
```

Abra `coverage-report/index.html` no navegador.

## 📦 Estrutura do Projeto

```
Sarc/
 ├── Controllers/
 │    ├── UsersController.cs
 │    ├── RoomsController.cs
 │    └── SchedulesController.cs
 │
 ├── DTOs/
 │    ├── CreateRoomDto.cs
 │    ├── UpdateRoomDto.cs
 │    └── CreateScheduleDto.cs
 │
 ├── Model/Entity/
 │    ├── User.cs
 │    ├── Room.cs
 │    └── Schedule.cs
 │
 ├── Repository/
 │    ├── Interface/
 │    ├── RoomRepository.cs
 │    ├── ScheduleRepository.cs
 │    └── UserRepository.cs
 │
 ├── Service/
 │    ├── Interface/
 │    ├── RoomService.cs
 │    ├── ScheduleService.cs
 │    └── UserService.cs
 │
 ├── docker-compose.yml
 ├── Dockerfile
 └── Program.cs

Sarc.Tests/
 ├── IntegrationTests.cs
 ├── UserServiceTests.cs
 └── ScheduleServiceTests.cs

infrastructure/
 └── modules/
      ├── auth/
      ├── database/
      ├── networking/
      ├── ec2/
      └── security/

```

## 🔄 CI/CD Pipeline

Este projeto utiliza GitHub Actions para garantir qualidade contínua da aplicação.  
O pipeline executa automaticamente as seguintes etapas:

1. **Lint**  
   Verifica formatação e padrões de código usando `dotnet format`, garantindo estilo consistente no repositório.

2. **Build**  
   Compila a solução em modo Release para validar que o código está funcional.

3. **Test + Coverage**  
   Executa os testes automatizados (unitários) e gera relatório de cobertura utilizando XPlat Code Coverage.  
   Um resumo da cobertura é publicado automaticamente no Pull Request.

4. **Docker Build & Test**  
   Constrói a imagem Docker da API e executa um container de teste para garantir que a imagem sobe corretamente.

5. **Docs (OpenAPI)**  
   Gera automaticamente a especificação OpenAPI (`openapi.json`) a partir da API rodando localmente no próprio pipeline e a publica como artifact.

### Status dos Checks

- ✅ Lint passa
- ✅ Build passa
- ✅ Tests passa (coverage report gerado e publicado no PR)
- ✅ Docker build passa

## 📚 Documentação da API

### Openapi/v1/Swagger

Acesse `http://localhost:5000` para ver a documentação interativa.

### Endpoints Disponíveis

| Método | Endpoint               | Auth        | Descrição                        |
| ------ | ---------------------- | ----------- | -------------------------------- |
| GET    | /api/v1/users          | Admin       | Lista usuários                   |
| GET    | /api/v1/users/{id}     | Admin/User  | Consulta usuário                 |
| PATCH  | /api/v1/users/{id}     | Admin/User  | Atualiza usuário                 |
| DELETE | /api/v1/users/{id}     | Admin       | Remove usuário                   |
| GET    | /api/v1/rooms          | Admin       | Lista salas (filtro + paginação) |
| POST   | /api/v1/rooms          | Admin       | Cria sala                        |
| PUT    | /api/v1/rooms/{id}     | Admin       | Atualiza sala                    |
| DELETE | /api/v1/rooms/{id}     | Admin       | Remove sala                      |
| POST   | /api/v1/schedules      | User/Admin  | Cria reserva                     |
| GET    | /api/v1/schedules/my   | User/Admin  | Lista reservas do usuário        |
| DELETE | /api/v1/schedules/{id} | Owner/Admin | Cancela reserva                  |

## 🐛 Troubleshooting

### Token inválido

- Verifique se o token não expirou
- Confirme que `JWT_ISSUER` e `JWT_AUDIENCE` estão corretos
- Verifique se o usuário pertence ao grupo correto

### 403 Forbidden

- Verifique se o usuário tem a role necessária
- Confirme que o claim `cognito:groups` está presente no token

### JWKS não encontrado

- Verifique se o User Pool existe e está ativo
- Confirme que a URL do JWKS está correta
- Teste: `curl https://cognito-idp.{region}.amazonaws.com/{pool-id}/.well-known/jwks.json`

## 📖 Referências

- [AWS Cognito Documentation](https://docs.aws.amazon.com/cognito/)
- [JWT.io](https://jwt.io/)
- [OAuth 2.0 RFC](https://oauth.net/2/)
- [OpenID Connect](https://openid.net/connect/)

## 📄 Licença

Este projeto foi desenvolvido para fins acadêmicos.
