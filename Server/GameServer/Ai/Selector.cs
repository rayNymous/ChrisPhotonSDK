namespace GameServer.Ai
{
    public class Selector : Composite
    {
        protected int _selector;

        public Selector()
        {
            Update = () =>
            {
                for (;;)
                {
                    BhStatus s = GetChild(_selector).Tick();
                    if (s != BhStatus.Failure)
                    {
                        if (s == BhStatus.Success)
                        {
                            _selector = 0;
                        }
                        return s;
                    }
                    if (++ _selector == ChildCount)
                    {
                        _selector = 0;
                        return BhStatus.Failure;
                    }
                }
            };

            Initialize = () => { _selector = 0; };
        }

        public override void Reset()
        {
            Status = BhStatus.Invalid;
        }
    }
}