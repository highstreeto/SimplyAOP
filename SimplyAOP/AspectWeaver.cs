using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SimplyAOP
{
    public class AspectWeaver<TTarget>
    {
        private readonly AspectConfiguration config;
        private readonly TTarget target;

        public AspectWeaver(AspectConfiguration config, TTarget target)
        {
            this.config = config;
            this.target = target;
        }

        public void Advice(Action method, [CallerMemberName] string callerMemberName = null)
        {
            var invocation = new Invocation(callerMemberName);
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
    }

    public class AspectConfiguration
    {
        private readonly List<IBeforeAdvice> beforeAdvices;
        private readonly List<IAfterAdvice> afterAdvices;

        public IEnumerable<IBeforeAdvice> BeforeAdvices => beforeAdvices;

        public IEnumerable<IAfterAdvice> AfterAdvices => afterAdvices;

        public void AddAspect<TAspect>() where TAspect : IAspect, new()
            => AddAspect(new TAspect());

        public void AddAspect(IAspect aspect)
        {
            if (aspect is IBeforeAdvice before)
                beforeAdvices.Add(before);
            if (aspect is IAfterAdvice after)
                afterAdvices.Insert(0, after);
        }
    }
}
