using RabbitMQ.Client;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace 消息队列
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                factory.UserName = "guest";//用户名
                factory.Password = "guest";//密码
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: "Test",
                            durable: true,//队列持久化
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                            );

                        string message = GetMessage(args);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "Test",
                            basicProperties: properties,//消息持久化
                            body: body
                            );//开始传递
                        Console.WriteLine(" 已发送：{0}", message);
                    }
                }
                args = Console.ReadLine().Split("");
            }
        }

        private static string GetMessage(String[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }

    }
}
