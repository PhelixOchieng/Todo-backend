using System.Net;
using System.Text.Json;

namespace Application.Core.Middlewares;

public class ReqResponseLoggingMiddleware : IMiddleware {
  private readonly ILogger<ReqResponseLoggingMiddleware> _logger;

  public ReqResponseLoggingMiddleware(
      ILogger<ReqResponseLoggingMiddleware> logger) {
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
    Console.WriteLine("Middleware hit\n\n");
    Stream originalResponseBody = context.Response.Body;
    MemoryStream responseBodyStream = new MemoryStream();
    context.Response.Body = responseBodyStream;

    try {
      await next(context);

      Console.WriteLine(
          $"\n\nType: {context.Response.ContentType?.Contains("application/json")}");

      if (context.Response.ContentType?.Contains("application/json")
              is not true)
        return;

      try {
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        string responseBody =
            await new StreamReader(responseBodyStream).ReadToEndAsync();
        Console.WriteLine($"Body: {responseBody}");

        var response = new {
          error = false, message = "success",
          data = JsonDocument.Parse(responseBody).RootElement,
          // data = "",
        };
        var serializedResponse = JsonSerializer.Serialize(response);
        Console.WriteLine("\nYow dudes");
        Console.WriteLine(serializedResponse);
        Console.WriteLine("Yow dudes\n\n");
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        // await responseBodyStream.WriteAsync(
        //     Encoding.UTF8.GetBytes(serializedResponse));
        // await responseBodyStream.FlushAsync();
        await context.Response.WriteAsync(serializedResponse);
				context.Response.Body.Seek(0, SeekOrigin.Begin);
				await responseBodyStream.CopyToAsync(originalResponseBody);
        // context.Response.Body = responseBodyStream;
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        Console.WriteLine(
            $"R: {await new StreamReader(responseBodyStream).ReadToEndAsync()}");
      } catch (Exception e) {
        Console.WriteLine("Hello");
        _logger.LogError(e, e.Message);
      }

      // context.Response.ContentType = "application/json";
      // var json = JsonSerializer.Serialize(response);

      // await r.WriteAsJsonAsync(response);
      responseBodyStream.Seek(0, SeekOrigin.Begin);
      Console.WriteLine(
          $"R2: {await new StreamReader(responseBodyStream).ReadToEndAsync()}");
    } catch (Exception e) {
      Console.WriteLine("Error yow\n\n");
      _logger.LogError(e, e.Message);

      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    } finally {
      responseBodyStream.Seek(0, SeekOrigin.Begin);
      var stream = context.Response.Body;
      var read = new StreamReader(stream).ReadToEnd();
      Console.WriteLine(
          $"Read: {read} - {stream} {responseBodyStream} {originalResponseBody}");
      responseBodyStream.Seek(0, SeekOrigin.Begin);
      // Console.WriteLine(
      //     $"Stream: {new StreamReader(originalResponseBody).ReadToEnd()}");

      // context.Response.Body = originalResponseBody;
    }
		responseBodyStream.Seek(0, SeekOrigin.Begin);
  }
}
