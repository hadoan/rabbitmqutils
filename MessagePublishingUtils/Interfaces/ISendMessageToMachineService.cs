﻿using System;

namespace MessagePublishingUtils
{
    public interface ISendMessageToMachineClientService 
    {
        bool SendQueuedMsgToMachines(KeyValueMessage message, CloudToMachineType machineType);
        bool SendMsgToMachines(KeyValueMessage message, CloudToMachineType machineType);
        Action<Exception,string> ErrorAction { get; set; }
        void InitConfigAndConnect(string hostName, string uesrName, string pwd);
        void Close();
    }
}
