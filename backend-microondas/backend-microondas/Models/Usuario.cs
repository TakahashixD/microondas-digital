using System.ComponentModel.DataAnnotations;

namespace backend_microondas.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}
