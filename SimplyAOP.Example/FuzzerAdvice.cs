using System;

namespace SimplyAOP.Example
{
    public class FuzzerAdvice : IBeforeAdvice
    {
        private Random random;

        public FuzzerAdvice() {
            random = new Random();
        }

        public string Name => "Fuzz Testing (Parameters are reset by this!)";

        public void Before(Invocation invocation) {
            // NOP
        }

        public void Before<TParam>(Invocation invocation, ref TParam parameter) {
            if (parameter is int) {
                parameter = (TParam)(object)random.Next(int.MinValue, int.MaxValue);
            }
            if (parameter is ValueTuple<int, int> req) {
                parameter = (TParam)(object)(
                    random.Next(int.MinValue, int.MaxValue),
                    random.Next(int.MinValue, int.MaxValue)
                );
            }
        }
    }
}
