namespace SimplyAOP
{
    public interface IAdvice
    {
        string Name { get; }
    }

    public interface IBeforeAdvice : IAdvice
    {

    }

    public interface IAfterAdvice : IAdvice
    {

    }
}
