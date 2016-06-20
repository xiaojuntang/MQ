using Common.Msg.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQ.AcceptMessage
{
    class Program
    {
        public static void Main()
        {
            MQHelper.InitMQ();
            Console.ReadLine();
        }
    }
}
