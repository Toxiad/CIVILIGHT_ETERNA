using CIVILIGHT_ETERNA.DBs;
using EquipmentLogExport.BabelSystem.Alarm;
using HandyControl.Data;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Stylet;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Linq;
using Toxiad.Common.Logger;
using Toxiad.IO.Standar;
using Toxiad.IO.Standar.Module;
using Vanara.PInvoke;

namespace CIVILIGHT_ETERNA.Pages
{
    public class ShellViewModel : Screen, IHandle<AFCEventArgs>
    {
        //private Process _process = Process.GetCurrentProcess();
        private bool ConfirmToExit = false;
        [Inject] private ILogger Logger;
        [Inject] private IEventAggregator _eventAggregator;
        [Inject] private IWindowManager _WindowManager;
        [Inject] private StyletIoC.IContainer _container;
        AFC _AFC;
        private AlarmSystem alm;
        public ProgramInfo Ver { get; set; } = new ProgramInfo(); 
        public User CurrentUser { get; set; }
        public bool IsInit { get; set; } = false;
        public Config Config { get; set; } = new Config();
        public ObservableCollection<Alarm> AlarmList { get; set; }
        public ShellViewModel()
        {
            //Logger = logger;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Kernel32.SetProcessShutdownParameters(0x4FF, 0);
            Application.Current.SessionEnding += (s, e) =>
            {
                e.Cancel = true;
                Logger.Log($"System Session Ending Event: {s}; {e.ReasonSessionEnding};", LogType.Warning);
            };
        }
        //public ObservableCollection<RunHistory> RunHistories { get; set; } = new ObservableCollection<RunHistory>();
        
