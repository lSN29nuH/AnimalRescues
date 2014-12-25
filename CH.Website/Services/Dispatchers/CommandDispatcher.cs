﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.Domain.Contracts.Commands;
using CH.Domain.Services.Commands;
using SimpleInjector;
using TOTD.Utility.ExceptionHelpers;

namespace CH.Website.Services.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Container _container;

        public CommandDispatcher(Container container)
        {
            ThrowIf.Argument.IsNull(container, "kernel");
            this._container = container;
        }

        public async Task<CommandResult> DispatchAsync<TParameter>(TParameter command) where TParameter : class
        {
            return await _container.GetInstance<IAsyncCommandHandler<TParameter>>().ExecuteAsync(command);
        }

        public CommandResult Dispatch<TParameter>(TParameter command) where TParameter : class
        {
            return _container.GetInstance<ICommandHandler<TParameter>>().Execute(command);
        }
    }
}
