using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentIcons.Common;

namespace EquipmentLogExport.BabelSystem.Alarm
{
    
    public class Alarm
    {
        public string Logo { 
            get {
                switch (Level)
                {
                    case Level.Alarm:
                        return "Warning";
                        break;
                    case Level.Warning:
                        return "Warning";
                        break;
                    case Level.Info:
                        return "Info";
                        break;
                    default:
                        return "Info";
                        break;
                }
            } 
            set { } 
        }
        public DateTime AddTime { get; set; } = DateTime.Now;
        public Level Level { get; set; }
        public OpenMode OpenMode { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public static Alarm Create(Level level, OpenMode openMode, string key, string text, string desc) 
        {
            return new Alarm {
                Level = level,
                OpenMode = openMode, 
                Key = key, 
                Text = text,
                Description = desc,
                AddTime = DateTime.Now
            };
        }
        public static Alarm CreateAlarm(OpenMode openMode, string key, string text, string desc)
        {
            return new Alarm
            {
                Level = Level.Alarm,
                OpenMode = openMode,
                Key = key,
                Text = text,
                Description = desc,
                AddTime = DateTime.Now
            };
        }
        public static Alarm CreateWarn(OpenMode openMode, string key, string text, string desc)
        {
            return new Alarm
            {
                Level = Level.Warning,
                OpenMode = openMode,
                Key = key,
                Text = text,
                Description = desc,
                AddTime = DateTime.Now
            };
        }
        public static Alarm CreateInfo(OpenMode openMode, string key, string text, string desc)
        {
            return new Alarm
            {
                Level = Level.Info,
                OpenMode = openMode,
                Key = key,
                Text = text,
                Description = desc,
                AddTime = DateTime.Now
            };
        }
    }
}
