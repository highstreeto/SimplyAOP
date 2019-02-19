using System;
using System.Transactions;

namespace SimplyAOP.Example
{
    public class TranslationalAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Translational (Ambient)";

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => BeginTransaction(invocation);

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation)
            => EndTransaction(invocation, rollback: false);

        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception)
            => EndTransaction(invocation, rollback: true);

        private void BeginTransaction<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var scope = new TransactionScope();
            invocation["txScope"] = scope;
            Console.WriteLine(" TX Start");
        }

        private void EndTransaction<TParam, TResult>(Invocation<TParam, TResult> invocation, bool rollback) {
            var scope = (TransactionScope)invocation["txScope"];
            if (!rollback) {
                Console.WriteLine(" TX Complete");
                scope.Complete();
            } else {
                Console.WriteLine(" TX Rollback");
            }
            scope.Dispose();
        }
    }
}
