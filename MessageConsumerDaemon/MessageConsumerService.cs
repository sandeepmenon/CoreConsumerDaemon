using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageConsumerDaemon
{
    internal class MessageConsumerService: IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<DaemonConfig> _config;
        public MessageConsumerService(ILogger<MessageConsumerService> logger, IOptions<DaemonConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.Value.DaemonName);

            var factory = new ConnectionFactory()
            {
                HostName = _config.Value.AMPQHost,
                Password = _config.Value.AMPQPassword,
                UserName = _config.Value.AMPQUserName,
                RequestedConnectionTimeout = 300000,
            };

            var connection = factory.CreateConnection();
            var model = connection.CreateModel();
           
                model.BasicQos(0, 1, false);
                model.QueueDeclare(queue: _config.Value.AMPQQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += (ch, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        var messageToLog = $" [x] Received '{routingKey}':'{message}'";
                        _logger.LogInformation("Received {0}", message);
                        Console.WriteLine(message);
                        model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e.Message);
                        Console.WriteLine(e.Message);
                    }
                };
                model.BasicConsume(queue: _config.Value.AMPQQueueName,
                                 autoAck: false,
                                 consumer: consumer);

                //BasicGetResult result = model.BasicGet(queue: _config.Value.AMPQQueueName, autoAck: true);
                //if (result != null)
                //{
                //    string data =
                //    Encoding.UTF8.GetString(result.Body);
                //    Console.WriteLine(data);
                //}

            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping daemon.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");

        }
    }
}
