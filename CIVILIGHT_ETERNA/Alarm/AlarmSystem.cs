using Stylet;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Media;
using Toxiad.Common.Logger;

namespace EquipmentLogExport.BabelSystem.Alarm
{
    public class AlarmSystem
    {
        private ILogger Logger; 
        public ObservableCollection<Alarm> Alarms { get; set; } = new ObservableCollection<Alarm>();
        //public event EventHandler<int> OnAlarmChanged;
        public AlarmSystem(ILogger lg)
        {
            Logger = lg;
            Logger.OnLog += AlarmAutoProcess;
        }
        public void AlarmAutoProcess(object sender, LogEventArgs args)
        {
            OpenMode om = args.CanUserRelease ? OpenMode.CanClose : OpenMode.AlwaysOn;
            switch (args.Type)
            {
                case LogType.Error:
                    OpenAlarm(Alarm.CreateAlarm(om, args.Key, args.Title, args.Message));
                    //Execute.PostToUIThread(() =>
                    //{
                    //    mp.Open(Err);
                    //    mp.Play();
                    //});
                    break;
                case LogType.Warning:
                    OpenAlarm(Alarm.CreateWarn(om, args.Key, args.Title, args.Message));
                    //Execute.PostToUIThread(() =>
                    //{
                    //    mp.Open(Warn);
                    //    mp.Play();
                    //});
                    break;
                case LogType.Info:
                    OpenAlarm(Alarm.CreateInfo(om, args.Key, args.Title, args.Message));
                    break;
                case LogType.Release:
                    CloseAlarm(args.Key);
                    break;
                case LogType.Log:
                default:
                    break;
            }
            //OnAlarmChanged(this, Alarms.Count);
        }
        public void OpenAlarm(Alarm alarm)
        {
            Execute.OnUIThread(() =>
            {
                var atemp = Alarms.FirstOrDefault(a => a.Key == alarm.Key);
                if (atemp != default)
                {
                    Alarms.Remove(atemp);
                }
                //int lastAlarmIndex = 0;
                //int lastWarnIndex = 0;
                //switch (alarm.Level)
                //{
                //    case Level.Alarm:
                //        Alarms.Insert(0, alarm);
                //        break;
                //    case Level.Warning:
                //        var lastAlarm = Alarms.LastOrDefault(i => i.Level == Level.Alarm);
                //        if (lastAlarm != default)
                //        {
                //            lastAlarmIndex = Alarms.IndexOf(Alarms.LastOrDefault(i => i.Level == Level.Alarm)) + 1;
                //        }
                //        Alarms.Insert(lastAlarmIndex, alarm);
                //        break;
                //    case Level.Info:
                //        var lastWarn = Alarms.LastOrDefault(i => i.Level == Level.Warning);
                //        if (lastWarn != default)
                //        {
                //            lastWarnIndex = Alarms.IndexOf(Alarms.LastOrDefault(i => i.Level == Level.Warning)) + 1;
                //        }
                //        Alarms.Insert(lastWarnIndex, alarm);
                //        break;
                //    case Level.None:
                //    default:
                //        break;
                //}
                Alarms.Insert(0, alarm);
            });
        }
        public void CloseAlarm(string key)
        {
            Execute.OnUIThread(() =>
            {
                var atemp = Alarms.FirstOrDefault(a => a.Key == key);
                if (atemp != default)
                {
                    Alarms.Remove(atemp);
                }
                atemp = null;
            });
        }
    }
}
