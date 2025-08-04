using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Toxiad.Common.Logger;

namespace Toxiad.Common.Logger
{
    public class Logger : ILogger
    {
        string logPath = "";
        string logName = "";
        string fullLogPath = "";
        string fullLogName = "";
        string logNamePt = "";
        DirectoryInfo logDirectory;
        DateTime CreateAt;
        public Logger()  
        {
            logPath = "Log";
            logNamePt = "Log_{Time}.log";
            CreateAt = DateTime.Today;
            fullLogPath = Path.GetFullPath(logPath);
            Directory.CreateDirectory(fullLogPath);
            logDirectory = new DirectoryInfo(fullLogPath);
            logName = logNamePt.Replace("{Time}", $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fff}");
            fullLogName = Path.Combine(logDirectory.FullName, logName);
            OnLog += LogAutoProcess;
        }
        public event EventHandler<LogEventArgs> OnLog;
        public void LogAutoProcess(object sender, LogEventArgs args)
        {
             
        }
        /// <summary>
        /// Send a error message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        public string Error(string message, Exception exception = null, string key = "{Guid}", string title = "错误", bool canUserRelease = false)
        {
            key = key.Replace("{Guid}", Guid.NewGuid().ToString());
            WriteFile(message, LogType.Error, key, exception);
            OnLog(this, new LogEventArgs 
            { 
                Key = key,
                Title = title,
                Message = message, 
                Type = LogType.Error,
                Exception = exception,
                CanUserRelease = canUserRelease
            });
            return key;
        }

        public void FileAutoClear(DateTime before)
        {
            Log($"Logger::AutoClear Start[before={before:yyyy-MM-dd_HH-mm-ss}];");
            logDirectory.GetFiles("Log*.log", SearchOption.AllDirectories).ToList().ForEach(file =>
            {
                if (file.LastWriteTime < before)
                {
                    try
                    {
                        file.Delete();
                        Log($"Logger::AutoClear name={file.FullName};");
                    }
                    catch (Exception ex)
                    {
                        Warn($"在{before:yyyy-MM-dd HH:mm:ss}]前，文件{file.FullName}", ex, "ToxiadLogAutoClearError", "日志自动清理失败");
                    }
                }
            });
        }

        /// <summary>
        /// Send a info message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        public string Info(string message, Exception exception = null, string key = "{Guid}", string title = "信息", bool canUserRelease = true)
        {
            key = key.Replace("{Guid}", Guid.NewGuid().ToString());
            WriteFile(message, LogType.Info, key, exception);
            OnLog(this, new LogEventArgs
            {
                Key = key,
                Title = title,
                Message = message,
                Type = LogType.Info,
                Exception = exception,
                CanUserRelease = canUserRelease
            });
            return key;
        }
        /// <summary>
        /// 此方法不会触发OnLog事件
        /// </summary>
        /// <param name="message">信息</param>
        public void Log(string message, LogType type = LogType.Log, Exception exception = null)
        {
            WriteFile(message, type, "ToxiadLogNormal", exception);
        }

        public void Release(string key)
        {
            OnLog(this, new LogEventArgs
            {
                Key = key,
                Message = "ReleaseLogHold",
                Type = LogType.Release
            });
        }

        /// <summary>
        /// Send a warning message, return key;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="key"></param>
        /// <param name="title"></param>
        /// <param name="canUserRelease"></param>
        /// <returns></returns>
        public string Warn(string message, Exception exception = null, string key = "{Guid}", string title = "警告", bool canUserRelease = true)
        {
            key = key.Replace("{Guid}", Guid.NewGuid().ToString());
            WriteFile(message, LogType.Warning, key, exception);
            OnLog(this, new LogEventArgs
            {
                Key = key,
                Title = title,
                Message = message,
                Type = LogType.Warning,
                Exception = exception,
                CanUserRelease = canUserRelease
            });
            return key;
        }
        private void WriteFile(string message, LogType type, string key, Exception exception = null)
        {
            if (CreateAt != DateTime.Today)
            {
                CreateAt = DateTime.Today;
                fullLogPath = Path.GetFullPath(logPath);
                logDirectory = Directory.CreateDirectory(fullLogPath);
                logName = logNamePt.Replace("{Time}", $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fff}");
                fullLogName = Path.Combine(logDirectory.FullName, logName);
            }
            try
            {
                File.AppendAllText(fullLogName, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}[{type}]{message};{key};{exception?.ToString()}\n");
            }
            catch
            {
            }
        }
    }
}
