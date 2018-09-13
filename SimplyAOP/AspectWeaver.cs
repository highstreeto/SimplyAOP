using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimplyAOP
{
    public class AspectWeaver
    {
        private readonly AspectConfiguration config;
        private readonly object target;
        private readonly Lazy<Type> targetType;

        public AspectWeaver(AspectConfiguration config, object target)
        {
            this.config = config;
            this.target = target;

            targetType = new Lazy<Type>(() => target.GetType());
        }

#pragma warning disable RCS1044 // disabled because throw; can not be used because of ref

        public void Advice(Action method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName);
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
                throw ex;
            }
        }

        public void Advice<TParam>(TParam param, Action<TParam> method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(targetType, callerMemberName);
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
                throw ex;
            }
        }

#pragma warning restore RCS1044 // Remove original exception from throw statement.
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
