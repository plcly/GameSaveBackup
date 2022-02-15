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
        private HotKeyManager hotKeyManager;
        private ConfigModel gameItem;
        public GameSaveBackup()
        {
            InitializeComponent();
            hotKeyManager = new HotKeyManager();
            LoadConfigs();
        }

       

        private void btnRegist_Click(object sender, EventArgs e)
        {
            if (CheckGameRunning())
            {
                var hotKeySave = hotKeyManager.Register(Key.NumPad4, System.Windows.Input.ModifierKeys.None);
                var hotKeyLoad = hotKeyManager.Register(Key.NumPad6, System.Windows.Input.ModifierKeys.None);

                hotKeyManager.KeyPressed += HotKeyManagerPressed;

                btnRegist.Enabled = false;
            }
            else
            {
                MessageBox.Show("Game is not running!");
            }
        }

        private bool CheckGameRunning()
        {
            var item = gameListBox.SelectedItem;
            if (item == null) return false;
            gameItem = (ConfigModel)item;
            return true; 
            //var gameProcesses = System.Diagnostics.Process.GetProcessesByName(gameItem.GameName + ".exe");
            //if (gameProcesses.Length<=0)
            //{
            //   return false;
            //}
            //return true; 
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (gameItem!=null)
            {
                if (e.HotKey.Key == Key.NumPad4)//Save
                {
                    switch (gameItem.BackupType)
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
                else if (e.HotKey.Key == Key.NumPad6)//Load
                {
                    switch (gameItem.BackupType)
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
            }
            
        }

        private void LoadGameAsFolder()
        {
            var folderInfo=new DirectoryInfo(gameItem.BackupPath);
            if (folderInfo.Exists)
            {
                var lastFolder = folderInfo.GetDirectories().OrderByDescending(p=>p.LastWriteTime).FirstOrDefault();
                if (lastFolder!=null)
                {
                    foreach (var file in lastFolder.GetFiles())
                    {
                        File.Copy(file.FullName, gameItem.SavePath+"\\"+file.Name,true);
                    }
                }
            }
        }

        private void LoadGameAsFile()
        {
        }

        private void SaveGameAsFolder()
        {
            if (!Directory.Exists(gameItem.BackupPath))
            {
                Directory.CreateDirectory(gameItem.BackupPath); 
            }
            var newFolderName = gameItem.BackupPath+"\\" + DateTime.Now.ToString("yyyyMMdd-HH-mm-ss");
            if (!Directory.Exists(newFolderName))
            {
                Directory.CreateDirectory(newFolderName);
            }
            foreach (var file in Directory.GetFiles(gameItem.SavePath))
            {
                File.Copy(file, newFolderName+"\\"+Path.GetFileName(file), true);
            }
        }

        private void SaveGameAsFile()
        {
        }

        private void LoadConfigs()
        {
            if (File.Exists("GameConfigs.txt"))
            {
                var txt = File.ReadAllText("GameConfigs.txt");
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
                MessageBox.Show("[GameConfigs.txt] is miss.");
            }
        }

        private void GameSaveBackup_FormClosing(object sender, FormClosingEventArgs e)
        {
            hotKeyManager.Dispose(); 
        }
    }
}
