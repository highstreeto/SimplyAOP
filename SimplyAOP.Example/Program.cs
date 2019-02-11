using System;

namespace SimplyAOP.Example
{
    static class Program
    {
        static void Main(string[] args) {
            var config = new AspectConfiguration();
            config.AddAspect<MethodConsoleTraceAdvice>();
            config.AddAspect<TranslationalAdvice>();
            config.AddAspect<MethodWatchAdvice>();

            INumericService service = new NumericService(config);

            service.Random();
            service.Random();

            try {
                service.Execute(n: -1);
            } catch (ArgumentOutOfRangeException) {
                Console.WriteLine("Caught ArgumentOutOfRangeException");
            }
            service.Execute(n: 10);
            service.Execute(n: 1);

            service.Sum(a: 1, b: -1);
            service.Sum(a: 100, b: 0);
        }
    }
}
