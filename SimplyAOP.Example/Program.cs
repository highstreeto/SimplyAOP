using System;

namespace SimplyAOP.Example
{
    static class Program
    {
        static string sep = new string('-', 15);

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

            config.AddAspect<ResultCacheAdvice>();
            Console.WriteLine($"{sep} Begin Caching {sep}");
            service.Execute(n: 10);
            service.Execute(n: 10);
            service.Execute(n: 10);
            service.Execute(n: 1);
            service.Execute(n: 1);
            service.Execute(n: 1);
            Console.WriteLine($"{sep} End Caching {sep}");

            service.Sum(a: 1, b: -1);
            service.Sum(a: 100, b: 0);

            FuzzTest();
        }

        static void FuzzTest() {
            var config = new AspectConfiguration();
            config.AddAspect<FuzzerAdvice>();
            config.AddAspect<MethodConsoleTraceAdvice>();

            var service = new NumericService(config);

            Console.WriteLine($"{sep} Begin Fuzz Sum() {sep}");
            for (int i = 0; i < 16; i++) {
                service.Sum(0, 0);
            }
            Console.WriteLine($"{sep} End Fuzz {sep}");
        }
    }
}
