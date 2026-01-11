using BookStoreApi.Dtos;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadBookDto>>> GetBooks()
        {
            var books = await _service.GetAllAsync();
            if (!books.Any())
            {
                return NotFound(new { Message = "Nenhum livro encontrado." });
            }
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadBookDto>> GetBook(int id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null) return NotFound(new { Message = "Livro não encontrado." });
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<ReadBookDto>> PostBook([FromForm] CreateBookDto bookDto)
        {
            var createdBook = await _service.CreateAsync(bookDto);
            
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReadBookDto>> PutBook(int id, [FromForm] UpdateBookDto bookDto)
        {
            var updatedBook = await _service.UpdateAsync(id, bookDto);
            
            if (updatedBook == null) 
                return NotFound(new { Message = "Livro não encontrado para atualização." });

            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _service.DeleteAsync(id);
            
            if (!success) 
                return NotFound(new { Message = "Livro não encontrado." });

            return Ok(new { Message = "Livro deletado com sucesso." });
        }
    }
}