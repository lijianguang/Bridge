using System.Diagnostics;

namespace Bridge.Core
{
    internal abstract class ActionMethodExecutor
    {
        private static readonly ActionMethodExecutor[] Executors = new ActionMethodExecutor[]
        {
            // Executors for sync methods
            new VoidResultExecutor(),
            new SyncObjectResultExecutor(),

            // Executors for async methods
            new TaskResultExecutor(),
            new AwaitableResultExecutor(),
            new AwaitableObjectResultExecutor(),
        };

        public abstract ValueTask<object?> Execute(
            ObjectMethodExecutor executor,
            object controller,
            object?[]? arguments);

        protected abstract bool CanExecute(ObjectMethodExecutor executor);

        public static ActionMethodExecutor GetExecutor(ObjectMethodExecutor executor)
        {
            for (var i = 0; i < Executors.Length; i++)
            {
                if (Executors[i].CanExecute(executor))
                {
                    return Executors[i];
                }
            }

            throw new UnreachableException();
        }

        // void LogMessage(..)
        private sealed class VoidResultExecutor : ActionMethodExecutor
        {
            public override ValueTask<object?> Execute(
                ObjectMethodExecutor executor,
                object controller,
                object?[]? arguments)
            {
                executor.Execute(controller, arguments);
                return default;
            }

            protected override bool CanExecute(ObjectMethodExecutor executor)
                => !executor.IsMethodAsync && executor.MethodReturnType == typeof(void);
        }

        // Person GetPerson(..)
        // object Index(..)
        private sealed class SyncObjectResultExecutor : ActionMethodExecutor
        {
            public override ValueTask<object?> Execute(
                ObjectMethodExecutor executor,
                object controller,
                object?[]? arguments)
            {
                // Sync method returning arbitrary object
                var returnValue = executor.Execute(controller, arguments);
                return new(returnValue);
            }

            // Catch-all for sync methods
            protected override bool CanExecute(ObjectMethodExecutor executor) => !executor.IsMethodAsync;
        }

        // Task SaveState(..)
        private sealed class TaskResultExecutor : ActionMethodExecutor
        {
            public override async ValueTask<object?> Execute(
                ObjectMethodExecutor executor,
                object controller,
                object?[]? arguments)
            {
                await (Task)executor.Execute(controller, arguments)!;
                return new();
            }

            protected override bool CanExecute(ObjectMethodExecutor executor) => executor.MethodReturnType == typeof(Task);
        }

        // CustomAsync PerformActionAsync(..)
        // Custom task-like type with no return value.
        private sealed class AwaitableResultExecutor : ActionMethodExecutor
        {
            public override async ValueTask<object?> Execute(
                ObjectMethodExecutor executor,
                object controller,
                object?[]? arguments)
            {
                await executor.ExecuteAsync(controller, arguments);
                return new();
            }

            protected override bool CanExecute(ObjectMethodExecutor executor)
            {
                // Async method returning void
                return executor.IsMethodAsync && executor.AsyncResultType == typeof(void);
            }
        }

        // Task<object> GetPerson(..)
        // Task<Customer> GetCustomerAsync(..)
        private sealed class AwaitableObjectResultExecutor : ActionMethodExecutor
        {
            public override async ValueTask<object?> Execute(
                ObjectMethodExecutor executor,
                object controller,
                object?[]? arguments)
            {
                // Async method returning awaitable-of-nonvoid
                return await executor.ExecuteAsync(controller, arguments);
            }

            protected override bool CanExecute(ObjectMethodExecutor executor) => true;
        }
    }
}
