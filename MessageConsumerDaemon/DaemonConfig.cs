using System;
using System.Collections.Generic;
using System.Text;

namespace MessageConsumerDaemon
{
    internal class DaemonConfig
    {
        public string DaemonName { get; set; }
        public string AMPQHost { get; set; }

        public string AMPQUserName { get; set; }

        public string AMPQPassword { get; set; }

        public string AMPQQueueName { get; set; }
    }
}
