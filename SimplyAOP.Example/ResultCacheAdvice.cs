using System;
using System.Collections.Generic;
using System.Text;

namespace SimplyAOP.Example
{
    public class ResultCacheAdvice : IBeforeAdvice, IAfterAdvice
    {
        private readonly IDictionary<object, int> cache =
            new Dictionary<object, int>();

        public string Name => "Result Cache (only for (int) methods)";

        public void Before(Invocation invocation) { }
        public void Before<TParam>(Invocation invocation, ref TParam parameter) {
            if (parameter is int n) {
                if (cache.ContainsKey(n)) {
                    invocation.SkipMethod();
                }
                invocation["cacheKey"] = n;
            }
        }

        public void AfterReturning(Invocation invocation) { }
        public void AfterReturning<TResult>(Invocation invocation, ref TResult result) {
            if (invocation.TryGetEntry("cacheKey", out object key)) {
                if (invocation.IsSkippingMethod)
                    result = (TResult)(object)cache[key];
                else
                    cache[key] = (int)(object)result;
            }
        }
        public void AfterThrowing(Invocation invocation, ref Exception exception) { }
    }
}
