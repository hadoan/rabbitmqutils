using System;

namespace MessagePublishingUtils
{
    public interface ISendMessageToCloudService 
    {
        bool SendQueuedMsgToCloud(KeyValueMessage message);
        bool SendMsgToCloud(KeyValueMessage message);
        Action<Exception, string> ErrorAction { get; set; }
        void InitConfigAndConnect(string hostName, string uesrName, string pwd, string clientName);
    }
}
