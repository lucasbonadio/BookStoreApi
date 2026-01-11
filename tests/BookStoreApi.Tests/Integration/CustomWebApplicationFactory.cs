using BookStoreApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Essa classe cria uma versão "fake" da sua API para os testes
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Encontra a configuração do Banco de Dados Real (SQL Server)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<APIDbContext>));

            // 2. Remove a configuração do banco real
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // 3. Adiciona o Banco em Memória para testes
            services.AddDbContext<APIDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }
}