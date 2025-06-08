
using backend_microondas.Models;

namespace backend_microondas.DTOs
{
    public class ProgramaAquecimentoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public int Tempo { get; set; }
        public int Potencia { get; set; }
        public string CaractereAquecimento { get; set; }
        public string Instrucoes { get; set; }
        public bool Customizado { get; set; }

        public ProgramaAquecimentoDTO(int id, string nome, string alimento, int tempo, int potencia, string caractereAquecimento, string instrucoes, bool customizado)
        {
            Id = id;
            Nome = nome;
            Alimento = alimento;
            Tempo = tempo;
            Potencia = potencia;
            CaractereAquecimento = caractereAquecimento;
            Instrucoes = instrucoes;
            Customizado = customizado;
        }

        public ProgramaAquecimentoDTO(ProgramaAquecimento programa)
        {
            Id = programa.Id;
            Nome = programa.Nome;
            Alimento = programa.Alimento;
            Tempo = programa.Tempo;
            Potencia = programa.Potencia;
            CaractereAquecimento = programa.CaractereAquecimento;
            Instrucoes = programa.Instrucoes;
            Customizado = programa.Customizado;
        }
    }
}
