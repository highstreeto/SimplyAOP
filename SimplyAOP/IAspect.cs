using System;

namespace SimplyAOP
{
    /// <summary>
    /// <para>Root interface of all aspects</para>
    /// <para>Terminology is loosely-based on Spring's AOP</para>
    /// </summary>
    public interface IAspect
    {
        string Name { get; }
    }

    /// <summary>
    /// <para>A advice which is executed before the target method</para>
    /// <para>Can alter the parameter of the method</para>
    /// </summary>
    public interface IBeforeAdvice : IAspect
    {
        void Before<TParam, TResult>(Invocation<TParam, TResult> invocation);
    }

    /// <summary>
    /// <para>A advice which is executed after the target method</para>
    /// <para>Can alter the result and the exception if one is thrown</para>
    /// </summary>
    public interface IAfterAdvice : IAspect
    {
        void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation);

        void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception);
    }
}
