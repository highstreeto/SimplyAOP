using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace SimplyAOP.Tests
{
    [TestClass]
    public class AsyncAspectWeaverTests
    {
        [TestMethod]
        public void TestBasicBeforeAsync() {
            var adviceMock = new Mock<IBeforeAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            var signal = new SemaphoreSlim(0, 1);
            var task = weaver.AdviceAsync(async () => {
                await signal.WaitAsync();
                value = "called";
            });
            Assert.AreEqual("", value);
            signal.Release();
            task.Wait();
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
        }
    }
}
