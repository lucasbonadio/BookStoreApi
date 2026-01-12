# 📚 BookStore API

API RESTful desenvolvida em .NET 8 para gerenciamento de uma livraria. O projeto adota boas práticas de arquitetura, separando a aplicação em camadas e incluindo **Testes de Integração** para garantir a confiabilidade dos endpoints.

O sistema permite operações completas de CRUD (Criar, Ler, Atualizar, Deletar) de livros, com destaque para o **upload de imagens de capa** e armazenamento seguro em banco de dados MySQL.

## 🚀 Tecnologias Utilizadas

* **Linguagem:** C# (.NET 8)
* **Banco de Dados:** MySQL 8.0
* **ORM:** Entity Framework Core
* **Containerização:** Docker & Docker Compose
* **Documentação:** Swagger (OpenAPI)
* **Arquitetura:** Separação de responsabilidades (src/tests)

---

## ⚙️ Pré-requisitos

Para rodar este projeto, você precisa ter instalado em sua máquina:

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Recomendado)
* **OU** .NET 8 SDK e um servidor MySQL local.

---

## 🐳 Como rodar (Modo Docker - Recomendado)

Esta é a maneira mais fácil de rodar a aplicação, pois o Docker configura o banco de dados e a API automaticamente.

1.  **Clone o repositório ou navegue até a pasta do projeto.**
2.  **Abra o terminal e execute o comando:**

    ```bash
    docker compose up --build
    ```

3.  **Aguarde a inicialização.**
    * O MySQL será iniciado na porta `3307` (para não conflitar com seu MySQL local).
    * A API aguardará o banco ficar pronto e aplicará as migrações (criação de tabelas) automaticamente.

4.  **Acesse a documentação (Swagger):**
    Abra seu navegador em: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 💻 Como rodar (Modo Manual / Desenvolvimento Local)

Caso queira rodar sem o Docker (apenas com o .NET instalado):

1.  **Configure o Banco de Dados:**
    No arquivo `appsettings.json`, ajuste a `DefaultConnection` para apontar para o seu MySQL local:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=books;User=root;Password=SUA_SENHA;"
    }
    ```

2.  **Aplique as Migrações:**
    No terminal, dentro da pasta do projeto, execute:
    ```bash
    dotnet ef database update
    ```

3.  **Execute a API na pasta src:**
    ```bash
    dotnet watch run
    ```
    A API estará disponível geralmente em `http://localhost:5xxx` (verifique o terminal).

---

## 🧪 Como rodar os Testes

O projeto conta com uma suíte de testes automatizados para validar o comportamento da API.

1.  **Na raiz do projeto, execute:**

    ```bash
    dotnet test
    ```

Isso irá compilar a aplicação e rodar todos os testes localizados na pasta `tests/`, exibindo o relatório de sucesso/falha no terminal.

---

## 🔌 Endpoints da API

A API documentada via Swagger possui os seguintes endpoints principais:

### Livros (`/api/books`)

| Método | Endpoint | Descrição | Formato |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/books` | Lista todos os livros | JSON |
| `GET` | `/api/books/{id}` | Busca um livro específico | JSON |
| `POST` | `/api/books` | Cadastra um novo livro | `multipart/form-data` |
| `PUT` | `/api/books/{id}` | Atualiza um livro existente | `multipart/form-data` |
| `DELETE` | `/api/books/{id}` | Remove um livro | - |

> **Nota sobre Imagens:** Os endpoints de `POST` e `PUT` esperam o formato `multipart/form-data` para permitir o envio do arquivo de imagem (`CoverImage`). A imagem é convertida e salva como binário (BLOB) no banco.

---

## 🛠 Estrutura do Projeto

* **Controllers:** Responsáveis exclusivamente por lidar com as requisições HTTP (entrada) e devolver os status codes corretos (saída). Não possuem regras de negócio, apenas delegam chamadas para os Services.
* **Services:** Camada onde reside a **Lógica de Negócio**. Responsável por processar os dados, realizar validações lógicas e fazer o mapeamento entre DTOs e Entidades.
* **Interfaces:** Definem os contratos dos serviços (ex: `IBookService`), permitindo o uso de **Injeção de Dependência** e facilitando a criação de testes unitários.
* **Models:** Representam as entidades do domínio e as tabelas do banco de dados (ex: `Book`).
* **DTOs (Data Transfer Objects):** Objetos utilizados para transportar dados entre as camadas, prevenindo a exposição direta das entidades do banco (Over-posting).
    * `CreateBookDto`: Contrato para criação.
    * `UpdateBookDto`: Contrato para atualização.
    * `ReadBookDto`: Contrato de resposta (leitura).
* **Data:** Configuração do contexto do Entity Framework e acesso ao banco de dados.