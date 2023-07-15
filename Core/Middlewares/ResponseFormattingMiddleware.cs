using System.Net;
using System.Text.Json;

namespace Application.Core.Middlewares;

public sealed class ResponseFormattingMiddleware : IMiddleware {
  private readonly ILogger<ResponseFormattingMiddleware> _logger;

  public ResponseFormattingMiddleware(
      ILogger<ResponseFormattingMiddleware> logger) => _logger = logger;

  public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
    Stream originalBodyStream = context.Response.Body;

    using (MemoryStream responseBodyStream = new MemoryStream()) {
      try {
        context.Response.Body = responseBodyStream;
        await next(context);
        Console.WriteLine(
            $"Response side -> {context.Response.StatusCode.ToString().StartsWith('2')}");

        string requestUrl = context.Request.Path.ToString();
        bool isMyJsonResponse = !requestUrl.StartsWith("/swagger");

        Console.WriteLine(
            $"Content Type: '{context.Response.ContentType}' {isMyJsonResponse}");
        if (isMyJsonResponse) {
          responseBodyStream.Seek(0, SeekOrigin.Begin);
          string serializedResponse = "";
          bool isOkResponse =
              context.Response.StatusCode.ToString().StartsWith('2');
          if (isOkResponse) {
            string responseBodyString =
                await new StreamReader(responseBodyStream).ReadToEndAsync();

            JsonElement? data = null;
            if (!string.IsNullOrEmpty(responseBodyString)) {
              data = JsonDocument.Parse(responseBodyString).RootElement;
            }

            serializedResponse = BuildResponse(false, "success", data);
          } else {
            string responseBodyString =
                await new StreamReader(responseBodyStream).ReadToEndAsync();
            Console.WriteLine($"Response body: {responseBodyString}");

						JsonElement? data = null;
						string msgString = "An error occured";

            if (!string.IsNullOrEmpty(responseBodyString)) {
              JsonElement responseData =
                  JsonDocument.Parse(responseBodyString).RootElement;
              Console.WriteLine($"Response data: {responseData}");
              JsonElement msgElement = new JsonElement();
              if (responseData.TryGetProperty("title", out msgElement))
                msgString = msgElement.ToString();

							JsonElement dataElement = new JsonElement();
							if (responseData.TryGetProperty("errors", out dataElement))
								data = dataElement;
							else
								data = responseData;
            }

						if (context.Response.StatusCode ==
								StatusCodes.Status404NotFound) {
							msgString = "Resource not found";
							data = null;
						}
            serializedResponse = BuildResponse(true, msgString, data);
          }

          responseBodyStream.SetLength(0);
          responseBodyStream.Seek(0, SeekOrigin.Begin);
          await context.Response.WriteAsync(serializedResponse);
        }
        Console.WriteLine(isMyJsonResponse);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalBodyStream);
      } catch (Exception e) {
        Console.WriteLine("Error");
        _logger.LogError(e, e.Message);

        using MemoryStream responseErrorStream = new MemoryStream();
        context.Response.Body = responseErrorStream;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType =
            "application/problem+json; charset=utf-8";

        var formattedResponseBody = new {
          error = true,
          message = e.Message,
          data = e.Data,
        };

        string serializedResponse =
            JsonSerializer.Serialize(formattedResponseBody);
        responseErrorStream.Seek(0, SeekOrigin.Begin);
        await context.Response.WriteAsync(serializedResponse);

        responseErrorStream.Seek(0, SeekOrigin.Begin);
        await responseErrorStream.CopyToAsync(originalBodyStream);
      }
    }
  }

  private string BuildResponse(bool isError, string message,
                               JsonElement? data) {
    var formattedResponseBody = new {
      error = isError,
      message = message,
      data = data,
    };
    return JsonSerializer.Serialize(formattedResponseBody);
  }
}
