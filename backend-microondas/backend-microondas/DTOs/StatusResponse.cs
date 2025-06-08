using backend_microondas.Models;

namespace backend_microondas.DTOs
{
    public class StatusResponse
    {
        public bool EmAndamento { get; set; }
        public bool Pausado { get; set; }
        public int TempoRestante { get; set; }
        public int Potencia { get; set; }
        public string StringProcessamento { get; set; } = "";
        public bool Concluido { get; set; }
        public bool ProgramaPreDefinido { get; set; }
        public ProgramaAquecimentoDTO Programa { get; set; }
    }

}
