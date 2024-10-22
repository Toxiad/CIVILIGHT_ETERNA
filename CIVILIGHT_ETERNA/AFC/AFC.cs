using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toxiad.Common.Logger;
using System.Diagnostics;
using Stylet;
using StyletIoC;

namespace CIVILIGHT_ETERNA
{
    public enum LoaderEventType
    {
        Progress,
        MessageBox,
        ProgressRemove
    }
    public class AFCEventArgs
    {
        public Guid ProcessId;
        public LoaderEventType EventType;
        public string Desc;
        public double value;
        public bool IsSuccess;
    }

    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class AFC
    {
        [Inject] private IEventAggregator eventAggregator;
        //public event EventHandler<AFCEventArgs> OnProcess; 
        private void OnProcess(object sender, AFCEventArgs args)
        {
            eventAggregator.Publish(args);
        }
        public Guid ProcessId { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = "就绪";
        public double Progress { get; set; } = 0;
        public void Set(string source, string target, ILogger logger, int delay = 0, bool autoDelete = false)
        {
            Source = source;
            delOrg = autoDelete;
            Output = target;
            Delay = delay;
            Logger = logger;
        }
        public string Source;
        public string Output;
        int Delay;
        bool delOrg;
        bool IsStop;
        bool isStop;
        ILogger Logger;
        public async Task Start()
        {
            isStop = false;
            IsStop = false;
            var subDirList = new DirectoryInfo(Source).GetDirectories();
            var execList = new List<DirectoryInfo>();
            var duedate = DateTime.Now.AddDays(-(double)Delay);
            var lastWritedateM1 = DateTime.Today.AddDays(-1).AddHours(4);
            var root = Path.GetPathRoot(Path.GetFullPath(Output));
            try
            {
                DriveInfo.GetDrives().ToList().ForEach(i =>
                {
                    if (i.Name == root && i.TotalFreeSpace < 1 * 1024 * 1024 * 1024L)
                    {
                        IsStop = true;
                        isStop = true;
                        OnProcess(this, new AFCEventArgs
                        {
                            EventType = LoaderEventType.ProgressRemove,
                            IsSuccess = false,
                            Desc = "磁盘空间小于1GB",
                            value = -1
                        });
                        Description = "磁盘空间小于1GB";
                        throw new Exception("磁盘空间小于1GB");
                    }
                });
                OnProcess(this, new AFCEventArgs
                {
                    EventType = LoaderEventType.Progress,
                    IsSuccess = false,
                    Desc = "查找目录",
                    value = -1
                });
                Description = "查找目录";
                Logger.Log("AFC Start Task");
                Logger.Log($"AFC Output {Output}");
                foreach (var subDir in subDirList)
                {
                    if (subDir.CreationTime < duedate && subDir.LastWriteTime < lastWritedateM1)
                    {
                        execList.Add(subDir);
                    }
                }
                Logger.Log($"AFC ExecList Cnt {execList.Count}");
                Logger.Log($"AFC ExecList Before {duedate:yyyy-MM-dd_HH-mm-ss_fff} And {lastWritedateM1:yyyy-MM-dd_HH-mm-ss_fff}");
                if (IsStop)
                {
                    isStop = true;
                    return;
                }
                OnProcess(this, new AFCEventArgs
                {
                    EventType = LoaderEventType.Progress,
                    IsSuccess = false,
                    Desc = "就绪",
                    value = -1
                });
                int current = 0;
                int CntSuccess = 0;
                int CntError = 0;
                int CntSkip = 0;
                Description = "就绪";
                Progress = 0;
                foreach (var subDir in execList)
                {
                    try
                    {
                        string opath = Path.Combine(Output, $"AFC_Archive_{subDir.Name}.zip");
                        Description = $"正在压缩目录({current + 1}/{execList.Count})，{subDir.FullName}";

                        OnProcess(this, new AFCEventArgs
                        {
                            EventType = LoaderEventType.Progress,
                            IsSuccess = false,
                            Desc = Description,
                            value = Progress
                        });
                        if (File.Exists(opath))
                        {
                            CntSkip++;
                            current++;
                            Progress = 100 * ((double)current / execList.Count);
                            OnProcess(this, new AFCEventArgs
                            {
                                EventType = LoaderEventType.Progress,
                                IsSuccess = false,
                                Desc = Description,
                                value = Progress
                            });
                            Logger.Log($"AFC Exists：{subDir.FullName}，{subDir.CreationTime}，{subDir.LastWriteTime}");
                            continue;
                        }
                        Logger.Log($"AFC Packing：{subDir.FullName}，{subDir.CreationTime}，{subDir.LastWriteTime}");
                        ZipFile.CreateFromDirectory(subDir.FullName, opath, CompressionLevel.Optimal, true, Encoding.UTF8);
                        if (delOrg)
                        {
                            Description = $"正在删除目录({current + 1}/{execList.Count})，{subDir.FullName}";
                            Logger.Log($"AFC Delete：{subDir.FullName}");
                            OnProcess(this, new AFCEventArgs
                            {
                                EventType = LoaderEventType.Progress,
                                IsSuccess = false,
                                Desc = Description,
                                value = Progress
                            });
                            subDir.Delete();
                        }
                        current++;
                        CntSuccess++;
                        Progress = 100 * ((double)current / execList.Count);
                        OnProcess(this, new AFCEventArgs
                        {
                            EventType = LoaderEventType.Progress,
                            IsSuccess = false,
                            Desc = Description,
                            value = Progress
                        });
                        if (IsStop)
                        {
                            isStop = true;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        current++;
                        CntError++;
                        Progress = 100 * ((double)current / execList.Count);
                        Logger.Error(ex.Message, ex, "AFC_EXECUTE_ERROR", "AFC执行过程异常", true);
                    }
                }
                OnProcess(this, new AFCEventArgs
                {
                    EventType = LoaderEventType.ProgressRemove,
                    IsSuccess = true,
                    Desc = $"本次执行{execList.Count}个目录，成功{CntSuccess}个，跳过{CntSkip}个，失败{CntError}个",
                    value = -1
                });
                Logger.Info($"本次执行{execList.Count}个目录，成功{CntSuccess}个，跳过{CntSkip}个，失败{CntError}个", null, "AFC_EXECUTE_RESULT", "AFC执行结果", true);
            }
            catch (Exception ex)
            {
                IsStop = true;
                Logger.Error(ex.Message, ex, "AFC_START_ERROR", "AFC启动异常", true);
                OnProcess(this, new AFCEventArgs
                {
                    EventType = LoaderEventType.ProgressRemove,
                    IsSuccess = false,
                    Desc = "AFC启动异常",
                    value = -1
                });
            }
            finally
            {
                isStop = true;
            }
        }

        public async Task Stop()
        {
            if (IsStop)
            {
                return;
            }
            OnProcess(this, new AFCEventArgs
            {
                EventType = LoaderEventType.Progress,
                IsSuccess = false,
                Desc = "正在停止...",
                value = -1
            });
            Description = "正在停止...";
            IsStop = true;
            while (!isStop)
            {
                Thread.Sleep(50);
            }
            OnProcess(this, new AFCEventArgs
            {
                EventType = LoaderEventType.ProgressRemove,
                IsSuccess = false,
                Desc = "用户停止",
                value = -1
            });
        }
    }
}
