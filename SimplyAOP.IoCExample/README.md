# SimplyAOP.IoCExample

This is a example using an IoC container ([DryIoc](https://github.com/dadhi/DryIoc)) with SimplyAOP

It demonstrates how to register the `AspectConfiguration` and resolving Advices with dependencies.

The core service of this example is to analyse a collection of strings and demonstrates this one a fixed array of strings and on all commit messages of this repository (using [libgit2sharp](https://github.com/libgit2/libgit2sharp)).

## Exemplary Output
```
Processing simple string[]
Process(...) took 00:00:00.0222010
Length min: 11 max: 21 avg: 14,00
- Hello (2)
- World (2)
- This (1)
- is (1)
- a (1)
- simple (1)
- Test (1)
- More (1)
- strings! (1)

Processing git commit messages of this repo.
Process(...) took 00:00:00.0876333
Length min: 13 max: 96 avg: 39,04
- Add (29)
- to (26)
- and (18)
- for (16)
- of (10)
- Invocation (9)
- in (8)
- Use (8)
- README (8)
- Update (7)
- example (7)
- Cake (7)
- AspectWeaver (7)
...
- throw (1)
- this (1)
- Target (1)
- argument (1)
- IAfterAdvice.AfterThrowing (1)
- IAdvice (1)
- IAspect (1)
- structure (1)
- Before- (1)
- AfterAdvices (1)

Recent events:
[20.02.2019 15:28:28]: <highs> Begin call of Process on StatisticsService
[20.02.2019 15:28:28]: <highs> End call of Process on StatisticsService
[20.02.2019 15:28:28]: <highs> Begin call of Process on StatisticsService
[20.02.2019 15:28:28]: <highs> End call of Process on StatisticsService
```