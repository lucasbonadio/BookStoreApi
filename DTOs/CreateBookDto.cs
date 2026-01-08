using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Dtos
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título não pode ter mais de 100 caracteres")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "O autor é obrigatório")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de publicação é obrigatória")]
        public DateTime PublicationDate { get; set; }

        [Required(ErrorMessage = "A imagem de capa é obrigatória")]
        public IFormFile CoverImage { get; set; } = null!;
    }
}