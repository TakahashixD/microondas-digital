using System.Web.Http;
using System.Web.Http.Filters;

namespace backend_microondas.App_Start
{
    public static class FilterConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Filtro global de erro
            config.Filters.Add(new GlobalExceptionFilter());
        }
    }

    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // Log do erro
            var exception = context.Exception;
            System.Diagnostics.Debug.WriteLine($"Erro na API: {exception.Message}");

            base.OnException(context);
        }
    }
}