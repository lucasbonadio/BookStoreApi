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
        ResetDatabase();

        var response = await _client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }

    [Fact]
    public async Task CreateBook_ShouldReturnCreated_WhenDataIsValid()
    {
        ResetDatabase();

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new StringContent("O Senhor dos Anéis"), "Title");
        multipartContent.Add(new StringContent("J.R.R. Tolkien"), "Author");
        multipartContent.Add(new StringContent("Uma jornada épica."), "Description");
        multipartContent.Add(new StringContent(DateTime.Now.ToString("o")), "PublicationDate");

        var fileContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
        fileContent.Headers.Add("Content-Type", "image/jpeg");
        multipartContent.Add(fileContent, "CoverImage", "capa-teste.jpg"); 

        var response = await _client.PostAsync("/api/books", multipartContent);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdBook = await response.Content.ReadFromJsonAsync<ReadBookDto>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be("O Senhor dos Anéis");
    }

    [Fact]
    public async Task GetBook_ShouldReturnBook_WhenIdExists()
    {
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
        var response = await _client.GetAsync("/api/books/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        var book = await response.Content.ReadFromJsonAsync<ReadBookDto>();
        book!.Title.Should().Be("Teste Get");
    }

    [Fact]
    public async Task DeleteBook_ShouldReturnOk_WhenBookExists()
    {
        ResetDatabase();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<APIDbContext>();
            context.Books.Add(new Book { Title = "Para Deletar", Author = "X", Description="Y", CoverImage= new byte[0] });
            await context.SaveChangesAsync();
        }

        var response = await _client.DeleteAsync("/api/books/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/books/1");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}