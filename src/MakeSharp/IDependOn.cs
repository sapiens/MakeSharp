namespace MakeSharp
{
    public interface IDependOn
    {
        IDependOn DependOn<T>();
        IConfigureTask But { get; }
    }
}