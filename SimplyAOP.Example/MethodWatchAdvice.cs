using System;
using System.Diagnostics;

namespace SimplyAOP.Example
{
    public class MethodWatchAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Method Watch";

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            invocation["watch"] = stopwatch;
        }

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => StopWatch(invocation);
        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception)
            => StopWatch(invocation);

        private void StopWatch<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var stopwatch = (Stopwatch)invocation["watch"];
            stopwatch.Stop();
            Console.WriteLine($"  {invocation.MethodName}(...) took {stopwatch.Elapsed}");
        }
    }
}
