using microondas_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace microondas_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<ProgramaAquecimento> ProgramasAquecimento { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações Fluent API
            modelBuilder.Entity<ProgramaAquecimento>(entity =>
            {
                entity.HasIndex(e => e.Nome).IsUnique();
                entity.HasIndex(e => e.CaractereAquecimento).IsUnique();
            });

            modelBuilder.Entity<ProgramaAquecimento>().HasData(
                new ProgramaAquecimento
                {
                    Id = 1,
                    Nome = "Pipoca",
                    Alimento = "Pipoca (de micro-ondas)",
                    Tempo = 180,
                    Potencia = 7,
                    CaractereAquecimento = "*",
                    Instrucoes = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.",
                    Customizado = false
                },
                new ProgramaAquecimento
                {
                    Id = 2,
                    Nome = "Leite",
                    Alimento = "Leite",
                    Tempo = 300,
                    Potencia = 5,
                    CaractereAquecimento = "~",
                    Instrucoes = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.",
                    Customizado = false
                },
                new ProgramaAquecimento
                {
                    Id = 3,
                    Nome = "Carnes de boi",
                    Alimento = "Carne em pedaço ou fatias",
                    Tempo = 840,
                    Potencia = 4,
                    CaractereAquecimento = "#",
                    Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.",
                    Customizado = false
                },
                new ProgramaAquecimento
                {
                    Id = 4,
                    Nome = "Frango",
                    Alimento = "Frango (qualquer corte)",
                    Tempo = 480,
                    Potencia = 7,
                    CaractereAquecimento = "+",
                    Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.",
                    Customizado = false
                },
                new ProgramaAquecimento
                {
                    Id = 5,
                    Nome = "Feijão",
                    Alimento = "Feijão congelado",
                    Tempo = 480,
                    Potencia = 9,
                    CaractereAquecimento = "@",
                    Instrucoes = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.",
                    Customizado = false
                }
            );
        }
    }
}
