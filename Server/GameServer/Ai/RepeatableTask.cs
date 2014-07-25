using System;
using System.Threading.Tasks;
using ExitGames.Logging;

namespace GameServer.Ai
{
    public class RepeatableTask
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();
        private Action _action;
        private TimeSpan _delay;
        private bool _stop;

        public RepeatableTask(Action action, TimeSpan delay)
        {
            _action = action;
            _delay = delay;
        }

        public void SetAction(Action action)
        {
            _action = action;
        }

        public void SetDelay(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Start(bool instantStart = false)
        {
            _stop = false;
            if (instantStart)
            {
                _action();
            }
            Schedule();
            OnStart();
        }

        public void Stop()
        {
            _stop = true;
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        private async void Schedule()
        {
            await Task.Delay(_delay);
            if (!_stop)
            {
                _action();
                Schedule();
            }
            else
            {
                OnStop();
            }
        }
    }
}