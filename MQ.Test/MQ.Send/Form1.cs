using Common.Msg.MQ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQ.Send
{
    public partial class Form1 : Form
    {
        private int index;
        public Form1()
        {
            InitializeComponent();     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserInfo u = new UserInfo();
            u.UserID = index++;
            u.UserName = "姓名" + textBox1.Text + index;
            MQHelper.SendMessage(u, typeof(UserInfo), MsgTypeEnum.UserMsg);



            LogInfo l = new LogInfo();
            l.LogID = index;
            l.LogName = "日志" + textBox1.Text + index;
            MQHelper.SendMessage(l, typeof(LogInfo), MsgTypeEnum.LogMsg);
        }
    }
}
