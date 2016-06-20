using Common.Msg.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                UserInfo u = new UserInfo();
                u.UserID = i;
                u.UserName = "姓名"+i;
                MQHelper.SendMessage(u, typeof(UserInfo), MsgTypeEnum.UserMsg);
            }

            for (int i = 0; i < 100; i++)
            {
                LogInfo u = new LogInfo();
                u.LogID = i;
                u.LogName = "姓名" + i;
                MQHelper.SendMessage(u, typeof(LogInfo), MsgTypeEnum.LogMsg);
            }
            Console.WriteLine("完毕");
            Console.ReadLine();
        }
    }


}
