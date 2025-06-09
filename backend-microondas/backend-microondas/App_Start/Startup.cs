using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;

namespace backend_microondas.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                //Configurar autenticação JWT
                ConfigureAuthentication(app);
                
                var config = new HttpConfiguration();
                
                //Configurar injeção de dependência
                DependencyConfig.Configure(config);
                
                //Configurar Web API (CORS, JSON, Rotas)
                WebApiConfig.Register(config);
                
                //Configurar filtros globais
                FilterConfig.Register(config);
                
                //Aplicar configurações
                app.UseWebApi(config);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na configuração: {ex.Message}");
                throw;
            }
        }

        private void ConfigureAuthentication(IAppBuilder app)
        {
            var secretKey = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["SecretKey"]);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ClockSkew = TimeSpan.Zero
                }
            });
        }
    }
}