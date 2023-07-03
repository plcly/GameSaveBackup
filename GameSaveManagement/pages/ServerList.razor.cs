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

namespace GameSaveManagement.pages
{
    public partial class ServerList
    {
        private HotKeyManager _hotKeyManager;
        private HotKey _saveHotKey;
        private HotKey _loadHotKey;

        [Parameter]
        public string GameModelId { get; set; }

        [Inject]
        public GameService Service { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }

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
            if (int.TryParse(GameModelId, out int modelId))
            {
                Model = Service.GetModelById(modelId);
                if (Model == null)
                {
                    return;
                }
                Service.InitModel(Model);
                _hotKeyManager = new HotKeyManager();
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
                StateHasChanged();
            });
        }

        private void HotKeyManagerPressed(object? sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == _saveHotKey.Key)//Save
            {
                Service.SaveGame(Model);
            }
            else if (e.HotKey.Key == _loadHotKey.Key)//Load
            {
                Service.LoadGame(Model, null);
            }
        }

        private void LoadGameByPath(string path)
        {
            var fullPath = Path.Combine(Model.GameBackupPath, path);
            Service.LoadGame(Model, fullPath);
        }

        private async void DeleteFolder(string path)
        {
            var fullPath = Path.Combine(Model.GameBackupPath, path);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
            Service.InitModel(Model);
            await RefreshPage();
        }

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _hotKeyManager.Dispose();
        }
        public void Dispose()
        {
            Navigation.LocationChanged -= HandleLocationChanged;
        }
    }
}
