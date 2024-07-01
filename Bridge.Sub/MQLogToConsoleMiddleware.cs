namespace Bridge.Sub
{
    public class MQLogToConsoleMiddleware : IMQMiddleware
    {
        private readonly IMessageConverter _messageConverter;

        public MQLogToConsoleMiddleware(IMessageConverter messageConverter)
        {
            _messageConverter = messageConverter;
        }

        public async Task InvokeAsync(MQContext context, MQDelegate next)
        {
            Console.WriteLine($"Request Message: {context.Message}");
            await next(context);
            Console.WriteLine($"Respone: {_messageConverter.Serialize(context.Response.Body)}");
        }
    }
}
