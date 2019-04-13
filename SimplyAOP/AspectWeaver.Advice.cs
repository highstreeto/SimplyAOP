﻿
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace SimplyAOP 
{
    public partial class AspectWeaver
    {
        public void Advice(Action method, [CallerMemberName] string callerMemberName = null) {
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
                return default;
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
                return default;
            }
        }
    }
}

