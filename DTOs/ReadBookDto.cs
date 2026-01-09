namespace BookStoreApi.Dtos
{
    public class ReadBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PublicationDate { get; set; }
        public byte[]? CoverImage { get; set; }
    }
}