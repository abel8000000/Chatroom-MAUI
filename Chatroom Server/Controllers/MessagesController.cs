using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Chatroom_Server.Hubs;
using System.Threading.Tasks;

namespace Chatroom_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private static List<Message> messages = new List<Message>();
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message) 
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
            {
                return BadRequest("Invalid message data.");
            }

            messages.Add(message);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.Username, message.Content, message.Time);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetMessages()
        {
            return Ok(messages);
        }
    }

    public class Message
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
