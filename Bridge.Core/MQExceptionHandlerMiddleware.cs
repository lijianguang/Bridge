using System.Runtime.ExceptionServices;

namespace Bridge.Core
{
    public class MQExceptionHandlerMiddleware : IMQMiddleware
    {
        public MQExceptionHandlerMiddleware()
        {
        }

        public async Task InvokeAsync(MQContext context, MQDelegate next)
        {
            ExceptionDispatchInfo edi;

            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                edi = ExceptionDispatchInfo.Capture(exception);
                await HandleException(context, edi);
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
