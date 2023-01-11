using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Api.Models
{
    public class Room
    {
        public Room()
        {
            Users = new List<User>();
        }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public List<User> Users { get; set; }
    }
}
