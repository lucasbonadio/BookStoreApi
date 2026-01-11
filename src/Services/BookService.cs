using BookStoreApi.Data;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Services
{
    public class BookService : IBookService
    {
        private readonly APIDbContext _context;

        public BookService(APIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReadBookDto>> GetAllAsync()
        {
            return await _context.Books
                .Select(b => MapToReadDto(b))
                .ToListAsync();
        }

        public async Task<ReadBookDto?> GetByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            return book == null ? null : MapToReadDto(book);
        }

        public async Task<ReadBookDto> CreateAsync(CreateBookDto createDto)
        {
            var newBook = new Book
            {
                Title = createDto.Title,
                Author = createDto.Author,
                Description = createDto.Description,
                PublicationDate = createDto.PublicationDate
            };

            if (createDto.CoverImage != null)
            {
                using var memoryStream = new MemoryStream();
                await createDto.CoverImage.CopyToAsync(memoryStream);
                newBook.CoverImage = memoryStream.ToArray();
            }

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return MapToReadDto(newBook);
        }

        public async Task<ReadBookDto?> UpdateAsync(int id, UpdateBookDto updateDto)
        {
            var bookToUpdate = await _context.Books.FindAsync(id);
            if (bookToUpdate == null) return null;

            bookToUpdate.Title = updateDto.Title ?? bookToUpdate.Title;
            bookToUpdate.Author = updateDto.Author ?? bookToUpdate.Author;
            bookToUpdate.Description = updateDto.Description ?? bookToUpdate.Description;

            if (updateDto.PublicationDate.HasValue)
                bookToUpdate.PublicationDate = updateDto.PublicationDate.Value;

            if (updateDto.CoverImage != null)
            {
                using var memoryStream = new MemoryStream();
                await updateDto.CoverImage.CopyToAsync(memoryStream);
                bookToUpdate.CoverImage = memoryStream.ToArray();
            }

            await _context.SaveChangesAsync();
            return MapToReadDto(bookToUpdate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
        private static ReadBookDto MapToReadDto(Book book)
        {
            return new ReadBookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                CoverImage = book.CoverImage
            };
        }
    }
}