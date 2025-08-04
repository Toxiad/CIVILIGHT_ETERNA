using CIVILIGHT_ETERNA;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiad.IO.Standar.Module
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    [Table("TOXIAD_BABEL_CONFIGS_CE_CONFIG_V1")]
    public class Config
    {
        [PrimaryKey]
        public int Id { get; set; } = 0;
        public string SourcePath { get; set; } = "D:\\Tokki\\Log";
        public string TargetPath { get; set; } = "F:\\LogPackage";
        public DateTime LastAct { get; set; } = DateTime.MinValue;
        public DateTime NextAct { get; set; } = DateTime.MinValue;
        public bool Enable { get; set; } = false;
        public int Delay { get; set; } = 1;
        public bool Delete { get; set; } = false;
        public bool Compress { get; set; } = true;
        public bool SpaceLowAutoDel { get; set; } = false;
    }
}
