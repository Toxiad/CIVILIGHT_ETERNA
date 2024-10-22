using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CIVILIGHT_ETERNA
{
    public class ProgramInfo
    {
        public string Title { get; set; } = "CIVILIGHT_ETERNA";
        public string MajVer { get; set; } = "0.9";
        public string MinorVer { get; set; } = "1";
        public string ReleaseVer { get; set; } = "394";
        public string ResVer { get; set; } = "97";
        public string DBVer { get; set; } = "2";
        public string Platform { get; set; } = "Win";
        public string Name { get; set; } = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        public string Region { get; set; } = "CN";
        public string Build { get; set; } = "BETA";
        public string DotNetRuntime { get; set; } = Environment.Version.ToString();
        public string OSVer { get; set; } = Environment.OSVersion.ToString();
        public long RAMWorkingSet => Environment.WorkingSet;
        public int CPUCount { get; set; } = Environment.ProcessorCount;
        public string RunDirectory { get; set; } = Environment.CurrentDirectory;
        public string Ver => $"{Region}{Build}{Platform}{MajVer}.{MinorVer}_R{ResVer}_S{ReleaseVer}_D{DBVer}"; 
    }
}
