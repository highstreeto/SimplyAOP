# SimplyAOP

[![License](https://img.shields.io/github/license/highstreeto/SimplyAOP.svg)](https://github.com/highstreeto/SimplyAOP/blob/master/LICENSE)
[![Build Status](https://travis-ci.com/highstreeto/SimplyAOP.svg?branch=master)](https://travis-ci.com/highstreeto/SimplyAOP)
[![NuGet](https://img.shields.io/nuget/v/SimplyAOP.svg)](https://www.nuget.org/packages/SimplyAOP/)

SimplyAOP is a .NET library (.NET Standard 2.0+) which allows to use AOP concepts in a simple and straightforward way.

It doesn't use dynamic proxy to accomplish that. Instead the AOP target must invoke a specific method (`Advice(...)`) on the `AspectWeaver` to allow for aspects to kick in.

So SimplyAOP doesn't need to create the instance of the target class so it can be used were instance creation is handled externally (for example by WCF).

## Goals

- Be as simple as possible
  - No runtime or compile time instrumentation
- Minimize reflection usage

## Basic Example

```csharp
var config = new AspectConfiguration();
config.AddAspect<MethodConsoleTraceAdvice>();

new Target(config).Foo(1, 15);

class Target : AspectWeaver.Class {
    Target(AspectConfiguration config) : base(config) {}

    bool Foo(int a, int b) {
        return Advice((a, b), req => {
            return a == b;
        });
    }
}

class MethodConsoleTraceAdvice : IBeforeAdvice {
    public string Name => "Method Console Trace";
    public void Before<TParam, TResult>(Invocation<TParam, TResult> invocation)
        => Console.WriteLine($"Method {invocation.MethodName}({invocation.Parameter}) begin");
}
```

For a more detailed example see [SimplyAOP.Example](https://github.com/highstreeto/SimplyAOP/tree/master/SimplyAOP.Example) or [SimplyAOP.IoCExample](https://github.com/highstreeto/SimplyAOP/tree/master/SimplyAOP.IoCExample)