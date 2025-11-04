# User Management API - Autenticação com AWS Cognito

API REST com autenticação JWT via AWS Cognito, controle de acesso baseado em roles (RBAC) e infraestrutura como código.

## 📋 Requisitos

- .NET 8.0 SDK
- Docker & Docker Compose
- AWS Account (para Cognito)
- Terraform (para infraestrutura)

## 🚀 Início Rápido

### 1. Configurar Infraestrutura

```bash
cd infra/
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

### 1. Com cURL

```bash
# Obter token (exemplo com resultado do método 2)
TOKEN="eyJraWQiOiJ..."

# Listar todos os usuários (admin only)
curl -X GET http://localhost:5000/api/users \
  -H "Authorization: Bearer $TOKEN"

# Buscar usuário específico
curl -X GET http://localhost:5000/api/users/admin-001 \
  -H "Authorization: Bearer $TOKEN"

# Atualizar usuário
curl -X PUT http://localhost:5000/api/users/user-001 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newemail@example.com",
    "fullName": "Updated Name"
  }'

# Deletar usuário (admin only)
curl -X DELETE http://localhost:5000/api/users/user-001 \
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

### Roles Disponíveis

- **admin**: Acesso total

  - GET /api/users (listar todos)
  - DELETE /api/users/:id (deletar qualquer usuário)
  - GET/PUT/PATCH /api/users/:id (acessar qualquer usuário)

- **user**: Acesso limitado
  - GET/PUT/PATCH /api/users/:id (apenas próprio usuário)

### Estrutura de Claims JWT

```json
{
  "sub": "user-id-123",
  "cognito:groups": ["admin"],
  "email": "user@example.com",
  "iss": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_XXXXXXXXX",
  "aud": "client-id",
  "exp": 1234567890
}
```

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
.
├── Controllers/
│   └── UsersController.cs       # Endpoints da API
├── Models/
│   └── User.cs                   # Modelos de dados
├── Repositories/
│   └── IUserRepository.cs        # Camada de dados
├── Services/
│   └── UserService.cs            # Lógica de negócio
├── UserManagementApi.Tests/
│   ├── UserServiceTests.cs       # Testes unitários
│   └── IntegrationTests.cs       # Testes de integração
├── infra/
│   └── main.tf                   # Terraform (Cognito)
├── .github/
│   └── workflows/
│       └── ci.yml                # Pipeline CI/CD
├── docker-compose.yml
├── Dockerfile
├── Program.cs
└── README.md
```

## 🔄 CI/CD Pipeline

O pipeline GitHub Actions executa automaticamente:

1. **Lint**: Verificação de formatação de código
2. **Build**: Compilação do projeto
3. **Test**: Testes unitários e de integração
4. **Coverage**: Relatório de cobertura de código
5. **Docker**: Build e teste da imagem Docker
6. **Docs**: Geração da especificação OpenAPI

### Status dos Checks

- ✅ Lint passa
- ✅ Build passa
- ✅ Tests passa (> 80% coverage)
- ✅ Docker build passa

## 📚 Documentação da API

### OpenAPI/Swagger

Acesse `http://localhost:5000` para ver a documentação interativa.

### Endpoints Disponíveis

| Método | Endpoint       | Auth        | Descrição               |
| ------ | -------------- | ----------- | ----------------------- |
| GET    | /api/users     | Admin       | Lista todos os usuários |
| GET    | /api/users/:id | Owner/Admin | Busca usuário por ID    |
| PUT    | /api/users/:id | Owner/Admin | Atualiza usuário        |
| PATCH  | /api/users/:id | Owner/Admin | Atualiza parcialmente   |
| DELETE | /api/users/:id | Admin       | Remove usuário          |

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
