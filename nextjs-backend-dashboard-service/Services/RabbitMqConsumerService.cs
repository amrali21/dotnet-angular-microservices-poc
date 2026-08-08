using System.Text;
using System.Text.Json;
using nextjs_backend_dashboard_service.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace nextjs_backend_dashboard_service.Services
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private const string ExchangeName = "ledgerly.events";
        private const string QueueName = "dashboard.kpi.queue";

        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumerService(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumerService> logger)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
            };

            while (true)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                    break;
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "RabbitMQ not reachable yet, retrying in 5s");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(QueueName, ExchangeName, "invoice.*", cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(QueueName, ExchangeName, "customer.*", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                string json = Encoding.UTF8.GetString(args.Body.ToArray());

                using IServiceScope scope = _scopeFactory.CreateScope();
                KpiService kpiService = scope.ServiceProvider.GetRequiredService<KpiService>();

                switch (args.RoutingKey)
                {
                    case "invoice.created":
                        await kpiService.ApplyInvoiceCreatedAsync(JsonSerializer.Deserialize<InvoiceCreatedEvent>(json)!);
                        break;
                    case "invoice.updated":
                        await kpiService.ApplyInvoiceUpdatedAsync(JsonSerializer.Deserialize<InvoiceUpdatedEvent>(json)!);
                        break;
                    case "invoice.deleted":
                        await kpiService.ApplyInvoiceDeletedAsync(JsonSerializer.Deserialize<InvoiceDeletedEvent>(json)!);
                        break;
                    case "customer.created":
                        await kpiService.ApplyCustomerCreatedAsync(JsonSerializer.Deserialize<CustomerCreatedEvent>(json)!);
                        break;
                    case "customer.deleted":
                        await kpiService.ApplyCustomerDeletedAsync(JsonSerializer.Deserialize<CustomerDeletedEvent>(json)!);
                        break;
                    default:
                        _logger.LogWarning("Unhandled routing key {RoutingKey}", args.RoutingKey);
                        break;
                }

                await _channel!.BasicAckAsync(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process RabbitMQ message with routing key {RoutingKey}", args.RoutingKey);
                await _channel!.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
                await _channel.CloseAsync(cancellationToken);
            if (_connection is not null)
                await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
