using handy_tarifvergleich_server.models;
using handy_tarifvergleich_server.models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace handy_tarifvergleich_server.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IMongoCollection<BsonDocument> _usersCollection;

        public UserController()
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("IDPA");
            _usersCollection = database.GetCollection<BsonDocument>("users");
        }

        [HttpGet]
        [Route("me")]
        [Authorize]
        public IActionResult GetMyUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return BadRequest("Benutzer nicht gefunden");

            var filter = Builders<BsonDocument>.Filter.Eq("UserId", Convert.ToInt32(userId));
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");
            user.Remove("_id");

            return Ok(user.ToJson());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUser(int userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");
            user.Remove("_id");

            return Ok(user.ToJson());
        }

        [HttpPut]
        [Route("form/update")]
        [Authorize]
        public IActionResult UpdateUserForm(Form request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return BadRequest("Benutzer nicht gefunden");

            var filter = Builders<BsonDocument>.Filter.Eq("UserId", Convert.ToInt32(userId));
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");

            var update = Builders<BsonDocument>.Update
                .Set("Form.StrongWorldWideUsage", request.StrongWorldWideUsage)
                .Set("Form.CallminutesPerMonthCH", request.CallminutesPerMonthCH)
                .Set("Form.GBPerMonthCH", request.GBPerMonthCH)
                .Set("Form.SMSPerMonthCH", request.SMSPerMonthCH)
                .Set("Form.CallminutesPerMonthEurope", request.CallminutesPerMonthEurope)
                .Set("Form.GBPerMonthEurope", request.GBPerMonthEurope)
                .Set("Form.SMSPerMonthEurope", request.SMSPerMonthEurope);

            _usersCollection.UpdateOne(filter, update);

            return Ok("Formular erfolgreich aktualisiert");
        }

        [HttpPut]
        [Route("update")]
        [Authorize]
        public IActionResult UpdateUser(UserDto request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return BadRequest("Benutzer nicht gefunden");

            var filter = Builders<BsonDocument>.Filter.Eq("UserId", Convert.ToInt32(userId));
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");

            var update = Builders<BsonDocument>.Update
                .Set("Username", request.Username)
                .Set("PasswordHash", BCrypt.Net.BCrypt.HashPassword(request.Password));

            _usersCollection.UpdateOne(filter, update);

            return Ok("Benutzer erfolgreich aktualisiert");
        }

        [HttpDelete]
        [Route("deleteme")]
        [Authorize]
        public IActionResult DeleteUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return BadRequest("Benutzer nicht gefunden");

            var filter = Builders<BsonDocument>.Filter.Eq("UserId", Convert.ToInt32(userId));
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");

            TokenBlacklist tokenBlacklist = new TokenBlacklist { Token = user["Token"].AsString, IsBlacklisted = true };
            _usersCollection.DeleteOne(filter);

            return Ok("Benutzer erfolgreich gelöscht");
        }

        [HttpDelete]
        [Route("delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");

            TokenBlacklist tokenBlacklist = new TokenBlacklist { Token = user["Token"].AsString, IsBlacklisted = true };
            _usersCollection.DeleteOne(filter);

            return Ok("Benutzer erfolgreich gelöscht");
        }

        [HttpGet]
        [Route("isAdmin")]
        [Authorize]
        public IActionResult IsAdmin()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (userId == null) return BadRequest("Benutzer nicht gefunden");

            var filter = Builders<BsonDocument>.Filter.Eq("UserId", Convert.ToInt32(userId));
            var user = _usersCollection.Find(filter).FirstOrDefault();
            if (user == null) return BadRequest("Benutzer nicht gefunden");

            bool isAdmin = user["Role"] == "Admin";

            return Ok(isAdmin);
        }

        [HttpGet]
        [Route("isTokenValid")]
        [Authorize]
        public IActionResult IsTokenValid()
        {
            return Ok(true);
        }
    }
}
