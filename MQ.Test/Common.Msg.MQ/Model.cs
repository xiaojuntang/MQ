using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Msg.MQ
{
    [Serializable]
    public class UserInfo
    {
        public int UserID { get; set; }

        public string UserName { get; set; }
    }

    [Serializable]
    public class LogInfo
    {
        public int LogID { get; set; }

        public string LogName { get; set; }
    }
}
