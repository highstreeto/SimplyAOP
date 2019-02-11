using System;

namespace SimplyAOP.Example
{
    public class MethodConsoleTraceAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Method Console Trace";

        public void Before(Invocation invocation)
            => Console.WriteLine($"Method {invocation.MethodName}() begin");
        public void Before<TParam>(Invocation invocation, ref TParam parameter)
            => Console.WriteLine($"Method {invocation.MethodName}({parameter}) begin");

        public void AfterReturning(Invocation invocation)
            => Console.WriteLine($"Method {invocation.MethodName}() end");
        public void AfterReturning<TResult>(Invocation invocation, ref TResult result)
            => Console.WriteLine($"Method {invocation.MethodName}() = {result} end");
        public void AfterThrowing(Invocation invocation, ref Exception exception)
            => Console.WriteLine($"Method {invocation.MethodName}() ! {ToOneLine(exception)} end");

        private string ToOneLine(Exception exception)
            => $"{exception.GetType().Name}: {exception.Message.Replace(Environment.NewLine, " ")}";
    }
}
