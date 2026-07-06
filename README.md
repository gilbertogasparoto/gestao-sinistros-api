# Gestão de Sinistros - Teste

## Tecnologias utilizadas

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* Docker / Docker Compose
* xUnit (testes unitários)

---

## Decisões técnicas

Estas foram algumas decisões técnicas que adotei no projeto além das regras de negócio descritas no teste:

* Aplicação automática das migrations na inicialização da aplicação.
* Seed inicial para popular o banco de dados com clientes, apólices, sinistros e histórico de alterações.
* Soft Delete ao invés de deletar os dados do banco permanentemente.
* Middleware global para tratamento de exceções e padronização das respostas de erro.
* Validação centralizada dos DTOs.
* Fluxo de status validado em regra de negócio, impedindo transições inválidas.
* Paginação, pesquisa e filtros em todos os endpoints de listagem.

---

## Arquitetura

### API

Responsável pela exposição da API REST e pela camada de aplicação.

* Controllers
* Services
* DTOs
* Middlewares
* Common (tratamento de erros)

### Domain

Responsável pelo domínio da aplicação.

* Entities
* Enums

### Infrastructure

Responsável pela persistência de dados.

* Entity Configurations
* Migrations
* Seed de dados para ambiente de desenvolvimento

---

## Executando o projeto

### Pré-requisitos

* .NET SDK 8
* Docker Desktop
* PostgreSQL (caso não utilize Docker)

---

### 1. Clone o repositório

```bash
git clone https://github.com/gilbertogasparoto/gestao-sinistros-api.git
```

---

### 2. Configure o banco de dados

Na raiz do projeto existe um arquivo `docker-compose.yml` contendo a configuração do banco PostgreSQL.

Caso utilize Docker, basta executar:

```bash
docker compose up -d
```

Se preferir utilizar uma instância própria do PostgreSQL, basta configurar a conexão no arquivo:

```text
appsettings.json
```

Alterando a Connection String para a sua base de dados.

---

### 3. Aplicar as migrations

```bash
dotnet ef database update
```

### 4. Executar a aplicação

```bash
dotnet restore 
dotnet run
```

Na primeira execução:

* As migrations serão aplicadas automaticamente.
* O Seed inicial será executado automaticamente.

Após iniciar a aplicação, a documentação da API estará disponível pelo Swagger.

Acesse:

https://localhost:5119/swagger

Todos os endpoints implementados podem ser testados diretamente pela interface do Swagger.

---

## Testes

Para executar os testes unitários:

```bash
dotnet test
```
