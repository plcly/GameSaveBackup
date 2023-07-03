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

namespace GameSaveManagement.Services
{
    public class GameService
    {
        private LiteDbService _liteDb;

        public GameService()
        {
            _liteDb = new LiteDbService();
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

        public GameModel GetModelById(int modelId)
        {
            return _liteDb.GetModelById(modelId);
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

        public void InitModel(GameModel model)
        {
            if (model.Details == null)
            {
                model.Details = new Dictionary<string, string>();
            }

            if (!Directory.Exists(model.GameBackupPath))
            {
                Directory.CreateDirectory(model.GameBackupPath);
            }

            var folderInfo = new DirectoryInfo(model.GameBackupPath);
            if (folderInfo.Exists)
            {
                var folders = folderInfo.GetDirectories().OrderByDescending(p => p.LastWriteTime);

                var notExistsFolders = model.Details.Where(p => !folders.Any(q => q.Name == p.Value)).Select(p => p.Key).ToList();

                notExistsFolders.ForEach(p => model.Details.Remove(p));

                _liteDb.Update(model);

                var top10Folders = folders.Take(10);
                foreach (var folder in top10Folders)
                {
                    if (model.Details.ContainsKey(folder.Name))
                    {
                        continue;
                    }
                    model.Details.Add(folder.Name, folder.Name);
                }
            }
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
                }
            }
        }
    }
}
