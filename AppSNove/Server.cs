
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
            listLogIn = GetAllUser(Directory.GetFiles("./User"));
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
                    SendMesage("Avtorization/true").GetAwaiter().GetResult();
                    string[] user = File.ReadAllLines($"./User/{parserUri[3]}.txt");
                    if(user[0] == parserUri[3] && user[1] == parserUri[4])
                    {
                        listUser.Add(new Users(parserUri[3], parserUri[4], wsClient));
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
        }


        private async Task SendMesage(string mesgae)
        {
            using (var messageWriterStream = wsClient.CreateMessageWriter(WebSocketMessageType.Text))
            using (var sw = new StreamWriter(messageWriterStream, Encoding.UTF8))
            {
                await sw.WriteAsync(mesgae);
            }
        }

        public static void Disckonect(string login)
        {
            listUser.Remove(listUser.Find(l => l.LogIn == login));
        }

        private List<string> GetAllUser(string[] fullArrUser)
        {
            List<string> listUser = new List<string>();
            listUser.AddRange(fullArrUser);
            for(int i = 0; i < listUser.Count; i++)
            {
                listUser[i] = listUser[i].Remove(0, listUser[i].LastIndexOf("\\") + 1).Replace(".txt", "");
            }
            return listUser;
        }
    }
}
