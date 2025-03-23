using Microsoft.AspNetCore.Mvc;

namespace Chatroom_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> _connectedUsers = new List<User>();

        [HttpPost("connect")]
        public IActionResult Connect([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username))
            {
                return BadRequest("Invalid user data.");
            }

            if (_connectedUsers.Any(u => u.Username == user.Username))
            {
                return Conflict("Username already exists.");
            }

            _connectedUsers.Add(user);
            return Ok();
        }

        [HttpPost("disconnect")]
        public IActionResult Disconnect([FromBody] User user)
        {
            var existingUser = _connectedUsers.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                _connectedUsers.Remove(existingUser);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_connectedUsers);
        }
    }

    public class User
    {
        public string Username { get; set; }
    }
}
