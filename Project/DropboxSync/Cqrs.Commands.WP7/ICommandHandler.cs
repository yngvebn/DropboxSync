namespace Cqrs.Commands.WP7
{
    public interface ICommandHandler<in T>
    {
        void Handle(T command);
    }
}