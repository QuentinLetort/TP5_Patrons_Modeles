using System.Collections.Generic;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserSDK;
using System.Text;
using Newtonsoft.Json;

namespace UserManager
{
    class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            users = new List<User>();
            users.Add(new User("ln", "fn", "em", "un"));
            users.Add(new User("aa", "bb", "cc", "dd"));
        }
        public void LoadUsers(string filename)
        {
            // TODO: Load users from jsonfile
        }
        public User GetUser(string username)
        {
            return users.Find((user) => user.UserName == username);
        }

        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "user_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "user_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    User user = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine(" [.] Search: {0}", message);
                        user = userManager.GetUser(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                    }
                    finally
                    {
                        var response = JsonConvert.SerializeObject(user);
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
