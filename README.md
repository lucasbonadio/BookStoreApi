# 📚 BookStore API

API RESTful desenvolvida em .NET 8 para gerenciamento de uma livraria. O projeto permite operações completas de CRUD (Criar, Ler, Atualizar, Deletar) de livros, incluindo suporte para **upload de imagens de capa** e armazenamento seguro em banco de dados MySQL.

## 🚀 Tecnologias Utilizadas

* **Linguagem:** C# (.NET 8)
* **Banco de Dados:** MySQL 8.0
* **ORM:** Entity Framework Core
* **Containerização:** Docker & Docker Compose
* **Documentação:** Swagger (OpenAPI)

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

3.  **Execute a API:**
    ```bash
    dotnet watch run
    ```
    A API estará disponível geralmente em `http://localhost:5xxx` (verifique o terminal).

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

* **Controllers:** Gerenciam as requisições HTTP e respostas.
* **Models:** Representam as tabelas do banco de dados (`Book`).
* **Dtos:** (Data Transfer Objects) Objetos simplificados para entrada e saída de dados, garantindo segurança e validação.
    * `CreateBookDto`: Validações obrigatórias para cadastro.
    * `UpdateBookDto`: Campos opcionais para atualização parcial.
    * `ReadBookDto`: Formato de entrega dos dados para o Front-end.
* **Data:** Contexto do Banco de Dados (`AppDbContext`) e Migrações.