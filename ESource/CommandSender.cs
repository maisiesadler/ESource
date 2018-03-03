using ESource.Base;
using System;
using System.Collections.Generic;

namespace ESource
{
    public class CommandSender : ICommandSender
    {
        private readonly Dictionary<Type, List<Action<Command>>> _routes = new Dictionary<Type, List<Action<Command>>>();
        public void RegisterHandler<T>(ICommandHandler<T> handler) where T : Command
        {
            List<Action<Command>> handlers;

            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<Command>>();
                _routes.Add(typeof(T), handlers);
            }

            handlers.Add(x => handler.Handle((T)x));
        }

        public void Send<T>(T command) where T : Command
        {
            List<Action<Command>> handlers;

            if (_routes.TryGetValue(typeof(T), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }
    }
}
