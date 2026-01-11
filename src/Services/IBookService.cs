using BookStoreApi.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<ReadBookDto>> GetAllAsync();
        Task<ReadBookDto?> GetByIdAsync(int id);
        Task<ReadBookDto> CreateAsync(CreateBookDto createDto);
        Task<ReadBookDto?> UpdateAsync(int id, UpdateBookDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}