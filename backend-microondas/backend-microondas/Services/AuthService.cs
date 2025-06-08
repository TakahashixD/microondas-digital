using backend_microondas.Configs;
using backend_microondas.Data;
using backend_microondas.DTOs;
using backend_microondas.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend_microondas.Services
{
    public class AuthService
    {
        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome)
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginResponse> Auth(LoginRequest request)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var usuario = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Nome == request.Nome);
                
                    if (usuario == null || !CryptoService.VerificarSenha(request.Senha, usuario.Senha))
                    {
                        return new LoginResponse
                        {
                            Sucesso = false,
                            Mensagem = "Credenciais inválidas"
                        };
                    }

                    var token = GenerateJwtToken(usuario);

                    return new LoginResponse
                    {
                        Sucesso = true,
                        Token = token,
                        Nome = usuario.Nome,
                        ExpiresAt = DateTime.UtcNow.AddHours(8),
                        Mensagem = "Autenticação realizada com sucesso"
                    };
            }
                }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro na autenticação: {ex.Message}"
                };
            }
        }

    }
}
