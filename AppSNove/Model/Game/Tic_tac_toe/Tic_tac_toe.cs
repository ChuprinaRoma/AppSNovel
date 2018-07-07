using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AppSNove.Game
{
    public class Tic_tac_toe : Room
    {
        public override string NameGame { get; set; }
        public override int id { get; set; }
        public override List<Users> listObserversUser { get; set; }
        public override List<Users> listPlayers { get; set; }
        public string PlayerX { get; set; }
        public string PlayerO { get; set; }
        public override HttpListener GetListener { get; set; }
        public override bool isGame { get; set; }
        public override string TypeRoom { get; set; }
        public override string Key { get; set; }
        public override string TypeGame { get; set; }

        private string[,] field = null;
        private HttpListener gamelistene = null;
        private Thread thread = null;

        public override  bool CheckCountlistPlayers()
        {
            if (listPlayers.Count == 2)
            {
                StartGame().GetAwaiter().GetResult();
                return true;
            }
            return false;
        }

        public override async Task StartGame()
        {
            gamelistene = new HttpListener();
            gamelistene.Prefixes.Add($"http://127.0.0.1:8081/{NameGame}/{id}");
            PlayerX = listPlayers[0].LogIn;
            PlayerO = listPlayers[1].LogIn;
            field = new string[3, 3]
            {
                { "--", "--", "--" },
                { "--", "--", "--" },
                { "--", "--", "--" }
            };
            gamelistene.Start();
            thread = new Thread(WorkGame);
            thread.Start();
        }
        private void WorkGame()
        {
            while (isGame)
            {
                var context = gamelistene.GetContext();
                string[] parser = context.Request.RawUrl.Split('/', '?');
            }
        }
    }
}
