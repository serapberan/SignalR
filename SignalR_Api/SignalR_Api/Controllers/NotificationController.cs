using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Api.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase   //Odalar oluşturuyoruz
    {
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationController(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        //postmana istek atıcaz bu adresten odalarda kaç kişi var görücez

        [HttpGet("{roomCount}")]
        public async Task<IActionResult> SetRoomCount(int roomCount)
        {
            MyHub.roomCount = roomCount; //kaç kişi olacak
            await _hubContext.Clients.All.SendAsync("Notify", $"Bu oda en fazla {roomCount} kişi olabilir.");
            return Ok();
        }
    }
}
