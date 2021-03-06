using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameSaveBackup
{
    public class ConfigModel
    {
        public string DisplayName { get; set; }
        public string GameName { get; set; } 
        public string SavePath { get; set; }
        public string BackupPath { get; set; }
        public BackType BackupType { get; set; }
        public Key SaveKey { get; set; }
        public Key LoadKey { get; set; }
    }
    public enum BackType
    {
        File,
        Folder
    }
}
