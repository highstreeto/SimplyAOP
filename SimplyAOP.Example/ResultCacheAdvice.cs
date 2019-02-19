using System;
using System.Collections.Generic;

namespace SimplyAOP.Example
{
    public class ResultCacheAdvice : IBeforeAdvice, IAfterAdvice
    {
        private readonly IDictionary<object, object> cache =
            new Dictionary<object, object>();

        public string Name => "Result Cache";

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var key = invocation.Parameter;
            if (cache.ContainsKey(key)) {
                invocation.SkipMethod();
                invocation.Result = (TResult)cache[key];
            }
        }

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            var key = invocation.Parameter;
            if (!invocation.IsSkippingMethod) {
                cache[key] = invocation.Result;
            }
        }

        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception) { }
    }
}
