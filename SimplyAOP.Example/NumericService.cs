using System;
using System.Threading;

namespace SimplyAOP.Example
{
    public interface INumericService
    {
        int Random();

        int Sum(int a, int b);

        void Execute(int n);
    }

    public class NumericService : AspectWeaver.Class, INumericService
    {
        private Random random;

        public NumericService(AspectConfiguration config) : base(config) {
            this.random = new Random();
        }

        public int Random() {
            return Advice(() => {
                return random.Next();
            });
        }

        public void Execute(int n_) {
            Advice(n_, n => {
                if (n < 0) {
                    throw new ArgumentOutOfRangeException(nameof(n), n, "N must be positive!");
                }

                Console.Write("  Crunching numbers ... ");
                var sleepTime = TimeSpan.FromMilliseconds(
                    new Random().Next(n * 500)
                );
                Thread.Sleep(sleepTime);
                Console.WriteLine("Done!");
            });
        }

        public int Sum(int a, int b) {
            return Advice((a, b), req => {
                Console.Write("  Summing numbers ... ");
                int result = a + b;
                Console.WriteLine("Done!");
                return result;
            });
        }
    }
}
