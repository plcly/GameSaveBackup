using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MudBlazor;
using Microsoft.Extensions.DependencyInjection;
using GameSaveManagement.Services;
using GameSaveManagement.Model;
using System.IO;
using GlobalHotKey;
using System.Windows.Input;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows.Shapes;

namespace GameSaveManagement.pages
{
    public partial class ServerList
    {
        private HotKeyManager _hotKeyManager;
        private HotKey _saveHotKey;
        private HotKey _loadHotKey;
        private bool visible;
        private string renameTxtField;
        private string hotKeyStr;

        private string folderToBeRenamed;

        [Parameter]
        public int? GameModelId { get; set; }

        [Parameter]
        public bool StartGame { get; set; }

        [Inject]
        public GameService Service { get; set; }
        [Inject]
        public EventBus EventBus { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IDialogService MudDialogService { get; set; }

        public GameModel Model { get; set; }

        public ServerList()
        {

        }

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += HandleLocationChanged;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            EventBus.OnExitEvent += HandleLocationChanged;

            if (GameModelId.HasValue)
            {
                _hotKeyManager = new HotKeyManager();
                Model = InitByModelId(GameModelId.Value);
                if (Model == null)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(Model.GameSaveHotKey)
                    && Enum.TryParse<Key>(Model.GameSaveHotKey, out Key saveKey))
                {
                    _saveHotKey = _hotKeyManager.Register(saveKey, System.Windows.Input.ModifierKeys.None);
                }
                if (!string.IsNullOrEmpty(Model.GameLoadHotKey)
                    && Enum.TryParse<Key>(Model.GameLoadHotKey, out Key loadKey))
                {
                    _loadHotKey = _hotKeyManager.Register(loadKey, System.Windows.Input.ModifierKeys.None);
                }

                _hotKeyManager.KeyPressed += HotKeyManagerPressed;

                if (StartGame)
                {
                    RunGame();
                }
            }
        }

        private void RunGame()
        {
            if (!string.IsNullOrEmpty(Model.GameFullPath))
            {
                var info = new ProcessStartInfo(Model.GameFullPath);
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.WorkingDirectory = System.IO.Path.GetDirectoryName(Model.GameFullPath);
                using Process process = Process.Start(info);
            }
        }

        private void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                using var process = Process.Start("explorer.exe", path);
            }
        }

        private async Task RefreshPage()
        {
            await InvokeAsync(() =>
            {
                Model = InitByModelId(Model.Id);
                StateHasChanged();
            });
        }

        private void HotKeyManagerPressed(object? sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == _saveHotKey.Key)//Save
            {
                Service.SaveGame(Model);
                RefreshPage().Wait();
            }
            else if (e.HotKey.Key == _loadHotKey.Key)//Load
            {
                Service.LoadGame(Model, null);
            }
            else
            {
                if (Model.GameDetails.Where(p => p.HotKey != null).Any(p => p.HotKey.Equals(e.HotKey)))
                {
                    var detail = Model.GameDetails.Where(p => p.HotKey != null).FirstOrDefault(p => p.HotKey.Equals(e.HotKey));
                    var fullPath = System.IO.Path.Combine(Model.GameBackupPath, detail.FolderName);
                    Service.LoadGame(Model, fullPath);
                }
            }
        }

        private void LoadGameByPath(string path)
        {
            var fullPath = System.IO.Path.Combine(Model.GameBackupPath, path);
            Service.LoadGame(Model, fullPath);
        }

        private async void DeleteFolder(string path)
        {
            var fullPath = System.IO.Path.Combine(Model.GameBackupPath, path);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }

            await RefreshPage();
        }

        private void HandleLocationChanged(object? sender, EventArgs e)
        {
            _hotKeyManager.Dispose();
        }

        private void OpenDialog(string folderName)
        {
            renameTxtField = string.Empty;
            hotKeyStr = string.Empty;
            folderToBeRenamed = folderName;
            visible = true;
        }
        private async void RenameFolder()
        {
            if (!string.IsNullOrEmpty(folderToBeRenamed)
                && (!string.IsNullOrEmpty(renameTxtField) || !string.IsNullOrEmpty(hotKeyStr)))
            {
                var fullPath = System.IO.Path.Combine(Model.GameBackupPath, folderToBeRenamed);
                if (!Directory.Exists(fullPath))
                {
                    return;
                }
                var pathRenamed = string.IsNullOrEmpty(renameTxtField) ? folderToBeRenamed : renameTxtField;

                if (hotKeyStr == Model.GameSaveHotKey || hotKeyStr == Model.GameLoadHotKey
                    || Model.GameDetails.Where(p => p.HotKey != null).Any(p => p.HotKey.Key.ToString() == hotKeyStr))
                {
                    MudDialogService.ShowMessageBox("错误", "快捷键重复", yesText: "确定");
                    return;
                }
                var hotKeyExist = Enum.TryParse<Key>(hotKeyStr, out Key hotKey);

                var fullPathRenamed = System.IO.Path.Combine(Model.GameBackupPath, pathRenamed);

                if (!string.IsNullOrEmpty(renameTxtField))
                {
                    Directory.Move(fullPath, fullPathRenamed);
                }

                var gameDetail = Model.GameDetails.FirstOrDefault(p => p.FolderName == folderToBeRenamed);
                if (gameDetail == null)
                {
                    Model.GameDetails.Add(new GameDetail
                    {
                        FolderName = pathRenamed,
                        HotKey = hotKeyExist ? new HotKey { Key = hotKey, Modifiers = System.Windows.Input.ModifierKeys.None } : null,
                        HotKeyStr = hotKeyStr
                    });
                }
                else
                {
                    gameDetail.FolderName = pathRenamed;
                    if (hotKeyExist)
                    {
                        if (gameDetail.HotKey != null)
                        {
                            gameDetail.HotKey.Key = hotKey;
                            gameDetail.HotKeyStr = hotKeyStr;
                        }
                        else
                        {
                            gameDetail.HotKey = new HotKey { Key = hotKey, Modifiers = System.Windows.Input.ModifierKeys.None };
                            gameDetail.HotKeyStr = hotKeyStr;
                        }
                    }
                }
                Service.InsertOrUpdate(Model);
                InitByModelId(Model.Id);
                await RefreshPage();
            }
            visible = false;
        }

        private GameModel InitByModelId(int id)
        {
            Model = Service.InitModelById(id);
            var hasHotKeyDetails = Model.GameDetails.Where(p => p.HotKey != null);
            foreach (var detail in hasHotKeyDetails)
            {
                _hotKeyManager.Unregister(detail.HotKey);
                _hotKeyManager.Register(detail.HotKey);
            }
            return Model;
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= HandleLocationChanged;
        }
    }
}
