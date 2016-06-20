using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Common.Msg.MQ
{
    /// <summary>
    /// MSMQ消息队列
    /// 公共队列
    ///MachineName\QueueName
    ///专用队列
    ///MachineName\Private$\QueueName
    ///日志队列
    ///MachineName\QueueName\Journal$
    /// </summary>
    public class MQHelper : IDisposable
    {
        private static MessageQueue _queue = null;
        private static string _path = string.Empty;
        public static bool _isLoacl = false;

        static MQHelper()
        {
            if (_queue == null)
            {
                if (_isLoacl)
                {
                    _path = @".\private$\TXJMQ";
                    _queue = MessageQueue.Exists(_path) ? new MessageQueue(_path) : MessageQueue.Create(_path, false);
                }
                else
                {
                    //注意事项
                    //1、服务器上消息队列权限设置：给ANONYMOUS LOGON赋予所有权限
                    //2、修改服务器的注册表，允许非验证客户端访问
                    //   注册表新增HKLM\Software\Microsoft\MSMQ\Parameters\security\AllowNonauthenticatedRpc项，设置其DWORD值为1
                    //   注册表新增HKLM\Software\Microsoft\MSMQ\Parameters\security\NewRemoteReadServerDenyWorkgroupClient项，设置其DWORD值为1
                    string MQServerIP = "10.1.2.102";
                    _path = "FormatName:DIRECT=TCP:" + MQServerIP + "\\private$\\TXJMQ";
                    _queue = new MessageQueue(_path);
                    //_queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                }
            }
        }

        /// <summary>
        /// 初始化消息接收
        /// </summary>
        public static void InitMQ()
        {
            _queue.ReceiveCompleted += _queue_ReceiveCompleted;
            _queue.BeginReceive();
        }

        /// <summary>
        /// 异步接收消息队列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                MessageQueue mq = sender as MessageQueue;
                if (null != mq)
                {
                    Message msg = mq.EndReceive(e.AsyncResult);
                    if (msg.Label == MsgTypeEnum.UserMsg.ToString())
                    {
                        msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(UserInfo) });
                        UserInfo u = (UserInfo)msg.Body;
                        Console.WriteLine("\r\n用户：" + u.UserName);
                    }
                    else if (msg.Label == MsgTypeEnum.LogMsg.ToString())
                    {
                        msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(LogInfo) });
                        LogInfo obj = (LogInfo)msg.Body;
                        Console.WriteLine("\r\n日志：" + obj.LogName);
                    }
                    _queue.BeginReceive();//接收下一次事件
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgBody"></param>
        /// <param name="objType"></param>
        /// <param name="msgType"></param>
        /// <returns></returns>
        public static bool SendMessage(object msgBody, Type objType, MsgTypeEnum msgType)
        {
            try
            {
                _queue.Formatter = new XmlMessageFormatter(new Type[] { objType });
                Message message = new Message();
                //message.Recoverable = true;
                message.Label = msgType.ToString();//用描述设置ID
                message.Body = msgBody;//消息主体
                message.ResponseQueue = _queue; //将消息加入到发送队列
                //message.AttachSenderId = true;
                message.Formatter = new XmlMessageFormatter(new Type[] { objType });
                _queue.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 同步接收消息
        /// </summary>
        /// <returns></returns>
        private static object Receive()
        {
            Message msg = null;
            try
            {
                //同步接收，如果没有消息，会等待
                msg = _queue.Receive();
                //msg = _queue.Peek();  接受但不删除消息
                if (msg.Label == MsgTypeEnum.UserMsg.ToString())
                {
                    msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(UserInfo) });
                }
                else if (msg.Label == MsgTypeEnum.LogMsg.ToString())
                {
                    msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(LogInfo) });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\n002处理消息失败11111111111" + ex.Message);
            }
            return msg;
        }

        public void Dispose()
        {
            _queue.Dispose();
            _queue.Close();
        }
    }

    public enum MsgTypeEnum
    {
        UserMsg,
        LogMsg
    } 
}
