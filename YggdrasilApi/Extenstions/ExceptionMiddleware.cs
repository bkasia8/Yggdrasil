using System.Net;
using System.Text.Json;

namespace YggdrasilApi.Extenstions {
    public class ExceptionMiddleware {
        public RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionMiddleware> logger) {
            _logger = logger;
            _requestDelegate = requestDelegate;
        }
        public async Task Invoke(HttpContext context) {
            try {
                await _requestDelegate(context);
            }
            catch (Exception ex) {
                await HandleExecption(context, ex);
            }
        }
        private Task HandleExecption(HttpContext context, Exception exception) {
            _logger.LogError(exception.ToString());
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(
                new ErrorDetails2(context.Response.StatusCode, exception.Message).ToString());
        }
    }

    public record ErrorDetails2(int StatusCode, string Message) {
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }
}
