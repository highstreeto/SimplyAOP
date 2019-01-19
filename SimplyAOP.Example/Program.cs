using System;

namespace SimplyAOP.Example
{
    static class Program
    {
        static void Main(string[] args)
        {
            var config = new AspectConfiguration();
            config.AddAspect<MethodConsoleTraceAdvice>();

            ILongRunningService service = new LongRunningService(config);

            service.Execute(n: 10);
            service.Execute(n: 1);
        }
    }
}
