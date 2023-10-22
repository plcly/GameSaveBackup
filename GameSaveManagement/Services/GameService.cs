using GameSaveManagement.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;
using System.Windows.Input;
using GlobalHotKey;
using System.Runtime.InteropServices;

namespace GameSaveManagement.Services
{
    public class GameService
    {
        private LiteDbService _liteDb;
        private string _loadWav;
        private string _saveWav;

        public GameService()
        {
            _liteDb = new LiteDbService();
            _loadWav = Path.Combine(Directory.GetCurrentDirectory(), "load.wav");
            _saveWav = Path.Combine(Directory.GetCurrentDirectory(), "save.wav");
        }

        public IEnumerable<GameModel> GetAll()
        {
            return _liteDb.GetAllGames();
        }

        public int InsertOrUpdate(GameModel model)
        {
            if (model.Id > 0)
            {
                _liteDb.Update(model);
            }
            else
            {
                _liteDb.Insert(model);
            }
            return model.Id;
        }

        public bool Delete(GameModel model)
        {
            if (model.Id > 0)
            {
               return _liteDb.Delete(model);
            }
            return false;
        }

        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public GameModel InitModelById(int modelId)
        {
            var model = _liteDb.GetModelById(modelId);
            if (model == null)
            {
                return model;
            }
            if (model.GameDetails == null)
            {
                model.GameDetails = new List<GameDetail>();
            }

            if (!Directory.Exists(model.GameBackupPath))
            {
                Directory.CreateDirectory(model.GameBackupPath);
            }

            var folderInfo = new DirectoryInfo(model.GameBackupPath);
            if (folderInfo.Exists)
            {
                var folders = folderInfo.GetDirectories().OrderByDescending(p => p.LastWriteTime);

                var notExistsFolders = model.GameDetails.Where(p => !folders.Any(q => q.Name == p.FolderName)).Select(p => p.FolderName).ToList();

                notExistsFolders.ForEach(p => model.GameDetails.RemoveAll(q => q.FolderName == p));

                _liteDb.Update(model);

                model.DisplayDetails = new List<GameDetail>();
                model.GameDetails.ForEach(p => model.DisplayDetails.Add(p));

                var top10Folders = folders.Take(10);
                foreach (var folder in top10Folders)
                {
                    if (model.DisplayDetails.Any(p => p.FolderName == folder.Name))
                    {
                        continue;
                    }
                    model.DisplayDetails.Add(new GameDetail { FolderName = folder.Name});
                }
            }

            return model;
        }

        public void SaveGame(GameModel model)
        {
            if (!Directory.Exists(model.GameBackupPath))
            {
                Directory.CreateDirectory(model.GameBackupPath);
            }
            var newFolderName = model.GameBackupPath + "\\" + DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
            if (!Directory.Exists(newFolderName))
            {
                Directory.CreateDirectory(newFolderName);
            }
            if (!Directory.Exists(model.GameSavePath))
            {
                return;
            }
            CopyDirectory(model.GameSavePath, newFolderName, true);
            PlaySound(_saveWav, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
        }

        public void LoadGame(GameModel model, string path = null)
        {
            if (!Directory.Exists(model.GameSavePath))
            {
                return;
            }
            var folderInfo = new DirectoryInfo(model.GameBackupPath);
            if (folderInfo.Exists)
            {
                DirectoryInfo backupFolder = null;
                if (!string.IsNullOrEmpty(path))
                {
                    backupFolder = new DirectoryInfo(path);
                }
                else
                {
                    backupFolder = folderInfo.GetDirectories().OrderByDescending(p => p.LastWriteTime).FirstOrDefault();
                }
                if (backupFolder != null)
                {
                    CopyDirectory(backupFolder.FullName, model.GameSavePath, true);
                    PlaySound(_loadWav, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
                }
            }
        }

        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

        [System.Flags]
        public enum PlaySoundFlags : int
        {
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }
    }
}
