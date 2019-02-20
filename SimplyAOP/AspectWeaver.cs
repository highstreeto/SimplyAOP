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
        private readonly object target;
        private readonly Lazy<Type> targetType;

        public AspectWeaver(AspectConfiguration config, object target) {
            this.config = config;
            this.target = target;
            this.targetType = new Lazy<Type>(() => this.target.GetType());
        }

        public void Advice(Action method, [CallerMemberName] string callerMemberName = null) {
            // Use ValueTuple (without type arguments) as a type for void (is similar to F# Unit)
            var invocation = new Invocation<ValueTuple, ValueTuple>(targetType, callerMemberName);
            try {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);

                if (!invocation.IsSkippingMethod)
                    method();

                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
            } catch (Exception ex) {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                if (ex == null)
                    throw new InvalidOperationException("Exception can not be changed to null!");
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        public void Advice<TParam>(TParam param, Action<TParam> method, [CallerMemberName] string callerMemberName = null) {
            var invocation = new Invocation<TParam, ValueTuple>(targetType, callerMemberName, param);
            try {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);

                if (!invocation.IsSkippingMethod)
                    method(invocation.Parameter);

                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
            } catch (Exception ex) {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                if (ex == null)
                    throw new InvalidOperationException("Exception can not be changed to null!");
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        public TResult Advice<TResult>(Func<TResult> method, [CallerMemberName] string callerMemberName = null) {
            var invocation = new Invocation<ValueTuple, TResult>(targetType, callerMemberName);
            try {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);

                if (!invocation.IsSkippingMethod)
                    invocation.Result = method();

                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
                return invocation.Result;
            } catch (Exception ex) {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                if (ex == null)
                    throw new InvalidOperationException("Exception can not be changed to null!");
                ExceptionDispatchInfo.Capture(ex).Throw();
                return default(TResult);
            }
        }

        public TResult Advice<TParam, TResult>(TParam param, Func<TParam, TResult> method, [CallerMemberName] string callerMemberName = null) {
            var invocation = new Invocation<TParam, TResult>(targetType, callerMemberName, param);
            try {
                foreach (var advice in config.BeforeAdvices)
                    advice.Before(invocation);

                if (!invocation.IsSkippingMethod)
                    invocation.Result = method(invocation.Parameter);

                foreach (var advice in config.AfterAdvices)
                    advice.AfterReturning(invocation);
                return invocation.Result;
            } catch (Exception ex) {
                foreach (var advice in config.AfterAdvices)
                    advice.AfterThrowing(invocation, ref ex);
                if (ex == null)
                    throw new InvalidOperationException("Exception can not be changed to null!");
                ExceptionDispatchInfo.Capture(ex).Throw();
                return default(TResult);
            }
        }

        public class Class
        {
            private readonly AspectWeaver weaver;

            public Class(AspectConfiguration config) {
                weaver = new AspectWeaver(config, this);
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

        public AspectConfiguration() {
            beforeAdvices = new List<IBeforeAdvice>();
            afterAdvices = new List<IAfterAdvice>();
        }

        public IEnumerable<IBeforeAdvice> BeforeAdvices => beforeAdvices;

        public IEnumerable<IAfterAdvice> AfterAdvices => afterAdvices;

        public AspectConfiguration AddAspect<TAspect>() where TAspect : IAspect, new()
            => AddAspect(new TAspect());

        public AspectConfiguration AddAspect(IAspect aspect) {
            if (aspect is IBeforeAdvice before)
                beforeAdvices.Add(before);
            if (aspect is IAfterAdvice after)
                afterAdvices.Insert(0, after);
            return this;
        }
    }
}
