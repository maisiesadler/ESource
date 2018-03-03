namespace ESource.Base
{
    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;
        void RegisterHandler<T>(ICommandHandler<T> handler) where T : Command;
    }
}
