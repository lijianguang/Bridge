using System.Runtime.ExceptionServices;

namespace Bridge.Core
{
    public class MQExceptionHandlerMiddleware : IMQMiddleware
    {
        public MQExceptionHandlerMiddleware()
        {
        }

        public Task InvokeAsync(MQContext context, MQDelegate next)
        {
            ExceptionDispatchInfo edi;

            try
            {
                var task = next(context);
                if (!task.IsCompletedSuccessfully)
                {
                    return Awaited(this, context, task);
                }

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                edi = ExceptionDispatchInfo.Capture(exception);
            }

            return HandleException(context, edi);
            static async Task Awaited(MQExceptionHandlerMiddleware middleware, MQContext context, Task task)
            {
                ExceptionDispatchInfo? edi = null;
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    edi = ExceptionDispatchInfo.Capture(exception);
                }

                if (edi != null)
                {
                    await middleware.HandleException(context, edi);
                }
            }
        }
        private async Task HandleException(MQContext context, ExceptionDispatchInfo edi)
        {
            await Task.Run(() =>
            {
                context.Response.Body = new ResponseBody() { Payload = edi.SourceException, StatusCode = MQStatusCode.InternalServerError };
            });
        }
    }
}
