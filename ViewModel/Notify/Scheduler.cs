using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BLL.EntitesDTO;

namespace ViewModel.Notify
{
    public class NotifyEventArgs : EventArgs
    {
        public int NotifyId { get; }

        public NotifyEventArgs(int notifyId)
        {
            NotifyId = notifyId;
        }
    }

    public static class Scheduler
    {
        public static EventHandler<NotifyEventArgs> RemoveFromDataBase;

        private static BackgroundWorker _notifyWorker;
        private static DispatcherTimer _notifyTimer;
        private static List<DispatcherTimer> _timers;

        public static void ScheduleNotify(IEnumerable<NotificationDTO> notifications)
        {
            _timers = new List<DispatcherTimer>();
            EventHandler<NotifyEventArgs> removeFromDataBase = RemoveFromDataBase;
            var missedNotify = new List<NotificationDTO>();

            foreach (var notify in notifications)
            {
                string notifyString = $"{notify.Subject} from {notify.BeginningDate} to {notify.EndingDate} at {notify.Room}";
                if (notify.BeginningDate.AddMinutes(-15) >= DateTime.Now)
                {
                    _notifyTimer = new DispatcherTimer();

                    var interval = TimeSpan.FromTicks(notify.BeginningDate.Ticks - DateTime.Now.Ticks + TimeSpan.FromMinutes(-15).Ticks);
                    _notifyTimer.Interval = interval;

                    _notifyTimer.Tick += delegate
                    {
                        MessageBox.Show(notifyString);
                        removeFromDataBase?.Invoke(null, new NotifyEventArgs(notify.NotificationId));
                    };

                    _timers.Add(_notifyTimer);

                    _notifyWorker = new BackgroundWorker();
                    _notifyWorker.DoWork += delegate
                    {
                        _notifyTimer?.Start();
                    };
                    _notifyWorker.RunWorkerAsync();
                }
                if (DateTime.Now > notify.BeginningDate.AddMinutes(-15) && DateTime.Now < notify.BeginningDate)
                {
                    MessageBox.Show($"{notifyString} - you have little time!");
                    removeFromDataBase?.Invoke(null, new NotifyEventArgs(notify.NotificationId));
                }
                if (notify.BeginningDate < DateTime.Now)
                {
                    missedNotify.Add(notify);
                    removeFromDataBase?.Invoke(null, new NotifyEventArgs(notify.NotificationId));
                }
            }
            if (missedNotify.Count > 0)
            {
                string missedApp = String.Empty;
                for (int i = 0; i < missedNotify.Count; i++)
                {
                    NotificationDTO infoNotify = missedNotify.ElementAt(i);
                    string finallyInfo =
                        $"{infoNotify.Subject} at {infoNotify.Room} from {infoNotify.BeginningDate} to {infoNotify.EndingDate} - MISSED! \r\n \r\n";
                    missedApp += finallyInfo;
                }
                MessageBox.Show(missedApp);
            }
        }
        public static void Shutdown()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
            }
        }
    }
}