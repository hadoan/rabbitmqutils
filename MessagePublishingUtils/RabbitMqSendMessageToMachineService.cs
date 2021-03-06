﻿using System;
using MessagePack;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace MessagePublishingUtils
{
    public class RabbitMqSendMessageToMachineService : ISendMessageToMachineClientService
    {
       

        private readonly IConnectToRabbitMqMessageService _connectToRabbitMqService;
        private bool isConnectedToServer = false;
        public Action<Exception,string> ErrorAction { get; set; }

        public RabbitMqSendMessageToMachineService(IConnectToRabbitMqMessageService connectToRabbitMqService)
        {

            _connectToRabbitMqService = connectToRabbitMqService;
        }

        public void InitConfigAndConnect(string hostName, string uesrName, string pwd)
        {
            _connectToRabbitMqService.Connect(hostName, uesrName, pwd);
            isConnectedToServer = true;
        }

        public bool SendQueuedMsgToMachines(KeyValueMessage message, CloudToMachineType machineType)
        {
            if (!isConnectedToServer)
                throw new InvalidOperationException("Please call InitConfigAndConnect medthod first!");
            try
            {
                var _queuedChannel = _connectToRabbitMqService.GetQueuedModel();
                _queuedChannel.ExchangeDeclare(exchange: RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_QUEUED, type: "direct");
                var msgStr = JsonConvert.SerializeObject(message);
                var body = MessagePackSerializer.Serialize(msgStr);

                var properties = _queuedChannel.CreateBasicProperties();
                properties.Persistent = true;

                switch (machineType)
                {
                    case CloudToMachineType.AllMachines:
                        _queuedChannel.BasicPublish(exchange: RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_QUEUED,
                               routingKey: RabbitMqConstants.ROUTING_KEY_MACHINES,
                               basicProperties: properties,
                               body: body);
                        break;
                    case CloudToMachineType.CurrentTenant:
                        break;
                    case CloudToMachineType.ToMachineId:
                        _queuedChannel.BasicPublish(exchange: "",
                              routingKey: message.MachineId.ToString(),
                              basicProperties: properties,
                              body: body);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
                }

                MessageLogUtil.Log($"Send message type {machineType.ToString()} to machines: {JsonConvert.SerializeObject(message)}");
                return true;

            }
            catch (Exception e)
            {
                MessageLogUtil.Error("SendQueuedMsgToMachines", e);
                ErrorAction?.Invoke(e, "SendQueuedMsgToMachines");
                return false;
            }


        }

        public bool SendMsgToMachines(KeyValueMessage message, CloudToMachineType machineType)
        {
            if (!isConnectedToServer)
                throw new InvalidOperationException("Please call InitConfigAndConnect medthod first!");
            try
            {
                var _noQueueChannel = _connectToRabbitMqService.GetNoQueuedModel();
                _noQueueChannel.ExchangeDeclare(RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_NOQUEUE, "fanout");
                var msgStr = JsonConvert.SerializeObject(message);
                var body = MessagePackSerializer.Serialize(msgStr);

                _noQueueChannel.BasicPublish(exchange: RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_NOQUEUE,
                    routingKey: "",
                    basicProperties: null,
                    body: body);

                MessageLogUtil.Log($"Send no queue message to cloud: {JsonConvert.SerializeObject(message)}");
                return true;
            }
            catch (Exception e)
            {
                MessageLogUtil.Error("SendMsgToMachine", e);
                ErrorAction?.Invoke(e, "SendMsgToMachine");
                return false;
            }

        }

        public void Close()
        {
            _connectToRabbitMqService.Dispose();
        }

    }
}
