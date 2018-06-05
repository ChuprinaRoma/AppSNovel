using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSNove.Game;

namespace AppSNove.Model
{
    public class SwitchGame
    {
        public static Room GetGame(string nameGame)
        {
            Room room = null;
            switch(nameGame)
            {
                case "Tic_tac_toe": room = new Tic_tac_toe(); break;
            }
            return room;
        }
    }
}
