namespace BookStoreApp.API.Models.Dtos.User
{
    public class AuthResponse
    {
        public string userID { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
