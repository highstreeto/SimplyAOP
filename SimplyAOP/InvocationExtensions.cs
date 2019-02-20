namespace SimplyAOP
{
    public static class InvocationExtensions
    {
        public static bool TryCastParameter<TParam, TResult, TOParam>(this Invocation<TParam, TResult> invocation, out IInvokeWithParameter<TOParam> parameterInvoc) {
            if (typeof(TParam).IsAssignableFrom(typeof(TOParam))) {
                parameterInvoc = (IInvokeWithParameter<TOParam>)invocation;
                return true;
            } else {
                parameterInvoc = null;
                return false;
            }
        }

        public static bool TryCastResult<TParam, TResult, TOResult>(this Invocation<TParam, TResult> invocation, out IInvokeWithResult<TOResult> resultInvoc) {
            if (typeof(TResult).IsAssignableFrom(typeof(TOResult))) {
                resultInvoc = (IInvokeWithResult<TOResult>)invocation;
                return true;
            } else {
                resultInvoc = null;
                return false;
            }
        }
    }

    public interface IInvokeWithParameter<TParam>
    {
        ref TParam Parameter { get; }
    }

    public interface IInvokeWithResult<TResult>
    {
        ref TResult Result { get; }
    }
}
