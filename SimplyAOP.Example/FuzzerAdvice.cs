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

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            if (invocation.TryCastParameter(out IInvokeWithParameter<int> intInvoc)) {
                intInvoc.Parameter = random.Next(int.MinValue, int.MaxValue);
            }
            if (invocation.TryCastParameter(out IInvokeWithParameter<(int, int)> intsInvoc)) {
                intsInvoc.Parameter = (
                    random.Next(int.MinValue, int.MaxValue),
                    random.Next(int.MinValue, int.MaxValue)
                );
            }
        }
    }
}
