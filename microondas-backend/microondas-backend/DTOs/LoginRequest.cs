namespace microondas_backend.DTOs
{
    public class LoginRequest
    {
        public required string Nome { get; set; }
        public required string Senha { get; set; }
    }
}
