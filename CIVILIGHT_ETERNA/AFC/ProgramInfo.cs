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
        public string Desc { get; set; } = "“文明的存续”";
        public string MajVer { get; set; } = "1.0";
        public string MinorVer { get; set; } = "1";
        public string ReleaseVer { get; set; } = "873";
        public string ResVer { get; set; } = "124";
        public string DBVer { get; set; } = "3";
        public string Platform { get; set; } = "Win";
        public string Name { get; set; } = "DWDB-221E";
        public string Region { get; set; } = "CN";
        public string Build { get; set; } = "REL";
        public string DotNetRuntime { get; set; } = Environment.Version.ToString();
        public string OSVer { get; set; } = Environment.OSVersion.ToString();
        public long RAMWorkingSet => Environment.WorkingSet;
        public int CPUCount { get; set; } = Environment.ProcessorCount;
        public string RunDirectory { get; set; } = Environment.CurrentDirectory;
        public string Ver => $"{Region}{Build}{Platform}{MajVer}.{MinorVer}_R{ResVer}_S{ReleaseVer}_D{DBVer}"; 
    }
}
