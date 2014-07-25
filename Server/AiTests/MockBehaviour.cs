using GameServer.Ai;

namespace AiTests
{
    public class MockBehaviour : Behaviour
    {
        public int _iInitializeCalled;
        public int _iUpdateCalled;
        public int _iTerminateCalled;
        public BhStatus _eReturnStatus;
        private BhStatus _eTerminateStatus;

        public MockBehaviour()
        {
            _iInitializeCalled = 0;
            _iTerminateCalled = 0;
            _iUpdateCalled = 0;

            _eReturnStatus = BhStatus.Running;

            Initialize = OnInitialize;
            Update = DoUpdate;
            Terminate = OnTerminate;
        }

        private void OnTerminate(BhStatus obj)
        {
            ++_iTerminateCalled;
            _eTerminateStatus = obj;
        }

        public BhStatus DoUpdate()
        {
            ++_iUpdateCalled;
            return _eReturnStatus;
        }

        public void OnInitialize()
        {
            ++_iInitializeCalled;
        }
    }
}
