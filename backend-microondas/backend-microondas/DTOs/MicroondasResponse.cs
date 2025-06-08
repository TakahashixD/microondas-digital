using backend_microondas.Models;

namespace backend_microondas.DTOs
{
    public class MicroondasResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public int TempoFinal { get; set; }
        public int PotenciaFinal { get; set; }
        public string StringProcessamento { get; set; }
        public bool EmAndamento { get; set; }
        public bool Pausado { get; set; }
        public int TempoRestante { get; set; }
        public bool ProgramaPreDefinido { get; set; }
        public ProgramaAquecimentoDTO Programa { get; set; }
    }
}
