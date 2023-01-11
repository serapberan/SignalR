using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Api.Hubs
{
    public class MyHub : Hub
    {
        private readonly Context _context;

        public MyHub(Context context)
        {
            _context = context;
        }

        public static List<string> Names { get; set; } = new List<string>();  //kişileri listesi

        public static int ClientCount { get; set; } = 0; //başlangıçtaki kullanıcı sayısı
        public static int roomCount { get; set; } = 7; //odada ki maximum  kullanıcı sayısı

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names); //tüm isimleri getir
        }

        public async override Task OnConnectedAsync()  //kullanıcı arttır
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        }

        public async override Task OnDisconnectedAsync(Exception exception) //kullanıcı azalt
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        }

        public async Task SendName(string name)
        {
            if(Names.Count >= roomCount)
            {
                await Clients.Caller.SendAsync("Error", $"Bu oda en fazla {roomCount} kişi kadar üye alabilir.");
            }
            else
            {
                Names.Add(name);  //parametreden gelen ismi 
                await Clients.All.SendAsync("ReceiveName", name);
            }
        }


        public async Task SendNameByGroup(string name, string roomName) //oda ve kişileri ayıralım
        {
            var room = _context.Rooms.Where(x => x.RoomName == roomName).FirstOrDefault();
            if (room != null)
            {
                room.Users.Add(new User
                {
                    Name = name
                });
            }
            else
            {
                var newRoom = new Room
                {
                    RoomName = roomName
                };
                newRoom.Users.Add(new User { Name = name });
                _context.Rooms.Add(newRoom);
            }
            await _context.SaveChangesAsync();
            await Clients.Group(roomName).SendAsync("ReceiveMessageByGroup", name, room.RoomID);
        }

        public async Task GetNamesByGroup() //Odalara göre isimleri getir
        {
            var rooms = _context.Rooms.Include(x => x.Users).Select(y => new
            {
                roomId = y.RoomID,
                Users = y.Users.ToList()
            });
            await Clients.All.SendAsync("ReceiveNamesByGroup", rooms);
        }

        // 1 kişi 1 oda da olabilir mantığından ötürü
        public async  Task AddToGroup(string roomName)  //Seçilen grouba (odaya göre ekle) 
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task RemoveToGroup(string roomName)   //Seçilen grouba (odaya göre çıkar) 
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName); 
        }

    }
}
