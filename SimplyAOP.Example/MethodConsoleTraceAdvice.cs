using System;

namespace SimplyAOP.Example
{
    public class MethodConsoleTraceAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Method Console Trace";

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => Console.WriteLine($"Method {invocation.MethodName}({invocation.Parameter}) begin");

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => Console.WriteLine($"Method {invocation.MethodName}() = {invocation.Result} end");
        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception)
            => Console.WriteLine($"Method {invocation.MethodName}() ! {ToOneLine(exception)} end");

        private string ToOneLine(Exception exception)
            => $"{exception.GetType().Name}: {exception.Message.Replace(Environment.NewLine, " ")}";
    }
}
