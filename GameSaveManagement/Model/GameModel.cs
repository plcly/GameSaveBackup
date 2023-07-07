using GlobalHotKey;
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
        public List<GameDetail> GameDetails { get; set; }
        [LiteDB.BsonIgnore]
        public List<GameDetail> DisplayDetails { get; set; }
    }
    public class GameDetail
    {
        public string FolderName { get; set; }
        public string HotKeyStr { get; set; }
        public HotKey HotKey { get; set; }
    }
}
