using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using S22.Imap;

namespace BunqPaymentsForMoneyBird
{
    public class MailHandler
    {
        private readonly string _address;
        private readonly string _password;
        private readonly int _port;
        private readonly string _server;
        private readonly bool _ssl;

        public MailHandler(string server, string address, string password, int port, bool ssl)
        {
            _server = server;
            _address = address;
            _port = port;
            _ssl = ssl;
            _password = password;
        }

        public async Task<MailMessage> GetMessage()
        {
            using (var client = new ImapClient(_server, _port, _address, _password, AuthMethod.Login, _ssl))
            {
                while (true)
                {
                    var message = client.GetMessages(client.Search(SearchCondition.Unseen())).SingleOrDefault();
                    if (message != null)
                    {
                        client.DeleteMessages(client.Search(SearchCondition.Seen()));
                        return message;
                    }

                    await Task.Delay(5000);
                }
            }
        }
    }
}