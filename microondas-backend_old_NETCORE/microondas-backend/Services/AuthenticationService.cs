using microondas_backend.Configs;
using microondas_backend.Data;
using microondas_backend.DTOs;
using microondas_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace microondas_backend.Services
{
    public class AuthenticationService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome)
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginResponse> Auth(LoginRequest request)
        {
            try
            {
                var usuario = await _context.Usuarios
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
