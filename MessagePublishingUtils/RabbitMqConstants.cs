using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublishingUtils
{
    public class RabbitMqConstants
    {
        public const string DEFAULT_QUEUE = "VooyDefaultQueue";
        public const string CLIENT_TO_SERVER_QUEUE = "VooyMachine2CloudQueue";
        //public const string CLIENT_TO_SERVER_QUEUE = "VooyMachine2CloudQueue";

        public const string DEFAULT_ROUTING_KEY = "VooyDefaultRoutingKey";
        public const string EXCHANGE_M2CLOUD_NOQUEUE = "VooyMachine2CloudExchangeNoQueue";
        public const string EXCHANGE_CLOUD_TO_MACHINE_NOQUEUE = "VooyCloud2MachineExchangeNoQueue";
        public const string EXCHANGE_CLOUD_TO_MACHINE_QUEUED = "VooyCloud2MachineExchangeQueued";
        public const string EXCHANGE_M2CLOUD_QUEUED = "VooyMachine2CloudExchangeQueued";


        //Routing keys
        public const string ROUTING_KEY_MACHINES = "VooyAllMachinesRoutingKey";
        public const string ROUTING_KEY_MACHINE_ID = "VooyMachineIdRoutingKey";
        public const string ROUTING_KEY_MACHINE_TENANT = "VooyMachineTenantRoutingKey";

    }
}
