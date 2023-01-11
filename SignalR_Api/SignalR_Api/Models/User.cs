using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Api.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int RoomID { get; set; }
        public string Name { get; set; }
        public Room Room { get; set; }
    }
}
