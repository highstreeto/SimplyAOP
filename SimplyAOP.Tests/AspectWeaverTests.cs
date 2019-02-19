using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace SimplyAOP.Tests
{
    [TestClass]
    public class AspectWeaverTests
    {
        [TestMethod]
        public void TestBasicBeforeAdvice() {
            var adviceMock = new Mock<IBeforeAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => { value = "called"; });
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);

            weaver.Advice("called2", arg => { value = arg; });
            Assert.AreEqual("called2", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);

            weaver.Advice(() => { value = "called3"; });
            Assert.AreEqual("called3", value);

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);
        }


        [TestMethod]
        public void TestBeforeAdviceWithResult() {
            var adviceMock = new Mock<IBeforeAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            Assert.AreEqual("called", weaver.Advice(() => "called"));

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Never);

            Assert.AreEqual("called2", weaver.Advice("called2", arg => arg));

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, string>>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Never);

            Assert.AreEqual("called3", weaver.Advice(() => "called3"));

            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TestBeforeAdviceAbort() {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()))
                .Callback(() => throw new InvalidOperationException());
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<int, ValueTuple>>()))
                .Callback(() => { });

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice(15, n => { });
            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(() => { });
            });
        }

        [TestMethod]
        public void TestBeforeAdviceChangeParam() {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()))
                .Callback(() => throw new InvalidOperationException());
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<string, ValueTuple>>()))
                .Callback((Invocation<string, ValueTuple> invoc) => invoc.Parameter = "Changed");
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<string, string>>()))
                .Callback((Invocation<string, string> invoc) => invoc.Parameter = "Changed 2");

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Changed", arg));
            Assert.AreEqual("Return", weaver.Advice("Not Changed", arg => {
                Assert.AreEqual("Changed 2", arg);
                return "Return";
            }));

            config = new AspectConfiguration();
            weaver = new AspectWeaver(config, this);

            weaver.Advice("Not Changed", arg => Assert.AreEqual("Not Changed", arg));
            Assert.AreEqual("Return", weaver.Advice("Not Changed", arg => {
                Assert.AreEqual("Not Changed", arg);
                return "Return";
            }));
        }

        [TestMethod]
        public void TestBasicAfterAdvice() {
            var adviceMock = new Mock<IAfterAdvice>();

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => { value = "called"; });
            Assert.AreEqual("called", value);

            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Never);

            weaver.Advice("called2", arg => { value = arg; });
            Assert.AreEqual("called2", value);

            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Never);

            weaver.Advice(() => { value = "called3"; });
            Assert.AreEqual("called3", value);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Never);

            weaver.Advice(() => { return value = "called3"; });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, string>>()), Times.Once);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Never);

            Assert.ThrowsException<Exception>(() => {
                weaver.Advice(() => { throw new Exception(); });
            });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, string>>()), Times.Once);
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<string, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);
        }

        [TestMethod]
        public void TestInvocationMethod() {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()))
                .Callback((Invocation<ValueTuple, string> invoc) => {
                    Assert.AreEqual("TestInvocationMethod", invoc.MethodName);
                    Assert.IsFalse(invoc.IsMethodLookupDone);
                    Assert.AreEqual("TestInvocationMethod", invoc.Method.Name);
                    Assert.IsTrue(invoc.IsMethodLookupDone);

                    Assert.IsNotNull(invoc.GetAttribute<TestMethodAttribute>());
                    Assert.IsNull(invoc.GetAttribute<TestClassAttribute>());

                    Assert.AreEqual("AspectWeaverTests", invoc.TargetType.Name);
                });
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<string, string>>()))
                .Callback((Invocation<string, string> invoc) => {
                    Assert.AreEqual("TestInvocationMethodCall", invoc.MethodName);
                    Assert.IsFalse(invoc.IsMethodLookupDone);
                    Assert.AreEqual("TestInvocationMethodCall", invoc.Method.Name);
                    Assert.IsTrue(invoc.IsMethodLookupDone);

                    Assert.IsNull(invoc.GetAttribute<TestMethodAttribute>());
                    Assert.IsNull(invoc.GetAttribute<TestClassAttribute>());

                    Assert.AreEqual("AspectWeaverTests", invoc.TargetType.Name);
                });

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            string value = "";
            weaver.Advice(() => value = "2");
            Assert.AreEqual("2", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, string>>()), Times.Never);

            value = weaver.Advice(() => "3");
            Assert.AreEqual("3", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, string>>()), Times.Never);

            value = weaver.Advice("4", (string a) => a, "TestInvocationMethodCall");
            Assert.AreEqual("4", value);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, string>>()), Times.Exactly(2));
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<string, string>>()), Times.Once);
        }

        private string TestInvocationMethodCall(string param) {
            return param;
        }

        [TestMethod]
        public void TestThrowException() {
            var adviceMock = new Mock<IAfterAdvice>();
            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(() => throw new InvalidOperationException());
            });
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);

            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(0, p => throw new InvalidOperationException());
            });
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<int, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);

            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(() => {
                    int a = 0;
                    if (a == 0) {
                        throw new InvalidOperationException();
                    }
                    return 0;
                });
            });
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, int>>(), ref It.Ref<Exception>.IsAny), Times.Once);

            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(0, p => {
                    if (p == 0) {
                        throw new InvalidOperationException();
                    }
                    return 0;
                });
            });
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<int, int>>(), ref It.Ref<Exception>.IsAny), Times.Once);
        }

        delegate void AfterThrowingCallBack(Invocation<ValueTuple, ValueTuple> invocation, ref Exception ex);

        [TestMethod]
        public void TestChangeThrownException() {
            var adviceMock = new Mock<IAfterAdvice>();
            adviceMock.Setup(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny))
                .Callback(new AfterThrowingCallBack((Invocation<ValueTuple, ValueTuple> _, ref Exception ex)
                    => ex = new NotSupportedException()));

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            Assert.ThrowsException<NotSupportedException>(() => {
                weaver.Advice(() => throw new InvalidOperationException());
            });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Never);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);

            weaver.Advice(() => { });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);
        }

        [TestMethod]
        public void TestNullThrownException() {
            var adviceMock = new Mock<IAfterAdvice>();
            adviceMock.Setup(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny))
                .Callback(new AfterThrowingCallBack((Invocation<ValueTuple, ValueTuple> _, ref Exception ex)
                    => ex = null));

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            Assert.ThrowsException<InvalidOperationException>(() => {
                weaver.Advice(() => throw new NotSupportedException());
            });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Never);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);

            weaver.Advice(() => { });
            adviceMock.Verify(a => a.AfterReturning(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.AfterThrowing(It.IsAny<Invocation<ValueTuple, ValueTuple>>(), ref It.Ref<Exception>.IsAny), Times.Once);
        }

        [TestMethod]
        public void TestSkipCall() {
            var adviceMock = new Mock<IBeforeAdvice>();
            adviceMock.Setup(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()))
                .Callback((Invocation<ValueTuple, ValueTuple> invoc) => invoc.SkipMethod());

            var config = new AspectConfiguration()
                .AddAspect(adviceMock.Object);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice(() => throw new NotSupportedException());
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);

            Assert.ThrowsException<NotSupportedException>(() => {
                weaver.Advice(0, (int a) => throw new NotSupportedException());
            });
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<ValueTuple, ValueTuple>>()), Times.Once);
            adviceMock.Verify(a => a.Before(It.IsAny<Invocation<int, ValueTuple>>()), Times.Once);
        }
    }
}
