using System.ComponentModel.DataAnnotations;
namespace BookStoreApp.API.Models.Dtos.Author
{
    public class CreateAuthorDTO
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(250)]
        public string Bio { get; set; }
    }
}
