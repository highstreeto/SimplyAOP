using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimplyAOP.Example
{
    public interface ILongRunningService
    {
        void Execute(int n);

        int ExecuteSum(int a, int b);
    }

    public class LongRunningService : AspectWeaver.Class, ILongRunningService
    {
        public LongRunningService(AspectConfiguration config)
            : base(config) { }

        public void Execute(int n_)
        {
            Advice(n_, n => {
                Console.Write("  Crunching numbers ... ");
                var sleepTime = TimeSpan.FromMilliseconds(
                    new Random().Next(n * 500)
                );
                Thread.Sleep(sleepTime);
                Console.WriteLine("Done!");
            });
        }

        public int ExecuteSum(int a, int b)
        {
            return Advice((a, b), req => {
                Console.Write("  Summing numbers ... ");
                int result = a + b;
                Console.WriteLine("Done!");
                return result;
            });
        }
    }
}
