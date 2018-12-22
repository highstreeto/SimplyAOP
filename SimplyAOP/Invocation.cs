using System;
using System.Reflection;

namespace SimplyAOP
{
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
