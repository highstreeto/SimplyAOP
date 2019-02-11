using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimplyAOP
{
    /// <summary>
    /// Contains information of one method invocation which is targeted by the advices
    /// </summary>
    public class Invocation
    {
        private readonly Lazy<Type> targetType;
        private readonly Lazy<MethodInfo> method;
        private readonly Lazy<Type[]> parameterTypes;
        private readonly Dictionary<string, object> store = new Dictionary<string, object>();

        public Invocation(Lazy<Type> targetType, string methodName, Lazy<Type[]> parameterTypes) {
            MethodName = methodName;
            this.targetType = targetType;
            this.parameterTypes = parameterTypes;

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

        public T GetAttribute<T>() where T : Attribute
            => Method.GetCustomAttribute<T>();

        public object this[string key] {
            get { return store[key]; }
            set { store[key] = value; }
        }
    }
}
