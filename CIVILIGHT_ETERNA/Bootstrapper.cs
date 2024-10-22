using CIVILIGHT_ETERNA.Pages;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Toxiad.Common.Logger;

namespace CIVILIGHT_ETERNA
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
        protected override void OnStart()
        {
            // This is called just after the application is started, but before the IoC container is set up.
            // Set up things like logging, etc
            bool createNew;
            Mutex mutex = new Mutex(true, "Toxiad_CIVILIGHT_ETERNA_SingleRun_Mutex", out createNew);
            if (!createNew)
            {
                MessageBox.Show("进程已启动");
                Application.Current.Shutdown();
            }
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<ILogger>().To<Logger>().InSingletonScope();
            builder.Bind<AFC>().ToSelf();
        }
        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
            // BABEL LOADING
            Container.Get<ILogger>().Log("Application Launched", LogType.Warning);
            Container.Get<ILogger>().Log("CIVILIGHT_ETERNA Loading", LogType.Log);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Called on Application.Exit
            Container.Get<ILogger>().Log("Application Exit", LogType.Warning);
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
                Container.Get<IWindowManager>().ShowDialog(new ExceptionViewModel("未捕获的UI线程异常，请及时联系管理员", e.Exception.Message, e.Exception.ToString()));

            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                Container.Get<IWindowManager>().ShowDialog(new ExceptionViewModel("致命UI线程错误，程序即将退出", ex.Message, ex.ToString()));
            }
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            Execute.OnUIThread(() =>
            {
                if (e.IsTerminating)
                {
                    Container.Get<IWindowManager>().ShowDialog(new ExceptionViewModel("致命域内错误，程序即将退出", ((Exception)e.ExceptionObject).Message, e.ExceptionObject.ToString()));
                }
                else
                {
                    Container.Get<IWindowManager>().ShowDialog(new ExceptionViewModel("未捕获的域内异常，请及时联系管理员", ((Exception)e.ExceptionObject).Message, e.ExceptionObject.ToString()));
                }
            });
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            Execute.OnUIThread(() =>
            {
                Container.Get<IWindowManager>().ShowDialog(new ExceptionViewModel("未捕获的Task线程异常，请及时联系管理员", e.Exception.Message, e.Exception.ToString()));
                e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
            });
        }
    }
}
