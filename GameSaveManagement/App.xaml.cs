using GameSaveManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GameSaveManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        EventBus _bus;

        public App()
        {
            _bus = new EventBus();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var gameService = new GameService();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddMudServices();
            serviceCollection.AddSingleton<IServiceCollection>(serviceCollection);
            serviceCollection.AddSingleton<GameService>(gameService);
            serviceCollection.AddSingleton<EventBus>(_bus);
            var provider = serviceCollection.BuildServiceProvider();
            Resources.Add("services", provider);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bus.OnExit(this, e);
            base.OnExit(e);
        }
    }
}
