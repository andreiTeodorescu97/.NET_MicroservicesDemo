// using System;
// using System.Text;
// using System.Text.Json;
// using Microsoft.Extensions.Configuration;
// using PlatformService.DTOs;
// using RabbitMQ.Client;

// namespace PlatformService.AsyncDataServices
// {
//     public class MessageBusClient : IMessageBusClient
//     {
//         private readonly IConfiguration configuration;
//         private readonly IConnection _connection;
//         private readonly IModel _channel;

//         public MessageBusClient(IConfiguration configuration)
//         {
//             this.configuration = configuration;

//             var factory = new ConnectionFactory()
//             {
//                 HostName = configuration["RabbitMqHost"],
//                 Port = int.Parse(configuration["RabbitMqPort"])
//             };
//             try
//             {
//                 _connection = factory.CreateConnection();
//                 _channel = _connection.CreateModel();
//                 _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

//                 _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
//                 Console.WriteLine("--> Rabbit MQ connection done succesfully!");

//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"--> Could not connect to Rabbit Mq {ex.Message} Inner: {ex.InnerException}");
//             }
//         }

//         public void PublishNewPlatform(PlatformPublishedDto plat)
//         {
//             var message = JsonSerializer.Serialize(plat);

//             if (_connection.IsOpen)
//             {
//                 Console.WriteLine("--> Rabbit MQ connection Open, sending message...");
//                 SendMessage(message);

//             }
//             else
//             {
//                 Console.WriteLine("--> Rabbit MQ connection Closed, NOT sending message");
//             }
//         }

//         private void SendMessage(string messageToBeSend)
//         {
//             var body = Encoding.UTF8.GetBytes(messageToBeSend);

//             _channel.BasicPublish(exchange: "trigger",
//                                     routingKey: "",
//                                     basicProperties: null,
//                                     body: body);

//             Console.WriteLine($"--> We have sent {messageToBeSend}");
//         }

//         public void Dispose()
//         {
//             Console.WriteLine("Rabbit MQ Disposed!");

//             if(_channel.IsOpen)
//             {
//                 _channel.Close();
//                 _channel.Close();
//             }
//         }

//         private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
//         {
//             Console.WriteLine("--> Rabbit MQ connection shutdown!");
//         }
//     }
// }

using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine($"--> RabbitMQ Connection Open, sending message...{message}");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connectionis closed, not sending");
            }   
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                            routingKey: "",
                            basicProperties: null,
                            body: body);
            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}