using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace SimplyAOP.Tests
{
    [TestClass]
    public class AspectWeaverTests
    {
        [TestMethod]
        public void TestBasicBeforeAdvice()
        {
            var adviceMock = new Mock<IBeforeAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => value = "called");
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);

            weaver.Advice("called2", arg => value = arg);
            Assert.AreEqual("called2", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);

            weaver.Advice(() => value = "called3");
            Assert.AreEqual("called3", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBeforeAdviceAbort()
        {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>()))
                .Callback(() => throw new InvalidOperationException());

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice(() => Assert.Fail("not aborted by before advice"));
        }

        delegate void BeforeCallback(Invocation invocation, ref string arg);

        [TestMethod]
        public void TestBeforeAdviceChangeParam()
        {
            var adviceMock = new Mock<IBeforeAdvice>();

            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<string>.IsAny))
                .Callback(new BeforeCallback((Invocation _, ref string arg) => arg = "Changed"));

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Changed", arg));

            config = new AspectConfiguration();
            weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Not Changed", arg));
        }
    }
}
