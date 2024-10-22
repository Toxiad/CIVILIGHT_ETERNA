using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentLogExport.BabelSystem.Alarm
{
    public enum Level
    {
        None = 9,
        Alarm = 1,
        Warning = 2,
        Info = 6,
    }
    public enum OpenMode 
    {
        AlwaysOn = 0,
        CanClose = 4,
    }
}
