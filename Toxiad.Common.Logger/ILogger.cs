using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiad.Common.Logger
{
    public enum LogType : byte
    {
        Error = 0,
        Release = 1, 
        Warning = 2,
        Info = 4,
        Log = 7,
    }
    public class LogEventArgs : EventArgs
    {
        public LogType Type;
        public string Key;
        public string Title;
        public string Message;
        public bool CanUserRelease;
        public Exception Exception;
    }
    public interface ILogger
    {
        event EventHandler<LogEventArgs> OnLog;
        void FileAutoClear(DateTime before);
        void Release(string key = "{Guid}");
        /// <summary>
        /// 向日志文件记录一项信息
        /// 此方法不会触发OnLog事件
        /// </summary>
        /// <param name="message"></param>
        void Log(string message, LogType type = LogType.Log, Exception exception = null);
        /// <summary>
        /// Send a warn message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        string Warn(string message, Exception exception = null, string key = "{Guid}", string title = "警告", bool canUserRelease = true);
        /// <summary>
        /// Send a error message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        string Error(string message, Exception exception = null, string key = "{Guid}", string title = "错误", bool canUserRelease = false);
        /// <summary>
        /// Send a info message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        string Info(string message, Exception exception = null, string key = "{Guid}", string title = "信息", bool canUserRelease = true);
    }
}
