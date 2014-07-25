using AiTests;
using GameServer.Ai;
using NUnit.Framework;

namespace ServerTests.AiTests
{
    public class BehaviourTests
    {
        [Test]
        public void Tick_DoesInitialize_Successful()
        {
            MockBehaviour t = new MockBehaviour();

            Assert.AreEqual(0, t._iInitializeCalled);

            t.Tick();

            Assert.AreEqual(1, t._iInitializeCalled);
        }

        [Test]
        public void Tick_UpdateCalled_ReturnsSuccess()
        {
            MockBehaviour t = new MockBehaviour();
            t.Tick();
            Assert.AreEqual(1, t._iUpdateCalled);

            t._eReturnStatus = BhStatus.Success;
            
            t.Tick();
            Assert.AreEqual(2, t._iUpdateCalled);
        }

        [Test]
        public void Tick_TerminateCalled_ReturnsSuccess()
        {
            MockBehaviour t = new MockBehaviour();
            t.Tick();
            
            Assert.AreEqual(0, t._iTerminateCalled);

            t._eReturnStatus = BhStatus.Success;
            t.Tick();
            Assert.AreEqual(1, t._iTerminateCalled);
        }
    }
}
