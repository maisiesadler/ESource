namespace ESource.Base
{
    public interface ICommandHandler<TCommand> where TCommand : Command
    {
        void Handle(TCommand message);
    }
}
