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

        public void Init(string nameGame, int id)
        {
            GetListener = new HttpListener();
            GetListener.Prefixes.Add($"http://127.0.0.1:8082/{id}/");
            this.NameGame = nameGame;
            this.id = id;
            this.listObserversUser = new List<Users>();
            this.listPlayers = new List<Users>();
            WorkRoom();
        }

        private void WorkRoom()
        {
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
            string[] parser = msgContent.Split('/');
            if (parser[1] == "NewPlayers")
            {
                listPlayers.Add(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
                isStart = CheckCountlistPlayers();
                Response(Context, isStart.ToString());
                
            }
            else if (parser[1] == "RemovePlayers")
            {
                listPlayers.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }
            else if (parser[1] == "NewObserversUser")
            {
                listObserversUser.Add(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
            }
            else if (parser[1] == "RemovePlayers")
            {
                listObserversUser.Remove(Server.Server.listUser.Find(u => u.LogIn == parser[2]));
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

        public abstract bool CheckCountlistPlayers();
        public abstract Task StartGame();

    }
}
