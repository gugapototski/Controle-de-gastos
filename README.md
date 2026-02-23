# Sistema de Controle de Gastos

Aplicação Full-Stack desenvolvida como avaliação técnica para vaga de
nível **Pleno**.

O sistema permite o gerenciamento de receitas e despesas com regras de
negócio protegidas no back-end e visualização consolidada no front-end.

------------------------------------------------------------------------

## Tecnologias Utilizadas

### Back-end

-   .NET 9 (C#)
-   Entity Framework Core (Code-First)
-   SQLite
-   Arquitetura em Camadas (API, Application, Domain, Infrastructure)
-   Middleware Global de Tratamento de Erros
-   Testes Unitários com xUnit + Moq

### Front-end

-   React 18 + TypeScript
-   Vite
-   Material UI (MUI v6)
-   Axios com Interceptor Global
-   Recharts

------------------------------------------------------------------------

## Arquitetura

O projeto segue princípios de:

-   Clean Architecture  
-   SOLID  
-   Separação de responsabilidades  
-   Entidades puras no domínio  
-   Controllers magros  
-   Inversão de dependência via abstrações

### Estrutura da Solution

    ControleGastos/
    ├── ControleGastos.Api
    ├── ControleGastos.Application
    ├── ControleGastos.Domain
    ├── ControleGastos.Infrastructure
    ├── ControleGastos.Tests
    └── controle-gastos-front

------------------------------------------------------------------------

## Pré-requisitos

Antes de executar o projeto, instale:

-   .NET 9 SDK
-   Node.js 18+
-   EF Core CLI Tool (caso necessário)

Instalação da ferramenta EF:

``` bash
dotnet tool install --global dotnet-ef
```

------------------------------------------------------------------------

## Como Executar o Back-end (API)

### 1️ Acesse a pasta da API

``` bash
cd ControleGastos.Api
```

### 2️ Restaure as dependências

``` bash
dotnet restore
```

### 3️ Aplique as migrations e crie o banco

O SQLite criará automaticamente o arquivo `.db`.

``` bash
dotnet ef database update --project ../ControleGastos.Infrastructure --startup-project .
```

### 4️ Execute a aplicação

``` bash
dotnet run
```

A API estará disponível em:

    http://localhost:5001

Swagger:

    http://localhost:5001/swagger

------------------------------------------------------------------------

## Executando os Testes

Na raiz da solução:

``` bash
dotnet test
```

Os testes validam as regras de negócio do `DocumentoService`.

------------------------------------------------------------------------

## Como Executar o Front-end

Abra um novo terminal (mantendo a API rodando):

### 1️ Vá até a pasta do front

``` bash
cd controle-gastos-front
```

### 2️ Configure as Variáveis de Ambiente
Crie uma cópia do arquivo de exemplo das variáveis de ambiente para que o React saiba onde a API está rodando:

Renomeie ou copie o arquivo .env Ex para .env (a URL base padrão já está configurada para http://localhost:5001/api).

### 3️ Instale as dependências

``` bash
npm install
```

### 4️ Execute o servidor de desenvolvimento

``` bash
npm run dev
```

A aplicação estará disponível em:

    http://localhost:5173

------------------------------------------------------------------------

## Regras de Negócio Implementadas

-   Menores de 18 anos só podem registrar Despesas.
-   Categoria deve ser compatível com o Tipo da transação.
-   Valor deve ser maior que zero.
-   Exclusão de Pessoa remove todas as suas transações (Cascade Delete).
-   Totais por pessoa calculados diretamente no banco (GroupBy + Sum).

