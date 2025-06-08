namespace backend_microondas.Migrations
{
    using backend_microondas.Configs;
    using backend_microondas.Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<backend_microondas.Data.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(backend_microondas.Data.DataContext context)
        {
            if (!context.Usuarios.Any(u => u.Nome == "admin"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nome = "admin",
                    Senha = CryptoService.HashSenha("admin")
                });
            }

            var programasExistentes = context.ProgramasAquecimento.Select(p => p.Nome).ToList();
            var programasParaAdicionar = new[]
            {
                new ProgramaAquecimento
                {
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
                    Nome = "Feijão",
                    Alimento = "Feijão congelado",
                    Tempo = 480,
                    Potencia = 9,
                    CaractereAquecimento = "@",
                    Instrucoes = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.",
                    Customizado = false
                }
            };

            foreach (var programa in programasParaAdicionar)
            {
                if (!programasExistentes.Contains(programa.Nome))
                {
                    context.ProgramasAquecimento.Add(programa);
                }
            }

            context.SaveChanges();
            base.Seed(context);
        }
    }
}