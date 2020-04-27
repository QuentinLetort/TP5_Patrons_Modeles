using System.Collections.Generic;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using BillSDK;
using UserSDK;
using StockSDK;

namespace BillManager
{
    class BillManager
    {
        private Bill CreateBill(User user, List<ItemLine> itemLines)
        {
            List<BillLine> billLines = new List<BillLine>();
            foreach (ItemLine line in itemLines)
            {
                billLines.Add(new BillLine(line.Item, line.Quantity));
            }
            return new Bill(user, billLines);

        }
        static void Main(string[] args)
        {
            BillManager billManager = new BillManager();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "bill_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "bill_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    Bill bill = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine(" [.] Create bill: {0}", message);
                        JObject request = JObject.Parse(message);
                        User user = request["user"].ToObject<User>();
                        List<ItemLine> products = request["products"].ToObject<List<ItemLine>>();
                        bill = billManager.CreateBill(user, products);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                    }
                    finally
                    {
                        var response = JsonConvert.SerializeObject(bill);
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