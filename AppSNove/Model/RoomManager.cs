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
        List<Room> listRoom = new List<Room>();
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
            parserUri = context.Request.RawUrl.ToString().Split('/', '?');
            if(parserUri[2] == "WindowGame")
            {
                room = SwitchGame.GetGame(parserUri[3]);
                room.Init(parserUri[3], GetIdRoom());
                listRoom.Add(room);
                Response(context, room.id.ToString());
                room = null;
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
    }
}
