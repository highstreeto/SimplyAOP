using System;
using System.Reflection;

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

        void AfterThrowing(Invocation invocation, ref Exception exception);
    }

    public struct Invocation
    {
        private readonly Lazy<Type> targetType;
        private readonly Lazy<MethodInfo> method;

        public Invocation(Lazy<Type> targetType, string methodName)
        {
            MethodName = methodName;
            this.targetType = targetType;

            method = new Lazy<MethodInfo>(() => targetType.Value.GetMethod(methodName));
        }

        public Type TargetType => targetType.Value;

        public string MethodName { get; }
    }
}
