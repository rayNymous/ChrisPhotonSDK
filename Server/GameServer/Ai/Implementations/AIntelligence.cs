using System;
using ExitGames.Logging;

namespace GameServer.Ai
{
    public class AIntelligence
    {
        private readonly Behaviour _prioritySelector;
        private readonly RepeatableTask task;
        protected ILogger Log = LogManager.GetCurrentClassLogger();
        private Action _updateTask;

        public AIntelligence(PrioritySelector prioritySelector)
        {
            if (prioritySelector != null)
            {
                _prioritySelector = prioritySelector;
                _updateTask = () =>
                {
                    try
                    {
                        _prioritySelector.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                };
                task = new RepeatableTask(_updateTask, TimeSpan.FromSeconds(1));
            }
        }

        public void Start()
        {
            if (task != null)
            {
                Log.Debug("Started Ai");
                task.Start();
            }
        }

        public void Stop()
        {
            if (task != null)
            {
                task.Stop();
                _prioritySelector.Reset();
            }
        }
    }
}