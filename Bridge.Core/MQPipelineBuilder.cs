namespace Bridge.Core
{
    public class MQPipelineBuilder : IMQPipelineBuilder
    {
        private readonly List<Func<MQDelegate, MQDelegate>> _components = new();

        public IMQPipelineBuilder Use(Func<MQDelegate, MQDelegate> component)
        {
            _components.Add(component);
            return this;
        }

        public IMQPipelineBuilder UseMiddleware<T>()
        {
            return UseMiddleware(typeof(T));
        }

        private IMQPipelineBuilder UseMiddleware(Type middlewareType)
        {
            if (typeof(IMQMiddleware).IsAssignableFrom(middlewareType))
            {
                return Use(next =>
                {
                    return async (context) =>
                    {
                        if (context.RequestServices == null)
                        {
                            // No request services
                            throw new InvalidOperationException("Can't find the RequestServices");
                        }
                        var factory = (IMQMiddlewareFactory?)context.RequestServices.GetService(typeof(IMQMiddlewareFactory));
                        if (factory == null)
                        {
                            // No queue middleware factory
                            throw new InvalidOperationException("Can't find the implement of IMQMiddlewareFactory");
                        }

                        var middleware = factory.Create(context.RequestServices, middlewareType);
                        if (middleware == null)
                        {
                            // The factory returned null, it's a broken implementation
                            throw new InvalidOperationException($"Can't find the instance of {middlewareType.FullName}");
                        }

                        await middleware.InvokeAsync(context, next);
                    };
                });
            }
            throw new NotImplementedException($"Type {middlewareType.FullName} does not implement interface IMQMiddleware");
        }

        public MQDelegate Build()
        {
            MQDelegate app = (model) =>
            {
                //ending
                return Task.CompletedTask;
            };
            for (var c = _components.Count - 1; c >= 0; c--)
            {
                app = _components[c](app);
            }

            return app;
        }
    }
}
