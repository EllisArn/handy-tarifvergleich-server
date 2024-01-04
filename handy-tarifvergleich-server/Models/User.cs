using MongoDB.Bson;

namespace handy_tarifvergleich_server.models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public int UserId { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public Form Form { get; set; } = new Form();
    }
}
