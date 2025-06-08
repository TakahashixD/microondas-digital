using backend_microondas.Services;
using System.Web.ApplicationServices;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace backend_microondas.App_Start
{
    public static class DependencyConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            var container = new UnityContainer();

            // Registrar serviços
            container.RegisterType<AuthService>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}