        #region WindowAction
        public async void Init()
        {
            //_WindowManager.ShowMessageBox("0");
            Message("正在启动", MessageBoxButton.YesNoCancel, Level.Info);
            Logger.Log("Toxiad AFC StartUp");
            Logger.Log($"Version {Ver.Ver}");
            Logger.Log($"Runtime {Environment.Version}");
            Logger.Log($"OS {Environment.OSVersion}");
            Logger.Log($"User {Environment.UserName}");
            alm = new AlarmSystem(Logger);
            _eventAggregator.Subscribe(this);
            //alm.OnAlarmChanged += (s, i) =>
            //{
            //};
            await Task.Run(async () =>
            {
                Logger.Log("Config Loading");
                await LoadConfig();
                Logger.Log("Config Loaded");
                CurrentUser = await Users.LogoutToOpe();
                //Task.Run(BabelSystem.Babel.Instance.AutoTask);
                Logger.Log($"Login {CurrentUser.AccountId}  {CurrentUser.AccessLevel}", LogType.Warning);
                Logger.Log("Run AutoTask");
                Task.Run(AutoTask);
            });
            AlarmList = alm.Alarms;
            //await Task.Run(() =>
            //{
            //    Thread.Sleep(1000);
            //});
            MConfirm();
            IsInit = true;
            Logger.Log("Toxiad AFC Initialized");
        }
        public void AlarmClick(Alarm sel)
        {
            if (sel.OpenMode == OpenMode.AlwaysOn) { return; }
            if (sel.Level == Level.Alarm)
            {
                if (UserLevelCheck(User.AdmSys, "解除关键提示"))
                {
                    Logger.Release(sel.Key);
                }
            }
            else
            {
                Logger.Release(sel.Key);
            }
        }
        public async void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ConfirmToExit;
            if (!ConfirmToExit)
            {
                if (UserLevelCheck(UserLevel.System, "退出"))
                {
                    if (await Danger("退出"))
                    {
                        ConfirmToExit = true;
                        Message("正在关闭", MessageBoxButton.YesNoCancel, Level.Info);
                        Logger.Log("All System Stop", LogType.Warning);
                        if (_AFC != null)
                        {
                            await Task.Run(_AFC.Stop);
                            Logger.Warn($"用户{CurrentUser.UserName}在{DateTime.Now: yyyy-MM-dd HH:mm:ss}停止了AFC任务", title: "用户停止任务");
                        }
                        Logger.Log($"User {CurrentUser.UserName} has stopped CE at{DateTime.Now: yyyy-MM-dd HH:mm:ss}", LogType.Warning);
                        Logger.Log("Environment.Exit(0);z", LogType.Warning);
                        Environment.Exit(0);
                        //Task.Run(async() =>
                        //{
                        //    Logger.Log("All System Stop", LogType.Warning);
                        //    if (_AFC != null)
                        //    {
                        //        await Task.Run(_AFC.Stop);
                        //        Logger.Warn($"用户{CurrentUser.UserName}在{DateTime.Now: yyyy-MM-dd HH:mm:ss}停止了AFC任务", title: "用户停止任务");
                        //    }
                        //    Logger.Log($"User {CurrentUser.UserName} has stopped CE at{DateTime.Now: yyyy-MM-dd HH:mm:ss}", LogType.Warning);
                        //    Logger.Log("Application.Current.Shutdown", LogType.Warning);
                        //    Application.Current.Shutdown();
                        //});
                    }
                }
            }
        }
        public void OpenFolder(string path)
        {
            Logger.Log($"User OpenFolder Click {path}");
            //if (!Directory.Exists(path))
            //{
            //    return;
            //}
            Process.Start("explorer", path);
        }
        public bool UserLevelCheck(UserLevel level, string actionName = "")
        {
            if (CurrentUser == null) return false;
            if (level.HasFlag(CurrentUser.AccessLevel))
            {
                Logger.Log($"Permission Pass::UserLevel:{CurrentUser.AccessLevel}, Limit:{level}, Action:{actionName}");
                return true;
            }
            Logger.Log($"Permission Unpass::UserLevel:{CurrentUser.AccessLevel}, Limit:{level}, Action:{actionName}");
            Task.Run(async () =>
            {
                string message = actionName != "" ? $"操作「{actionName}」需要权限等级{level}\n当前权限等级{CurrentUser.AccessLevel}，确定以切换账户" : $"当前操作需要权限等级{level}\n当前权限等级{CurrentUser.AccessLevel}，点击确定提权";
                var res = await Message(message, MessageBoxButton.OKCancel, Level.Info);
                if (res == MessageBoxResult.OK)
                {
                    UBoxOpen();
                }
            });
            return false;
        }
        public async Task AutoTask()
        {
            while (true)
            {
                Thread.Sleep(1000);//Per Second
                if (DateTime.Now.Second % 10 == 0)//10 Sec
                {
                    //CurrentUser.LastHeartBeat = DateTime.Now;
                    //_ = Task.Run(async () =>
                    //{
                    //    await SQLUtil.Instance.MainDB.UpdateAsync(CurrentUser);
                    //});
                    //RAMWorkingSet = _process.WorkingSet64;
                }
                if (DateTime.Now.Minute % 10 == 0 && DateTime.Now.Second == 0)//10 Min
                {
                    var root = Path.GetPathRoot(Path.GetFullPath(Config.TargetPath));
                    DriveInfo.GetDrives().ToList().ForEach(i =>
                    {
                        if (i.Name == root && i.TotalFreeSpace < 5 * 1024 * 1024 * 1024L)
                        {
                            Logger.Warn($"当前磁盘剩余空间{i.TotalFreeSpace/1024/1024/1024}GB", null, "AFC_STORAGE_FREE_SPACE_LOW", "磁盘空间不足", false);
                        }
                        else
                        {
                            Logger.Release("AFC_STORAGE_FREE_SPACE_LOW");
                        }
                    });
                }
                if (DateTime.Now >= Config.NextAct)//AFC
                {
#if DEBUG
                    Config.NextAct = DateTime.Now.AddSeconds(30);
#else
                    Config.NextAct = DateTime.Today.AddDays(1).AddHours(4);
#endif
                    SaveConfig();
                    Logger.Log($"AFC Next Act {Config.NextAct}");
                    _ = Task.Run(async () =>
                    {
                        // AFC
                        if (Config.Enable)
                        {
                            Directory.CreateDirectory(Config.TargetPath);
                            if (Directory.Exists(Config.SourcePath))
                            {
                                ProgressShow = true;
                                ProgressTitle = $"AFC {DateTime.Now:yyyy/MM/dd HH:mm:ss}";
                                Logger.Log($"New AFC {DateTime.Now:yyyy/MM/dd HH:mm:ss}");
                                _AFC = _container.Get<AFC>();
                                _AFC.Set(Config.SourcePath, Config.TargetPath, Logger, Config.Delay, Config.Delete);
                                //Execute.PostToUIThread(() => { AFCs.Add(_AFC); });
                                Thread.Sleep(2000);
                                await _AFC.Start();
                                Config.LastAct = DateTime.Now;
                                Logger.Log($"AFC Last Act {Config.LastAct}");
                                SaveConfig();
                                //GC.Collect();
                                Logger.Log($"GC LargeObjectHeapCompact");
                                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                                Logger.Log($"GC Force Collect 0");
                                GC.Collect(0, GCCollectionMode.Forced);
                            }
                            else
                            {
                                Logger.Error($"无法找到目标文件夹{Config.SourcePath}，未能执行自动任务。", null, "AFC_AUTOTASK_FAIL_SOURCE_MISMATCH", "AFC自动任务失败", true);
                            }
                        }
                        else
                        {
                            Logger.Log("AFC AutoTask Closed");
                        }
                    });
                    Logger.FileAutoClear(DateTime.Today.AddDays(-10));
                }
            }
        }
#endregion
        #region Config  
        public async Task LoadConfig()
        {
            try
            {
                Config config = new Config();
                var temp = await SQLUtil.Instance.MainDB.Table<Config>().FirstAsync();
                if (temp != null)
                {
                    config = temp;
                }
                //if (File.Exists(@".\Data\Config.json"))
                //{
                //    //using (var fs = File.OpenRead(@".\Data\Config.json"))
                //    //{
                //    //    //var temp = await JsonSerializer.DeserializeAsync<Config>(fs);
                //    //    var temp = await SQLUtil.Instance.MainDB.Table<Config>().FirstAsync();
                //    //    if (temp != null)
                //    //    {
                //    //        config = temp;
                //    //    }
                //    //    fs.Close();
                //    //}
                //}
                this.Config = config;
                SaveConfig();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex, "BB_CONFIG_LOAD_ERR", "设置恢复失败", true);
            }
        }
        bool ConfigSaveLock = false;
        public async void SaveConfig()
        {
            if (ConfigSaveLock) { return; }
            ConfigSaveLock = true;
            try
            {
                //var temp = JsonSerializer.Serialize(Config);
                //File.WriteAllText(@".\Data\Config.json", temp);
                var res = await SQLUtil.Instance.MainDB.UpdateAsync(Config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex, "BB_CONFIG_SAVE_ERR", "设置保存失败", true);
            }
            ConfigSaveLock = false;
        }
        #endregion
        #region AutoFolderCompressor
        public bool ForceRun { get; set; } = false;
        public bool ProgressShow { get; set; } = false;
        public string ProgressTitle { get; set; } = "当前无任务";
        public string ProgressDesc { get; set; } = "就绪"; 
        public double ProgressValue { get; set; } = 0;
        public bool ProgressRun { get; set; } = false;
        public int AFCDelay { get; set; } = 1; 
        public void Handle(AFCEventArgs e)
        {
            //Execute.PostToUIThread(() => {
            switch (e.EventType)
            {
                case LoaderEventType.Progress:
                    ProgressDesc = e.Desc;
                    ProgressShow = true;
                    if (e.value >= 0)
                    {
                        ProgressRun = true;
                        ProgressValue = e.value;
                    }
                    else
                    {
                        ProgressRun = false;
                    }
                    break;
                case LoaderEventType.ProgressRemove:
                    ProgressShow = false;
                    ProgressRun = false;
                    ForceRun = false;
                    ProgressDesc = "就绪";
                    _AFC = null;
                    //AFCs.Remove(AFCs.Where(i => i.ProcessId == e.ProcessId).FirstOrDefault());
                    break;
                default:
                    break;
            }
            //});
        }
        //private void AFC_OnProcess(object sender, AFCEventArgs e)
        //{
        //    Execute.PostToUIThread(() => {
        //        switch (e.EventType)
        //        {
        //            case LoaderEventType.Progress:
        //                ProgressDesc = e.Desc;
        //                ProgressShow = true;
        //                if (e.value >= 0)
        //                {
        //                    ProgressRun = true;
        //                    ProgressValue = e.value;
        //                }
        //                else
        //                {
        //                    ProgressRun = false;
        //                }
        //                break;
        //            case LoaderEventType.ProgressRemove:
        //                //var s = (AFC)sender;
        //                //s.OnProcess -= AFC_OnProcess;
        //                //_AFC = null;
        //                ProgressShow = false;
        //                ProgressRun = false;
        //                ForceRun = false;
        //                ProgressDesc = "就绪";
        //                //RunHistories.Add(new RunHistory
        //                //{
        //                //    SourcePath = s.Source,
        //                //    TargetPath = s.Output,
        //                //    Act = DateTime.Now,
        //                //    IsSuccess = e.IsSuccess,
        //                //});
        //                //if (RunHistories.Count > 100)
        //                //{
        //                //    RunHistories.RemoveAt(0);
        //                //}
        //                break;
        //            default:
        //                break;
        //        }
        //    });
        //}
        public void AFCRun()
        {
            if (ForceRun)
            {
                return;
            }
            if (UserLevelCheck(User.AdmSys, "执行AFC任务"))
            {
                ForceRun = true;
                Config.NextAct = DateTime.Today;
            }
        }
        public void AFCStop()
        {
            if (UserLevelCheck(User.AdmSys, "停止任务"))
            {
                Task.Run(_AFC.Stop);
                Logger.Warn($"用户{CurrentUser.UserName}在{DateTime.Now: yyyy-MM-dd HH:mm:ss}停止了AFC任务", title: "用户停止任务");
            }
        }
        public async void AFCSourceChange()
        {
            if (UserLevelCheck(User.AdmSys, "更改AFC源路径"))
            {
                if (await Danger("更改AFC源路径"))
                {
                    CommonOpenFileDialog cofd = new CommonOpenFileDialog();
                    cofd.IsFolderPicker = true;
                    cofd.Multiselect = false;
                    cofd.Title = "选择需要进行压缩的文件夹";
                    var res = cofd.ShowDialog();
                    if (res == CommonFileDialogResult.Ok)
                    {
                        Config.SourcePath = cofd.FileName;
                        SaveConfig();
                    }
                }
            }
        }
        public async void AFCDelayChanged(FunctionEventArgs<double> e)
        {
            //if (!IsInit) return;
            if (UserLevelCheck(User.AdmSys, "更改AFC天数"))
            {
                if (e.Info < 1)
                {
                    Config.Delay = 1;
                    e.Info = 1;
                    await Message("起始天数不可小于1", MessageBoxButton.OKCancel, Level.Info);
                }
                else
                {
                    Config.Delay = (int)e.Info;
                }
                SaveConfig();
            }
        }
        public async void AFCDeleteSwitch() 
        {
            if (UserLevelCheck(User.AdmSys, "切换AFC删除条件"))
            {
                if (await Danger("切换AFC删除条件"))
                {
                    Config.Delete = !Config.Delete;
                    SaveConfig();
                }
            }
        }
        public void AFCTargetChange()
        {
            if (UserLevelCheck(User.AdmSys, "更改AFC目标路径"))
            {
                CommonOpenFileDialog cofd = new CommonOpenFileDialog();
                cofd.IsFolderPicker = true;
                cofd.Multiselect = false;
                cofd.Title = "选择压缩文件输出的文件夹";
                var res = cofd.ShowDialog();
                if (res == CommonFileDialogResult.Ok)
                {
                    Config.TargetPath = cofd.FileName;
                    SaveConfig();
                }
            }
        }
        public async void AFCSwitch()
        {
            if (UserLevelCheck(User.AdmSys, "切换AFC状态"))
            {
                if (await Danger("切换AFC状态"))
                {
                    Config.Enable = !Config.Enable;
                    SaveConfig();
                }
            }
        }
        #endregion
        #region MessageLayer
        public async Task<bool> Danger(string actionName)
        {
            string message = actionName != "" ? $"操作「{actionName}」为危险操作，请确认是否执行" : "该操作为危险操作，请确认是否执行";
            return await Message(message, MessageBoxButton.OKCancel, Level.Alarm) == MessageBoxResult.OK;
        }
        public string MsgContent { get; set; } = string.Empty;
        public bool MBoxShow { get; set; } = false;
        public bool MCanClose { get; set; } = false;
        public bool IsDuoButton { get; set; } = false;
        public Level MsgColor { get; set; } = Level.None;
        private int M_Result = 0;
        /// <summary>
        /// Return <c>MessageBoxResult.OK</c> Or <c>MessageBoxResult.Cancel</c>;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="button">YesNoCancel To Ban All Buttons</param>
        /// <returns></returns>
        public async Task<MessageBoxResult> Message(string message, MessageBoxButton button = MessageBoxButton.OK, Level image = Level.None)
        {
            if (MBoxShow)
            {
                MCancel();
                Thread.Sleep(150);
            }
            IsDuoButton = button == MessageBoxButton.OKCancel;
            MCanClose = button != MessageBoxButton.YesNoCancel;
            MsgContent = message;
            MsgColor = image;
            M_Result = 0;
            MBoxShow = true;
            return await Task.Run(async () =>
            {
                while (M_Result == 0)
                {
                    //await Task.Delay(100);
                    Thread.Sleep(100);
                }
                MBoxShow = false;
                return M_Result == 1 ? MessageBoxResult.OK : MessageBoxResult.Cancel;
            });
        }
        public void MConfirm()
        {
            M_Result = 1;
        }
        public void MCancel()
        {
            M_Result = -1;
        }
        #endregion
        #region UserLoginLayer
        public bool UBoxShow { get; set; } = false;
        public string UAccount { get; set; } = string.Empty;
        public string UPassword { get; set; } = string.Empty;
        public void ULogout()
        {
            Task.Run(async () =>
            {
                var user = CurrentUser;
                CurrentUser = await Users.LogoutToOpe();
                CurrentUser = CurrentUser;
                    Logger.Log("Logout", Toxiad.Common.Logger.LogType.Warning);
                await Message($"已退出登录", MessageBoxButton.OK, Level.Info);
            });
        }
        public void UBoxOpen()
        {
            UBoxShow = true;
        }
        public void UConfirm()
        {
            Task.Run(async () =>
            {
                try
                {
                    CurrentUser = await Users.Login(UAccount, UPassword);
                    UCancel();
                    Logger.Log($"Login {CurrentUser.AccountId}  {CurrentUser.AccessLevel}", Toxiad.Common.Logger.LogType.Warning);
                    await Message($"权限等级 {CurrentUser.AccessLevel}\n欢迎，{CurrentUser.UserName}。", MessageBoxButton.OK, Level.Info);
                    Logger.Release("LOGIN_FAIL_ACCOUNT_MISMATCH");
                    Logger.Release("LOGIN_FAIL_UNK");
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Login Fail - User Not Exists")
                    {
                        //UCancel();
                        Logger.Error("用户不存在", ex, "LOGIN_FAIL_ACCOUNT_MISMATCH", "登录失败", true);
                        await Message("用户不存在", MessageBoxButton.OK, Level.Warning);
                    }
                    else if (ex.Message == "Login Fail - Password Verify Error")
                    {
                        //UCancel();
                        Logger.Error("密码错误，无法登录", ex, "LOGIN_FAIL_ACCOUNT_MISMATCH", "登录失败", true);
                        await Message("密码错误，无法登录", MessageBoxButton.OK, Level.Warning);
                    }
                    else
                    {
                        Logger.Error($"登录出现错误：{ex.Message}", ex, "LOGIN_FAIL_UNK", "登录失败", true);
                        await Message($"登录出现错误：{ex.Message}", MessageBoxButton.OK, Level.Alarm);
                    }
                }
            });
        }
        public void UCancel()
        {
            UBoxShow = false;
            UAccount = string.Empty;
            UPassword = string.Empty;
        }


        #endregion
    }
}
