using System;
using System.Collections.Generic;
using System.Text;
using SimplyAOP.IoCExample.Services;

namespace SimplyAOP.IoCExample
{
    public class EventAdvice : IBeforeAdvice, IAfterAdvice
    {
        private readonly IEventRepository eventRepo;

        public EventAdvice(IEventRepository eventRepo) {
            this.eventRepo = eventRepo;
        }

        public string Name => "Trace method calls in Event repo.";

        public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            eventRepo.Add(new Model.Event($"Begin call of {invocation.MethodName} on {invocation.TargetType.Name}"));
        }

        public void AfterReturning<TParam, TResult>(Invocation<TParam, TResult> invocation) {
            eventRepo.Add(new Model.Event($"End call of {invocation.MethodName} on {invocation.TargetType.Name}"));
        }

        public void AfterThrowing<TParam, TResult>(Invocation<TParam, TResult> invocation, ref Exception exception) {
            eventRepo.Add(new Model.Event($"End failed call of {invocation.MethodName} on {invocation.TargetType.Name} with {exception.GetType().Name}"));
        }
    }
}
