using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace USUARIOminimalSolution.Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception ex)
            {
                var pd = new ProblemDetails
                {
                    Title = "Erro interno",
                    Detail = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
                httpContext.Response.StatusCode = pd.Status.Value;
                httpContext.Response.ContentType = "application/problem+json";
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(pd));
            }
        }
    }
}
