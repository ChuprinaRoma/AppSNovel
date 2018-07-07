
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace AppSNove.Server
{
    public class Server
    {
        WebSocketListener ws = null;
        WebSocket wsClient = null;
        public static List<Users> listUser = new List<Users>();

        public Server()
        {
            ws = new WebSocketListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
            ws.Standards.RegisterStandard(new WebSocketFactoryRfc6455());
            ws.StartAsync();
            GetNewUser();
        }

        private void GetNewUser()
        {
            new RoomManager();
            new Thread(() =>
            {
                while(true)
                {
                    WorksServer().GetAwaiter().GetResult();
                }
            }).Start();
        }
                                             
        private async Task WorksServer()
        {
            string[] parserUri = null;
            List<string> listLogIn = new List<string>();
            listLogIn = GetAllUserParser(Directory.GetDirectories("./User"));
            wsClient = await ws.AcceptWebSocketAsync(CancellationToken.None);
            parserUri = wsClient.HttpRequest.RequestUri.ToString().Split('/', '?');
            if (parserUri[2] == "Registartion")
            {
                if(listUser.Count == 0 || !listLogIn.Contains(parserUri[3]))
                {
                    SendMesage("Registartion/true").GetAwaiter().GetResult();
                    string[] user = new string[2];
                    user[0] = parserUri[3];
                    user[1] = parserUri[4];
                    File.WriteAllLines($"./User/{parserUri[3]}.txt", user);
                }
                else
                {                                                                                                    
                    SendMesage("Registartion/false").GetAwaiter().GetResult();
                    wsClient.Close();
                }
            }
            else if(parserUri[2] == "Avtorization")
            {
                if(listLogIn.Contains(parserUri[3]))
                {
                    string[] arrUser = File.ReadAllLines($"./User/{parserUri[3]}/{parserUri[3]}.txt");
                    if(arrUser[0] == parserUri[3] && arrUser[1] == parserUri[4])
                    {
                        listUser.Add(new Users(parserUri[3], parserUri[4], wsClient));
                        SendMesage("Avtorization/true").GetAwaiter().GetResult();
                        SendMesageAllClient(parserUri[3]);
                    }
                    else
                    {
                        SendMesage("Avtorization/false").GetAwaiter().GetResult();
                        wsClient.Close();
                    }
                }
                else
                {
                    SendMesage("Avtorization/false").GetAwaiter().GetResult();
                    wsClient.Close();
                }
            }
            else
            {
                wsClient.Close();
            }
            wsClient = null;
        }

        private void SendMesageAllClient(string logIn)
        {
            List<string> allGame = new List<string>();
            allGame = GetAllUserParser(Directory.GetDirectories($"./User/{logIn}/Game"));


            for (int user = 0; user < listUser.Count; user++)
            {
                listUser[user].isStateUser = "не играет";
               
                if (listUser[user].LogIn != logIn)
                { 
                    SendMesage($"UpdateUser/{listUser[user].LogIn}/{listUser[user].isStateUser}").GetAwaiter().GetResult();
                    listUser[user].SendMesage($"UpdateUser/{logIn}/не играет").GetAwaiter().GetResult();
                }
            }
            
            for(int game = 0; game < allGame.Count; game++)
            {
                SendMesage($"UpdateGame/{allGame[game]}/Настольная").GetAwaiter().GetResult();
            }

            for (int room = 0; room < RoomManager.listRoom.Count; room++)
            {
                if (RoomManager.listRoom[room].TypeRoom == "public")
                {
                    SendMesage($"NewRoom/{RoomManager.listRoom[room].id}/{RoomManager.listRoom[room].NameGame}").GetAwaiter().GetResult();
                }
            }
        }

        private async Task SendMesage(string mesgae)
        {
            using (var messageWriterStream = wsClient.CreateMessageWriter(WebSocketMessageType.Text))
            using (var sw = new StreamWriter(messageWriterStream, Encoding.UTF8))
            {
                await sw.WriteAsync(mesgae);
            }
        }

        public static void Disckonect(string logIn)
        {
            listUser.Remove(listUser.Find(l => l.LogIn == logIn));
            for (int user = 0; user < listUser.Count; user++)
            {
                listUser[user].SendMesage($"RemoveUser/{logIn}").GetAwaiter().GetResult();
            }
        }

        private List<string> GetAllUserParser(string[] fullArrUser)
        {
            List<string> listUser1 = new List<string>();
            listUser1.AddRange(fullArrUser);
            for(int i = 0; i < listUser1.Count; i++)
            {
                listUser1[i] = listUser1[i].Remove(0, listUser1[i].LastIndexOf("\\") + 1);
            }
            return listUser1;
        }
    }
}
