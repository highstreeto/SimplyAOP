using System;

namespace SimplyAOP.Example
{
    static class Program
    {
        static void Main(string[] args)
        {
            var config = new AspectConfiguration();
            config.AddAspect<MethodConsoleTraceAdvice>();
            config.AddAspect<MethodWatchAdvice>();

            ILongRunningService service = new LongRunningService(config);

            service.Execute(n: 10);
            service.Execute(n: 1);

            service.ExecuteSum(a: 1, b: -1);
            service.ExecuteSum(a: 100, b: 0);
        }
    }
}
