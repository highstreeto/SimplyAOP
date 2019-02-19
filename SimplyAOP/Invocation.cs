using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimplyAOP
{
    /// <summary>
    /// Contains information of one method invocation which is targeted by the advices
    /// </summary>
    public class Invocation<TParam, TResult> : IInvokeWithParameter<TParam>, IInvokeWithResult<TResult>
    {
        private readonly Lazy<Type> targetType;
        private readonly Lazy<MethodInfo> method;
        private readonly Lazy<Type[]> parameterTypes;
        private readonly Dictionary<string, object> store = new Dictionary<string, object>();

        private TParam parameter;
        private TResult result;

        public Invocation(Lazy<Type> targetType, string methodName)
            : this(targetType, methodName, default(TParam)) { }

        public Invocation(Lazy<Type> targetType, string methodName, TParam parameter) {
            MethodName = methodName;
            this.targetType = targetType;
            this.parameter = parameter;
            this.parameterTypes = DetermineParameterTypes(parameter);

            method = new Lazy<MethodInfo>(() =>
                this.targetType.Value.GetMethod(methodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: this.parameterTypes.Value,
                    modifiers: null)
                ?? throw new ArgumentException($"Can not find method '{methodName} on type {targetType} with parameters {this.parameterTypes.Value}!'", nameof(methodName))
            );
        }

        public Type TargetType => targetType.Value;

        public string MethodName { get; }
        public bool IsMethodLookupDone => method.IsValueCreated;
        public MethodInfo Method => method.Value;

        public bool IsSkippingMethod { get; private set; }

        public void SkipMethod()
            => IsSkippingMethod = true;

        public ref TParam Parameter => ref parameter;
        public ref TResult Result => ref result;

        public T GetAttribute<T>() where T : Attribute
            => Method.GetCustomAttribute<T>();

        public bool TryGetEntry(string key, out object value) {
            if (store.ContainsKey(key)) {
                value = store[key];
                return true;
            } else {
                value = null;
                return false;
            }
        }

        public object this[string key] {
            get { return store[key]; }
            set { store[key] = value; }
        }

        private static Lazy<Type[]> DetermineParameterTypes(TParam param) {
            return new Lazy<Type[]>(() => {
                var paramType = typeof(TParam);
                if (paramType.FullName.StartsWith("System.ValueTuple")) {
                    return paramType.GenericTypeArguments;
                }
                return new[] { param.GetType() };
            });
        }
    }
}
