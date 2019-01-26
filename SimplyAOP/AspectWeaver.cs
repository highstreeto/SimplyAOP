using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace SimplyAOP
{
    /// <summary>
    /// Simple Aspect Waver which must be directly called
    /// </summary>
    public class AspectWeaver
    {
        private readonly AspectConfiguration config;
        private readonly Lazy<Type> targetType;

        public AspectWeaver(AspectConfiguration config, Lazy<Type> targetType)
        {
            this.config = config;
            this.targetType = targetType;
        }

        public void Advice(Action method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName,
                new Lazy<Type[]>(() => Type.EmptyTypes));
            try
            {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);
                method();
                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
            }
            catch (Exception ex)
            {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        public void Advice<TParam>(TParam param, Action<TParam> method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName,
                DetermineParameterTypes(param));
            try
            {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation, ref param);
                method(param);
                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
            }
            catch (Exception ex)
            {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        public TResult Advice<TResult>(Func<TResult> method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName,
                new Lazy<Type[]>(() => Type.EmptyTypes));
            try
            {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);
                var result = method();
                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation, ref result);
                return result;
            }
            catch (Exception ex)
            {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                return default(TResult);
            }
        }

        public TResult Advice<TParam, TResult>(TParam param, Func<TParam, TResult> method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName,
                DetermineParameterTypes(param));
            try
            {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation, ref param);
                var result = method(param);
                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation, ref result);
                return result;
            }
            catch (Exception ex)
            {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                return default(TResult);
            }
        }

        private static Lazy<Type[]> DetermineParameterTypes<TParam>(TParam param)
        {
            return new Lazy<Type[]>(() => {
                var paramType = typeof(TParam);
                if (paramType.FullName.StartsWith("System.ValueTuple"))
                {
                    return paramType.GenericTypeArguments;
                }
                return new[] { param.GetType() };
            });
        }

        public class Class
        {
            private readonly AspectWeaver weaver;

            public Class(AspectConfiguration config)
            {
                weaver = new AspectWeaver(config, new Lazy<Type>(GetType));
            }

            protected void Advice(Action method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(method, callerMemberName);
            protected void Advice<TParam>(TParam param, Action<TParam> method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(param, method, callerMemberName);
            protected TResult Advice<TResult>(Func<TResult> method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(method, callerMemberName);
            protected TResult Advice<TParam, TResult>(TParam param, Func<TParam, TResult> method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(param, method, callerMemberName);
        }
    }

    public class AspectConfiguration
    {
        private readonly List<IBeforeAdvice> beforeAdvices;
        private readonly List<IAfterAdvice> afterAdvices;

        public AspectConfiguration()
        {
            beforeAdvices = new List<IBeforeAdvice>();
            afterAdvices = new List<IAfterAdvice>();
        }

        public IEnumerable<IBeforeAdvice> BeforeAdvices => beforeAdvices;

        public IEnumerable<IAfterAdvice> AfterAdvices => afterAdvices;

        public AspectConfiguration AddAspect<TAspect>() where TAspect : IAspect, new()
            => AddAspect(new TAspect());

        public AspectConfiguration AddAspect(IAspect aspect)
        {
            if (aspect is IBeforeAdvice before)
                beforeAdvices.Add(before);
            if (aspect is IAfterAdvice after)
                afterAdvices.Insert(0, after);
            return this;
        }
    }
}
