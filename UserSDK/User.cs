using System;
using RPC;
using Newtonsoft.Json;

namespace UserSDK
{
    public class User
    {
        public string LastName { get; }
        public string FirstName { get; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public User(string lastname, string firstname, string email, string username)
        {
            this.LastName = lastname;
            this.FirstName = firstname;
            this.Email = email;
            this.UserName = username;
        }
        public static User GetUser(string username)
        {
            RPCClient rpcClient = new RPCClient();
            string response = rpcClient.Call(username, "user_queue");
            rpcClient.Close();
            return JsonConvert.DeserializeObject<User>(response);
        }
    }
}
