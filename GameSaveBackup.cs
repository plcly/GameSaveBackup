using GlobalHotKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GameSaveBackup
{
    public partial class GameSaveBackup : Form
    {
        private HotKeyManager _hotKeyManager;
        private ConfigModel _gameItem;
        private System.Media.SoundPlayer _playerLoad;
        private System.Media.SoundPlayer _playerSave;

        private Key _saveKey;
        private Key _loadKey;

        public GameSaveBackup()
        {
            InitializeComponent();
            _hotKeyManager = new HotKeyManager();
            _playerLoad = new System.Media.SoundPlayer();
            _playerSave = new System.Media.SoundPlayer();
            var path = Path.GetDirectoryName(Application.ExecutablePath);
            _playerLoad.SoundLocation = Path.Combine(path, "load.wav");
            _playerSave.SoundLocation = Path.Combine(path, "save.wav");
            LoadConfigs();
        }

        private void btnRegist_Click(object sender, EventArgs e)
        {
            var item = gameListBox.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("请先选择游戏");
                return;
            }
            _gameItem = (ConfigModel)item;

            _saveKey = _gameItem.SaveKey;
            _loadKey = _gameItem.LoadKey;

            var hotKeySave = _hotKeyManager.Register(_saveKey, System.Windows.Input.ModifierKeys.None);
            var hotKeyLoad = _hotKeyManager.Register(_loadKey, System.Windows.Input.ModifierKeys.None);

            _hotKeyManager.KeyPressed += HotKeyManagerPressed;

            btnRegist.Enabled = false;
            if (chkRegistBackup.Checked)
            {
                SaveGame();
            }

        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (_gameItem != null)
            {
                if (e.HotKey.Key == _saveKey)//Save
                {
                    SaveGame();
                }
                else if (e.HotKey.Key == _loadKey)//Load
                {
                    LoadGame();
                }
            }

        }

        private void LoadGame()
        {
            switch (_gameItem.BackupType)
            {
                case BackType.File:
                    LoadGameAsFile();
                    break;
                case BackType.Folder:
                    LoadGameAsFolder();
                    break;
                default:
                    break;
            }
        }

        private void LoadGameAsFolder()
        {
            var folderInfo = new DirectoryInfo(_gameItem.BackupPath);
            if (folderInfo.Exists)
            {
                var lastFolder = folderInfo.GetDirectories().OrderByDescending(p => p.LastWriteTime).FirstOrDefault();
                if (lastFolder != null)
                {
                    CopyDirectory(lastFolder.FullName, _gameItem.SavePath, true);
                    ShowMessage("Loaded");
                    _playerLoad.Play();
                }
            }
        }

        private void LoadGameAsFile()
        {
        }

        private void SaveGame()
        {
            switch (_gameItem.BackupType)
            {
                case BackType.File:
                    SaveGameAsFile();
                    break;
                case BackType.Folder:
                    SaveGameAsFolder();
                    break;
                default:
                    break;
            }
        }
        private void SaveGameAsFolder()
        {
            if (!Directory.Exists(_gameItem.BackupPath))
            {
                Directory.CreateDirectory(_gameItem.BackupPath);
            }
            var newFolderName = _gameItem.BackupPath + "\\" + DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
            if (!Directory.Exists(newFolderName))
            {
                Directory.CreateDirectory(newFolderName);
            }
            if (!Directory.Exists(_gameItem.SavePath))
            {
                Directory.CreateDirectory(_gameItem.SavePath);
            }
            CopyDirectory(_gameItem.SavePath, newFolderName, true);
            ShowMessage("Saved");
            _playerSave.Play();

        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
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

        private void SaveGameAsFile()
        {
        }

        private void LoadConfigs()
        {
            try
            {
                var currentFolder = typeof(Program).Assembly.Location;
                var configFile = Path.GetDirectoryName(currentFolder) + "\\" + "GameConfigs.txt";
                if (File.Exists(configFile))
                {
                    var txt = File.ReadAllText(configFile);
                    try
                    {
                        var list = JsonConvert.DeserializeObject<List<ConfigModel>>(txt);
                        gameListBox.DataSource = list;
                        gameListBox.DisplayMember = "DisplayName";
                        gameListBox.ValueMember = "GameName";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show($"[{configFile}] is miss.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ShowMessage(string msg)
        {
            if (chkShowMsg.Checked)
            {
                MessageBox.Show(msg, msg, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1
                , MessageBoxOptions.ServiceNotification);
            }

        }

        private void GameSaveBackup_FormClosing(object sender, FormClosingEventArgs e)
        {
            _hotKeyManager.Dispose();
        }
    }
}
