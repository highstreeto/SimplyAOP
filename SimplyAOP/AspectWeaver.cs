using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace SimplyAOP
{
    /// <summary>
    /// Simple Aspect Waver which must be directly called
    /// </summary>
    public partial class AspectWeaver
    {
        private readonly AspectConfiguration config;
        private readonly object target;
        private readonly Lazy<Type> targetType;

        public AspectWeaver(AspectConfiguration config, object target) {
            this.config = config;
            this.target = target;
            this.targetType = new Lazy<Type>(() => this.target.GetType());
        }

        public void Advice<TParam>(Action<TParam> method, TParam param, [CallerMemberName] string callerMemberName = null)
            => Advice(param, method, callerMemberName);

        public TResult Advice<TParam, TResult>(Func<TParam, TResult> method, TParam param, [CallerMemberName] string callerMemberName = null)
            => Advice(param, method, callerMemberName);

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
            protected void Advice<TParam>(Action<TParam> method, TParam param, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(param, method, callerMemberName);
            protected TResult Advice<TResult>(Func<TResult> method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(method, callerMemberName);
            protected TResult Advice<TParam, TResult>(TParam param, Func<TParam, TResult> method, [CallerMemberName] string callerMemberName = null)
                => weaver.Advice(param, method, callerMemberName);
            protected TResult Advice<TParam, TResult>(Func<TParam, TResult> method, TParam param, [CallerMemberName] string callerMemberName = null)
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
