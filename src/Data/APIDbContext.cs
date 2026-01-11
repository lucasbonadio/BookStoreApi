using Microsoft.EntityFrameworkCore;
using BookStoreApi.Models;

namespace BookStoreApi.Data
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }
        
        public DbSet<Book> Books { get; set; }
    }
}