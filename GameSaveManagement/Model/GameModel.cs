using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSaveManagement.Model
{
    public class GameModel
    {
        public int Id { get; set; }
        public string GameFullPath { get; set; }
        public string GameName { get; set; }
        public string GameSavePath { get; set; }
        public string GameBackupPath { get; set; }
        public string GameIconPath { get; set; }
        public string GameSaveHotKey { get; set; }
        public string GameLoadHotKey { get; set; }
        public Dictionary<string, string> Details { get; set; }
    }
}
