using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ledgerly_backend.Services
{
    public class RabbitMqPublisher : IAsyncDisposable
    {
        public const string ExchangeName = "ledgerly.events";

        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqPublisher> _logger;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string routingKey, T message)
        {
            try
            {
                IChannel channel = await GetChannelAsync();

                byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);
                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);
            }
            catch (Exception ex)
            {
                // Educational fire-and-forget publisher: a broker hiccup must never break
                // the invoice CRUD operation that triggered this event.
                _logger.LogError(ex, "Failed to publish RabbitMQ event with routing key {RoutingKey}", routingKey);
            }
        }

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel is { IsOpen: true })
                return _channel;

            await _initLock.WaitAsync();
            try
            {
                if (_channel is { IsOpen: true })
                    return _channel;

                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest",
                    VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);

                return _channel;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
                await _channel.DisposeAsync();
            if (_connection is not null)
                await _connection.DisposeAsync();
        }
    }
}
