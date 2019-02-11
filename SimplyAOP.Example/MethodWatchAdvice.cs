using System;
using System.Diagnostics;

namespace SimplyAOP.Example
{
    public class MethodWatchAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Method Watch";

        public void Before(Invocation invocation)
            => StartWatch(invocation);
        public void Before<TParam>(Invocation invocation, ref TParam parameter)
            => StartWatch(invocation);

        public void AfterReturning(Invocation invocation)
            => StopWatch(invocation);
        public void AfterReturning<TResult>(Invocation invocation, ref TResult result)
            => StopWatch(invocation);
        public void AfterThrowing(Invocation invocation, ref Exception exception)
            => StopWatch(invocation);

        private void StartWatch(Invocation invocation) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            invocation["watch"] = stopwatch;
        }

        private void StopWatch(Invocation invocation) {
            var stopwatch = (Stopwatch)invocation["watch"];
            stopwatch.Stop();
            Console.WriteLine($"  {invocation.MethodName}(...) took {stopwatch.Elapsed}");
        }
    }
}
