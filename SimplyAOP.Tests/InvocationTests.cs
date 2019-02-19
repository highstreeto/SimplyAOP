using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimplyAOP.Tests
{
    [TestClass]
    public class InvocationTests
    {
        [TestMethod]
        public void TestKeyValueStore() {
            var watchAdvice = new MethodWatchAdvice();
            var config = new AspectConfiguration()
                .AddAspect(watchAdvice);

            var weaver = new AspectWeaver(config, this);

            weaver.Advice(() => { Thread.Sleep(10); });
            Assert.IsTrue(watchAdvice.TotalElapsed >= TimeSpan.FromMilliseconds(10));
        }
    }

    public class MethodWatchAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Method Watch";

        public TimeSpan TotalElapsed { get; private set; }

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => StartWatch(invocation);

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => StopWatch(invocation);
        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception)
            => StopWatch(invocation);

        private void StartWatch<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            invocation["watch"] = stopwatch;
        }

        private void StopWatch<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var stopwatch = (Stopwatch)invocation["watch"];
            stopwatch.Stop();
            TotalElapsed += stopwatch.Elapsed;
        }
    }
}
