using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using BookStoreApi.Data;

public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public BooksControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Helper para limpar o banco antes de cada teste (para um teste não afetar o outro)
    private void ResetDatabase()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<APIDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    [Fact]
    public async Task GetBooks_ShouldReturnEmptyList_WhenNoBooksExist()
    {
        // Arrange
        ResetDatabase();

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert - Sua API retorna 404 quando a lista é vazia/null
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBook_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        ResetDatabase();

        // Como sua API espera [FromForm] com IFormFile, precisamos montar um MultipartFormDataContent
        using var multipartContent = new MultipartFormDataContent();
        
        multipartContent.Add(new StringContent("O Senhor dos Anéis"), "Title");
        multipartContent.Add(new StringContent("J.R.R. Tolkien"), "Author");
        multipartContent.Add(new StringContent("Uma jornada épica."), "Description");
        multipartContent.Add(new StringContent(DateTime.Now.ToString("o")), "PublicationDate");

        // Criando uma imagem falsa em memória (bytes vazios ou fake)
        var fileContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }); // Header fake de JPG
        fileContent.Headers.Add("Content-Type", "image/jpeg");
        
        // "CoverImage" deve bater com o nome da propriedade no seu CreateBookDto
        multipartContent.Add(fileContent, "CoverImage", "capa-teste.jpg"); 

        // Act
        var response = await _client.PostAsync("/api/books", multipartContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verifica se retornou o objeto criado
        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be("O Senhor dos Anéis");
    }

    [Fact]
    public async Task GetBook_ShouldReturnBook_WhenIdExists()
    {
        // Arrange - Precisamos criar um livro primeiro para poder buscar ele
        ResetDatabase();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<APIDbContext>();
            context.Books.Add(new Book 
            { 
                Title = "Teste Get", 
                Author = "Autor Teste", 
                Description = "Desc", 
                PublicationDate = DateTime.Now,
                CoverImage = new byte[0]
            });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/books/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var book = await response.Content.ReadFromJsonAsync<ReadBookDto>();
        book!.Title.Should().Be("Teste Get");
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        ResetDatabase();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<APIDbContext>();
            context.Books.Add(new Book { Title = "Para Deletar", Author = "X", Description="Y", CoverImage= new byte[0] });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.DeleteAsync("/api/books/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Confirmação dupla: Tenta buscar o livro deletado, deve dar 404
        var getResponse = await _client.GetAsync("/api/books/1");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}