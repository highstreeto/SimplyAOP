using System;

namespace SimplyAOP
{
    public interface IAspect
    {
        string Name { get; }
    }

    public interface IBeforeAdvice : IAspect
    {
        void Before(Invocation invocation);

        void Before<TParam>(Invocation invocation, ref TParam parameter);
    }

    public interface IAfterAdvice : IAspect
    {
        void AfterReturning(Invocation invocation);

        void AfterReturning<TResult>(Invocation invocation, ref TResult result);

        void AfterThrowing<TResult>(Invocation invocation, ref Exception exception);
    }

    public struct Invocation
    {

    }
}
