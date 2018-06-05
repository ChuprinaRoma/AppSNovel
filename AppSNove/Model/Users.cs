using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vtortola.WebSockets;

namespace AppSNove
{
    public class Users
    {
        public string LogIn { get; set; }
        public string Password { get; set; }
        public WebSocket wsClient { get; set; }
        private bool isStateConnect { get; set; }
        public string isStateUser { get; set; }

        public Users(string LogIn, string Password, WebSocket wsClient)
        {
            isStateConnect = true;
            this.LogIn = LogIn;
            this.Password = Password;
            this.wsClient = wsClient;
            WorkUser();
        }

        private void WorkUser()
        {
            new Thread(() =>
            {
                while (wsClient.IsConnected)
                {
                    RecipientMesage().GetAwaiter().GetResult();
                }
            }).Start();
        }

        private async Task RecipientMesage()
        {
            var messageReadStream = await wsClient.ReadMessageAsync(CancellationToken.None);
            if (wsClient.IsConnected)
            {
                var msgContent = string.Empty;
                using (var sr = new StreamReader(messageReadStream, Encoding.UTF8))
                {
                    msgContent = await sr.ReadToEndAsync();
                }
            }  
            else
            {
                Server.Server.Disckonect(this.LogIn);
            }
        }

        public async Task SendMesage(string mesgae)
        {
            using (var messageWriterStream = wsClient.CreateMessageWriter(WebSocketMessageType.Text))
            using (var sw = new StreamWriter(messageWriterStream, Encoding.UTF8))
            {
                await sw.WriteAsync(mesgae);
            }
        }
    }
}
