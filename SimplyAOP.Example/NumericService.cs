using System;
using System.Threading;

namespace SimplyAOP.Example
{
    public interface INumericService
    {
        int Random(Random random = null);

        int Sum(int a, int b);

        int Execute(int n);
    }

    public class NumericService : AspectWeaver.Class, INumericService
    {
        private Random random;

        public NumericService(AspectConfiguration config) : base(config) {
            this.random = new Random();
        }

        public int Random(Random random_ = null) {
            return Advice(random_, random => {
                if (random == null)
                    random = this.random;
                return random.Next();
            });
        }

        public int Execute(int n_) {
            return Advice(n_, n => {
                if (n < 0) {
                    throw new ArgumentOutOfRangeException(nameof(n), n, "N must be positive!");
                }

                Console.Write("  Crunching numbers ... ");
                var sleepTime = TimeSpan.FromMilliseconds(
                    new Random().Next(n * 500)
                );
                Thread.Sleep(sleepTime);
                Console.WriteLine("Done!");

                return n * 500;
            });
        }

        public int Sum(int a, int b) {
            return Advice((a, b), req => {
                Console.Write("  Summing numbers ... ");
                int result = req.a + req.b;
                Console.WriteLine("Done!");
                return result;
            });
        }
    }
}
