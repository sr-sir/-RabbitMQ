using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace 消息队列Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.UserName = "guest";
            factory.Password = "guest";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Test",
                        durable: true,//持久化队列
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);


                    channel.BasicConsume(
                        queue: "Test",
                        autoAck: false,
                        consumer: consumer
                        );
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);
                        Console.WriteLine("已接收：{0}", message);
                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
