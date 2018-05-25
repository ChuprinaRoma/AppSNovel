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
        private bool isState { get; set; }

        public Users(string LogIn, string Password, WebSocket wsClient)
        {
            isState = true;
            this.LogIn = LogIn;
            this.Password = Password;
            this.wsClient = wsClient;
            WorkUser();
        }

        private void WorkUser()
        {
            new Thread(() =>
            {
                while (isState)
                {
                    RecipientMesage().GetAwaiter().GetResult();
                }
            }).Start();
        }
        private async Task RecipientMesage()
        {
            var messageReadStream = await wsClient.ReadMessageAsync(CancellationToken.None);
            if (messageReadStream != null)
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
                isState = false;
            }
        }
    }
}
