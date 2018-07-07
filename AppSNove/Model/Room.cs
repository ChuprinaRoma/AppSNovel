using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace AppSNove
{
    public abstract class Room
    {
        public abstract string NameGame { get; set; }
        public abstract int id { get; set; }
        public abstract List<Users> listObserversUser { get; set; }
        public abstract List<Users> listPlayers { get; set; }
        public abstract HttpListener GetListener { get; set; }
        public abstract bool isGame { get; set; }
        public abstract string TypeRoom { get; set; }
        public abstract string TypeGame { get; set; }
        public abstract string Key { get; set; }


        public void Init(string nameGame, int id, string TypeRoom)
        {
            GetListener = new HttpListener();
            GetListener.Prefixes.Add($"http://127.0.0.1:8081/{id}/");
            this.NameGame = nameGame;
            this.id = id;
            this.TypeRoom = TypeRoom;
            this.listObserversUser = new List<Users>();
            this.listPlayers = new List<Users>();
            WorkRoom();
        }

        private void WorkRoom()
        {
            GetListener.Start();
            new Thread(() =>
            {
                RecipientMesage();

            }).Start();
        }

        private  void RecipientMesage()
        {
            bool isStart = false;
            var Context = GetListener.GetContext();
            var msgContent = Context.Request.RawUrl;
            string[] parser = msgContent.Split('/', '?');
            if (parser[2] == "NewPlayers")
            {
                listObserversUser.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
                listPlayers.Add(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
                isStart = CheckCountlistPlayers();
                Response(Context, $"{isGame.ToString()}/{listPlayers[0].LogIn}").GetAwaiter().GetResult();
            }
            else if (parser[2] == "RemovePlayersFull")
            {
                listPlayers.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }
            else if (parser[2] == "RemovePlayers")
            {
                listPlayers.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
                listObserversUser.Add(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }
            else if (parser[2] == "NewObserversUser")
            {
                listObserversUser.Add(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }
            else if (parser[2] == "RemoveObserversPlayers")
            {
                listObserversUser.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }

        }


        protected async Task Response(HttpListenerContext context, string responseStr)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
            context.Response.ContentLength64 = buffer.Length;
            Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public abstract bool CheckCountlistPlayers();
        public abstract Task StartGame();

    }
}
