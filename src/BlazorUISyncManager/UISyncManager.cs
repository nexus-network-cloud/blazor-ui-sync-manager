using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.BlazorUISyncManager
{
    public interface IUISyncManager
    {
        bool EnableAutoUIRefreshTimer { get; set; }
        int AutoUIRefreshTimerPeriod { get; set; }

        IDisposable Subscribe(IObserver<UISyncEvent> observer);
        void RequestUIRefresh();
    }

    public class UISyncManager : IUISyncManager, IObservable<UISyncEvent>
    {
        private readonly List<IObserver<UISyncEvent>> _observers = new List<IObserver<UISyncEvent>>();
        private Timer uiRefreshTimer;

        private bool enableAutoUIRefreshTimer = true;
        public bool EnableAutoUIRefreshTimer
        {
            get
            {
                return enableAutoUIRefreshTimer;
            }
            set
            {
                enableAutoUIRefreshTimer = value;
                if (enableAutoUIRefreshTimer)
                    uiRefreshTimer.Change(0, AutoUIRefreshTimerPeriod);
                else
                    uiRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public int AutoUIRefreshTimerPeriod { get; set; } = 1000;

        public UISyncManager()
        {
            uiRefreshTimer = new Timer(RefreshUI, null, 0, AutoUIRefreshTimerPeriod);
        }

        public IDisposable Subscribe(IObserver<UISyncEvent> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber<UISyncEvent>(_observers, observer);
        }

        public class Unsubscriber<UIStateEvent> : IDisposable
        {
            private List<IObserver<UIStateEvent>> _observers;
            private IObserver<UIStateEvent> _observer;

            internal Unsubscriber(List<IObserver<UIStateEvent>> observers, IObserver<UIStateEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void RequestUIRefresh()
        {
            RefreshUI(null);
        }

        private void RefreshUI(object? state)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(new UISyncEvent());
            }
        }
    }

    public class UISyncEvent
    {

    }
}
