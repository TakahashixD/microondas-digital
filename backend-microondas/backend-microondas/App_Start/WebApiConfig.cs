using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace backend_microondas.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Configurar CORS
            ConfigureCors(config);

            //Configurar JSON
            ConfigureJson(config);

            //Configurar Rotas
            ConfigureRoutes(config);
        }

        private static void ConfigureCors(HttpConfiguration config)
        {
            var allowedOrigins = ConfigurationManager.AppSettings["AllowedOrigins"] ?? "http://localhost:4200";
            var corsAttr = new EnableCorsAttribute(
                origins: allowedOrigins,
                headers: "*",
                methods: "*"
            );
            config.EnableCors(corsAttr);
        }

        private static void ConfigureJson(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.JsonFormatter;

            jsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            jsonFormatter.SerializerSettings.DateFormatHandling =
                Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

            jsonFormatter.SerializerSettings.NullValueHandling =
                Newtonsoft.Json.NullValueHandling.Ignore;

            jsonFormatter.SerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.Indented;
        }

        private static void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}