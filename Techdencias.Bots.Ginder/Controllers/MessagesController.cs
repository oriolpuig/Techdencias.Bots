using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using System.Net;
using Techdencias.Bots.Ginder.Models;

namespace Techdencias.Bots.Ginder
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                return this.RetrieveGithubUserData(message);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message RetrieveGithubUserData([FromBody]Message message)
        {
            return CallToGithubAPI(message, (message.Text));
        }

        private Message CallToGithubAPI([FromBody]Message message, string username)
        {
            using (WebClient proxy = new WebClient())
            {
                try
                {
                    proxy.Headers.Add("user-agent", "*");
                    var response = proxy.DownloadString("https://api.github.com/users/" + username);
                    User user = new User(response); 

                    var msg = "";
                    if (user != null)
                    {
                        msg = GenerateResponse(user);
                    }

                    return message.CreateReplyMessage(msg);
                }
                catch (WebException e)
                {
                    string errorMessage = "ERROR";

                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        errorMessage += " Status Code: " + ((HttpWebResponse)e.Response).StatusCode;
                        errorMessage += " Status Description:" + ((HttpWebResponse)e.Response).StatusDescription;
                    }

                    return message.CreateReplyMessage(errorMessage);
                }
            }
        }

        private string GenerateResponse(User user) {
            var msg = string.Empty;

            if (user.avatar_url != null || user.avatar_url != string.Empty)
            {
                msg += AddImage(user);
                //msg += AddBreakLine();
            }

            if (user.name != null && user.name != string.Empty)
            {
                //msg += AddName(user); msg += AddBreakLine();
            }

            if (user.location != null && user.location != string.Empty)
            {
                //msg += AddLocation(user); msg += AddBreakLine();
            }

            return msg;
        }

        private string AddBreakLine() {
            return "---";
        }

        private string AddImage(User user)
        {
            var name = user.login;
            var avatar_url = user.avatar_url;

            return "![@" + name + " ](" + avatar_url + ")--- # H1";
        }

        private string AddName(User user)
        {
            return "Username: " + user.name;
        }

        private string AddLocation(User user) {
            return "Location: " + user.location;
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}