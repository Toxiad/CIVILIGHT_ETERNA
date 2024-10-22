using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Toxiad.Common.Logger;
using Vanara.PInvoke;

namespace CIVILIGHT_ETERNA.Pages
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            //Application.Current.SessionEnding += (s, e) =>
            //{
            //    e.Cancel = true;
            //    User32.ShutdownBlockReasonCreate(new WindowInteropHelper(this).Handle, "退出后日志备份将停止，请在开机后重启");
            //};
        }

    }
}
