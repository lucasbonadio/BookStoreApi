namespace BookStoreApi.Dtos
{
    public class UpdateBookDto
    { 
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public DateTime? PublicationDate { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}