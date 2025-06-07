using System.ComponentModel.DataAnnotations;

namespace microondas_backend.Models
{
    public class ProgramaAquecimento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Alimento { get; set; } = string.Empty;

        [Required]
        public int Tempo { get; set; }

        [Required]
        [Range(1, 10)]
        public int Potencia { get; set; }

        [Required]
        [MaxLength(1)]
        public string CaractereAquecimento { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Instrucoes { get; set; } = string.Empty;

        public bool Customizado { get; set; } = false;
    }
}
