namespace microondas_backend.DTOs
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Nome { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
    }
}
