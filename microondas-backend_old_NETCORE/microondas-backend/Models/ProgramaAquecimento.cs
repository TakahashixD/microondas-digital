﻿using microondas_backend.DTOs;
using System.ComponentModel.DataAnnotations;

namespace microondas_backend.Models
{
    public class ProgramaAquecimento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Alimento { get; set; }

        [Required]
        public int Tempo { get; set; }

        [Required]
        [Range(1, 10)]
        public int Potencia { get; set; }

        [Required]
        [MaxLength(1)]
        public string? CaractereAquecimento { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Instrucoes { get; set; }

        public bool Customizado { get; set; } = false;

        public ProgramaAquecimento(ProgramaAquecimentoDTO programa)
        {
            Nome = programa.Nome;
            Alimento = programa.Alimento;
            Tempo = programa.Tempo;
            Potencia = programa.Potencia;
            CaractereAquecimento = programa.CaractereAquecimento;
            Instrucoes = programa.Instrucoes;
            Customizado = programa.Customizado;
        }

        public ProgramaAquecimento()
        {
        }
    }
}

