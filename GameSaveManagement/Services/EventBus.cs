using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameSaveManagement.Services
{
    public class EventBus
    {
        public event EventHandler<EventArgs> OnExitEvent;
        public void OnExit(object sender, EventArgs e)
        {
            OnExitEvent?.Invoke(sender, e);
        }
    }
}
