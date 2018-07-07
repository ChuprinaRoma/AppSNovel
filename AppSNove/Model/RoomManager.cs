using AppSNove.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace AppSNove
{
    public class RoomManager
    {
        HttpListener listener;
        public static List<Room> listRoom = new List<Room>();
        public RoomManager()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8081/");
            listener.Start();
            Start();
        }


        private void Start()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Work().GetAwaiter().GetResult();
                }
            }).Start();
        }

        private async Task Work()
        {
            Room room = null;
            string[] parserUri = null;
            var context = await listener.GetContextAsync();
            parserUri = context.Request.RawUrl.ToString().Split('/');
            room = SwitchGame.GetGame(parserUri[1]);
            if (parserUri[2] == "AddXeshInRoom")
            {
                listRoom.Find(r => r.Key == parserUri[3]).listObserversUser.Add(Server.Server.listUser.Find(u => u.LogIn == parserUri[3]));
            }
            else if (room.TypeGame == "WindowGame")
            {
                
                room.Init(parserUri[1], GetIdRoom(), parserUri[2]);
                room.listObserversUser.Add(Server.Server.listUser.Find(u => u.LogIn == parserUri[3]));
                room.Key = GetKeyRoom();
                listRoom.Add(room);
                Response(context, $"{room.NameGame}/{room.id.ToString()}/{room.Key}/{room.TypeRoom}");
                if (room.TypeRoom == "public")
                {
                    PushAllRoom(room).GetAwaiter().GetResult();
                }
                room = null;
            }
             
        }
        //После создания комнаты, следует отправка комнат всем клиентам
        private async Task PushAllRoom(Room room)
        {
            for(int i = 0; i < Server.Server.listUser.Count; i++)
            {
                Server.Server.listUser[i].SendMesage($"NewRoom/{room.NameGame}/{room.id}").GetAwaiter().GetResult();
            }
        }

        private void Response(HttpListenerContext context, string responseStr)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
            context.Response.ContentLength64 = buffer.Length;
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private int GetIdRoom()
        {
            int id = 0;
            Random rn = new Random();
            do
            {
                id = rn.Next(1, 2147483647);
            }
            while (listRoom.Contains(listRoom.Find(r => r.id == id)));
            return id;
        }

        private string GetKeyRoom()
        {
            Random rn = new Random();
            string key = "";
            for (int i = 0; i < 10; i++)
            {
                
                int temp = rn.Next(0, 15);
                if(temp > 9)
                {
                    key += Gethexadecimal(temp);
                }
                else if(temp < 10)
                {
                    key += temp.ToString();
                }
            }
            return key;
        }
        
        private string Gethexadecimal(int numb)
        {
            string tempStr = "";
            switch(numb)
            {
                case 10: tempStr = "a"; break;
                case 11: tempStr = "b"; break;
                case 12: tempStr = "c"; break;
                case 13: tempStr = "d"; break;
                case 14: tempStr = "e"; break;
                case 15: tempStr = "f"; break;
            }
            return tempStr;
        }
    }
}
