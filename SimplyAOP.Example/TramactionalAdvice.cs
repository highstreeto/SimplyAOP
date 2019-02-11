using System;
using System.Transactions;

namespace SimplyAOP.Example
{
    public class TranslationalAdvice : IBeforeAdvice, IAfterAdvice
    {
        public string Name => "Translational (Ambient)";

        public void Before(Invocation invocation)
            => BeginTransaction(invocation);
        public void Before<TParam>(Invocation invocation, ref TParam parameter)
            => BeginTransaction(invocation);

        public void AfterReturning(Invocation invocation)
            => EndTransaction(invocation, rollback: false);
        public void AfterReturning<TResult>(Invocation invocation, ref TResult result)
            => EndTransaction(invocation, rollback: false);
        public void AfterThrowing(Invocation invocation, ref Exception exception)
            => EndTransaction(invocation, rollback: true);

        private void BeginTransaction(Invocation invocation)
        {
            var scope = new TransactionScope();
            invocation["txScope"] = scope;
            Console.WriteLine(" TX Start");
        }

        private void EndTransaction(Invocation invocation, bool rollback)
        {
            var scope = (TransactionScope)invocation["txScope"];
            if (!rollback)
            {
                Console.WriteLine(" TX Complete");
                scope.Complete();
            }
            else
            {
                Console.WriteLine(" TX Rollback");
            }
            scope.Dispose();
        }
    }
}
