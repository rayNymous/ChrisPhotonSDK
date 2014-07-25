namespace GameServer.Ai
{
    public class PrioritySelector : Selector
    {
        private int _lastSelector;

        public PrioritySelector()
        {
            Update = () =>
            {
                _selector = 0;
                for (;;)
                {
                    BhStatus s = GetChild(_selector).Tick();
                    if (s != BhStatus.Failure)
                    {
                        for (int i = _selector + 1; i <= _lastSelector; i++)
                        {
                            GetChild(i).Reset();
                        }
                        _lastSelector = _selector;
                        return s;
                    }
                    if (++_selector == ChildCount)
                    {
                        return BhStatus.Failure;
                    }
                }
            };
        }
    }
}