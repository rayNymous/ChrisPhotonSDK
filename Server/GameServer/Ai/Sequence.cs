namespace GameServer.Ai
{
    public class Sequence : Composite
    {
        public int _sequence;

        public Sequence()
        {
            Update = () =>
            {
                for (;;)
                {
                    BhStatus s = GetChild(_sequence).Tick();
                    if (s != BhStatus.Success)
                    {
                        if (s == BhStatus.Failure)
                        {
                            _sequence = 0;
                        }
                        return s;
                    }
                    if (++ _sequence == ChildCount)
                    {
                        _sequence = 0;
                        return BhStatus.Success;
                    }
                }
            };

            Initialize = () => { _sequence = 0; };
        }

        public override void Reset()
        {
            Status = BhStatus.Invalid;
        }
    }
}