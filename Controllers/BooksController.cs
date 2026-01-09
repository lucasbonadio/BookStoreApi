// Controllers/BooksController.cs
using BookStoreApi.Data;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly APIDbContext _context;

        public BooksController(APIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadBookDto>>> GetBooks()
        {
            var books = await _context.Books.Select(b => new ReadBookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                PublicationDate = b.PublicationDate,
                CoverImage = b.CoverImage
            })
                .ToListAsync();

            if (books == null || books.Count == 0)
            {
                return NotFound(new { Message = "Nenhum livro encontrado." });
            }

            return books;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadBookDto>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }

            var bookDto = new ReadBookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                CoverImage = book.CoverImage
            };

            return bookDto;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromForm] CreateBookDto book)
        {
            var newBook = new Book
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
            };

            if (book.CoverImage != null)
            {
                using var memoryStream = new MemoryStream();
                await book.CoverImage.CopyToAsync(memoryStream);
                newBook.CoverImage = memoryStream.ToArray();
            }

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReadBookDto>> PutBook(int id, [FromForm] UpdateBookDto book)
        {
            var bookToUpdate = await _context.Books.FindAsync(id);


            if (bookToUpdate == null)
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }


            bookToUpdate.Title = book.Title ?? bookToUpdate.Title;
            bookToUpdate.Author = book.Author ?? bookToUpdate.Author;
            bookToUpdate.Description = book.Description ?? bookToUpdate.Description;

            if (book.PublicationDate.HasValue)
            {
                bookToUpdate.PublicationDate = book.PublicationDate.Value;
            }

            if (book.CoverImage != null)
            {
                using var memoryStream = new MemoryStream();
                await book.CoverImage.CopyToAsync(memoryStream);
                bookToUpdate.CoverImage = memoryStream.ToArray();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var response = new ReadBookDto
            {
                Id = bookToUpdate.Id,
                Title = bookToUpdate.Title,
                Author = bookToUpdate.Author,
                Description = bookToUpdate.Description,
                PublicationDate = bookToUpdate.PublicationDate,
                CoverImage = bookToUpdate.CoverImage
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(new { Message = "Livro não encontrado." });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Livro deletado com sucesso." });
        }
    }
}