using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using vtortola.WebSockets;

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
            PlayerX = listPlayers[0].LogIn;
            PlayerO = listPlayers[1].LogIn;
        }
       
    }
}
