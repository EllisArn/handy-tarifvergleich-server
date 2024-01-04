using handy_tarifvergleich_server.models;
using handy_tarifvergleich_server.models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace handy_tarifvergleich_server.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        private readonly IMongoCollection<BsonDocument> _usersCollection;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("IDPA");
            _usersCollection = database.GetCollection<BsonDocument>("users");
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDto request)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Username", request.Username);
            var existingUser = _usersCollection.Find(filter).FirstOrDefault();
            if (existingUser != null) return BadRequest("Benutzername bereits vergeben");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            int userId = Convert.ToInt32(_usersCollection.Find(new BsonDocument()).ToList().Last()["UserId"]) + 1;

            var newUser = new User
            {
                Id = ObjectId.GenerateNewId(),
                UserId = userId,
                Username = request.Username,
                PasswordHash = hashedPassword,
                Form = new Form()
            };
            string jwtToken = GenerateJwtToken(newUser);
            newUser.Token = jwtToken;

            _usersCollection.InsertOne(newUser.ToBsonDocument());

            return Ok("Benutzer erfolgreich registriert");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDto? request)
        {
            if (request == null || request.Username.IsNullOrEmpty() || request.Password.IsNullOrEmpty())
                return BadRequest(new { message = "Benutzername oder Passwort leer" });

            var filter = Builders<BsonDocument>.Filter.Eq("Username", request.Username);
            var existingUser = _usersCollection.Find(filter).FirstOrDefault();
            if (existingUser == null) return BadRequest("Benutzername oder Passwort falsch");

            string hashedPassword = existingUser["PasswordHash"].AsString;
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword);
            if (!isPasswordCorrect) return BadRequest("Benutzername oder Passwort falsch");

            var user = BsonSerializer.Deserialize<User>(existingUser);
            string jwtToken = GenerateJwtToken(user);
            
            var update = Builders<BsonDocument>.Update.Set("Token", jwtToken);
            _usersCollection.UpdateOne(filter, update);

            return Ok("Benutzer erfolgreich eingeloggt");
        }

        private string GenerateJwtToken(User user)
        {
            var tokenBlacklist = new TokenBlacklist { Token = user.Token, IsBlacklisted = true };
            var tokenBlacklistCollection = _usersCollection.Database.GetCollection<BsonDocument>("tokenBlacklist");
            tokenBlacklistCollection.InsertOne(tokenBlacklist.ToBsonDocument());

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}