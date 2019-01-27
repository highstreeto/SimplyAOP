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
            weaver.Advice(() => { value = "called"; });
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);

            weaver.Advice("called2", arg => { value = arg; });
            Assert.AreEqual("called2", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);

            weaver.Advice(() => { value = "called3"; });
            Assert.AreEqual("called3", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TestBeforeAdviceWithResult()
        {
            var adviceMock = new Mock<IBeforeAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            Assert.AreEqual("called", weaver.Advice(() => "called"));

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);

            Assert.AreEqual("called2", weaver.Advice("called2", arg => arg));

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);

            Assert.AreEqual("called3", weaver.Advice(() => "called3" ));

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

        delegate void BeforeStringCallback(Invocation invocation, ref string arg);

        [TestMethod]
        public void TestBeforeAdviceChangeParam()
        {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>()))
                .Callback(() => throw new InvalidOperationException());

            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<string>.IsAny))
                .Callback(new BeforeStringCallback((Invocation _, ref string arg) => arg = "Changed"));

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Changed", arg));

            config = new AspectConfiguration();
            weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Not Changed", arg));
        }

        [TestMethod]
        public void TestBasicAfterAdvice()
        {
            var adviceMock = new Mock<IAfterAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => { value = "called"; });
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation>(), ref It.Ref<Exception>.IsAny), Times.Never);

            weaver.Advice("called2", arg => { value = arg; });
            Assert.AreEqual("called2", value);

            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>()), Times.Exactly(2));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation>(), ref It.Ref<Exception>.IsAny), Times.Never);

            weaver.Advice(() => { value = "called3"; });
            Assert.AreEqual("called3", value);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>()), Times.Exactly(3));

            weaver.Advice(() => { return value = "called3"; });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>()), Times.Exactly(3));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation>(), ref It.Ref<Exception>.IsAny), Times.Never);

            Assert.ThrowsException<Exception>(() => {
                weaver.Advice(() => { throw new Exception(); });
            });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>()), Times.Exactly(3));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation>(), ref It.Ref<Exception>.IsAny), Times.Once);
        }

        [TestMethod]
        public void TestInvocationMethod() {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>()))
                .Callback((Invocation invoc) => {
                    Assert.AreEqual("TestInvocationMethod", invoc.MethodName);
                    Assert.AreEqual("TestInvocationMethod", invoc.Method.Name);

                    Assert.IsNotNull(invoc.GetAttribute<TestMethodAttribute>());
                    Assert.IsNull(invoc.GetAttribute<TestClassAttribute>());

                    Assert.AreEqual("AspectWeaverTests", invoc.TargetType.Name);
                });
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<string>.IsAny))
                .Callback(new BeforeStringCallback((Invocation invoc, ref string param) => {
                    Assert.AreEqual("TestInvocationMethodCall", invoc.MethodName);
                    Assert.AreEqual("TestInvocationMethodCall", invoc.Method.Name);

                    Assert.IsNull(invoc.GetAttribute<TestMethodAttribute>());
                    Assert.IsNull(invoc.GetAttribute<TestClassAttribute>());

                    Assert.AreEqual("AspectWeaverTests", invoc.TargetType.Name);
                })
            );

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => value = "2");
            Assert.AreEqual("2", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);

            value = weaver.Advice(() => "3");
            Assert.AreEqual("3", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Exactly(2));
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Never);

            value = weaver.Advice("4", (string a) => a, "TestInvocationMethodCall");
            Assert.AreEqual("4", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>()), Times.Exactly(2));
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation>(), ref It.Ref<object>.IsAny), Times.Once);
        }

        private string TestInvocationMethodCall(string param)
        {
            return param;
        }
    }
}
