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

        private Key _saveKey;
        private Key _loadKey;

        public GameSaveBackup()
        {
            InitializeComponent();
            _hotKeyManager = new HotKeyManager();
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
                    foreach (var file in lastFolder.GetFiles())
                    {
                        File.Copy(file.FullName, _gameItem.SavePath + "\\" + file.Name, true);
                    }
                    ShowMessage("Loaded");
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
            foreach (var file in Directory.GetFiles(_gameItem.SavePath))
            {
                File.Copy(file, newFolderName + "\\" + Path.GetFileName(file), true);
            }
            ShowMessage("Saved");
            
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
