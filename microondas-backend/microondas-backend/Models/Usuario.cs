using System.ComponentModel.DataAnnotations;

namespace microondas_backend.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Senha { get; set; }
    }
}
