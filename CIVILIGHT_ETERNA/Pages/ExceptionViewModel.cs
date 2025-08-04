using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Stylet;

namespace CIVILIGHT_ETERNA.Pages
{
    public class ExceptionViewModel : Screen
    {
        public ExceptionViewModel() { }
        public ExceptionViewModel(string title, string desc, string info)
        {
            ExTitle = title;
            ExDesc = desc;
            ExInfo = info;
            FileSave();
        }
        public string ExTitle { get; set; } = "错误";
        public string ExDesc { get; set; } = "错误描述";
        public string ExInfo { get; set; } = "错误详情";
        public void FileSave()
        {
            var fullLogPath = Path.GetFullPath("Log");
            var logDirectory = Directory.CreateDirectory(fullLogPath);
            var fullLogName = Path.Combine(logDirectory.FullName, $"Error_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_fff}.log");
            try
            {
                File.AppendAllText(fullLogName, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n{ExTitle}\n{ExDesc}\n{ExInfo}\n\n\n");
            }
            catch
            {
            }
        }
    }
}
