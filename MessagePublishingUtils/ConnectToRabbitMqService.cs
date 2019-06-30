using System;
using System.Timers;
using RabbitMQ.Client;

namespace MessagePublishingUtils
{
    public class ConnectToRabbitMqService : IConnectToRabbitMqMessageService
    {
        private IConnection _connection;
        private IModel _queuedChannel;
        private IModel _noQueueChannel;
        //private ILogger _logger;

        //private readonly IConfigurationRoot _configurationRoot;
        //private readonly IDetailLogService _detailLogService;
        private string _hostName;
        private string _userName;
        private string _pwd;
        private readonly Timer _checkConnectTimer;


        public Action<Exception> ErrorAction { get; set; }
        public ConnectToRabbitMqService()
        {
            //Connect();
            _checkConnectTimer = new Timer(60000);//timer run every minutes to check connection
            _checkConnectTimer.Elapsed += CheckConnectTimerElapsed;
            _checkConnectTimer.Enabled = true;
            _checkConnectTimer.Start();

        }

        public void Connect(string hostName,string userName,string pwd,string clientName="KonbiCloud")
        {
            try
            {
                _hostName = hostName;
                _userName = userName;
                _pwd = pwd;

                MessageLogUtil.Log($"Connecting to RabbitMQ {hostName}");
                var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = pwd };

                _connection = factory.CreateConnection(clientName);
                _queuedChannel = _connection.CreateModel();
                _noQueueChannel = _connection.CreateModel();

            }
            catch (Exception e)
            {
                MessageLogUtil.Error("InitRabbitMqConnection", e);
                ErrorAction?.Invoke(e);
            }

        }

        public IModel GetQueuedModel()
        {
            return _queuedChannel;
        }

        public IModel GetNoQueuedModel()
        {
            return _noQueueChannel;
        }

        public IConnection GetConnection()
        {
            return _connection;
        }


        private void CheckConnectTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                MessageLogUtil.Log("RabbitMQ connection closed, try to reopen...");
                if (_queuedChannel!= null && _queuedChannel.IsOpen) _queuedChannel.Close();

                Connect(_hostName,_userName,_pwd);
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _queuedChannel?.Dispose();
            _noQueueChannel?.Dispose();
            _checkConnectTimer?.Dispose();
        }

    }
}
