using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Dtos.Book
{
    public class CreateBookDTO
    {
        [Required]
        [StringLength(50)]
        public string? Title { get; set; }
        [Required]
        [Range(1800, int.MaxValue)]
        public int? Year { get; set; }
        [Required]
        public string? Isbn { get; set; }
        [Required]
        [StringLength(250, MinimumLength =10)]
        public string? Summary { get; set; }
        public string? Image { get; set; }
        [Required]
        public decimal? Price { get; set; }
    }
}
