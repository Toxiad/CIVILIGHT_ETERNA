using System;
using System.ComponentModel;
using System.Reflection;
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
        }
        public string ExTitle { get; set; } = "错误";
        public string ExDesc { get; set; } = "错误描述";
        public string ExInfo { get; set; } = "错误详情";
    }
}
