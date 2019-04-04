# Introduction

SimplyAOP is a .NET library (.NET Standard 2.0+) which allows to use AOP concepts in a simple and straightforward way.

It doesn't use dynamic proxy to accomplish that. Instead the AOP target must invoke a specific method (`Advice(...)`) on the `AspectWeaver` to allow for aspects to kick in.

So SimplyAOP doesn't need to create the instance of the target class so it can be used were instance creation is handled externally (for example by WCF).