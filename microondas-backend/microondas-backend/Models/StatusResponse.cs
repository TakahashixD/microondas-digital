namespace microondas_backend.Models
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
        public ProgramaAquecimento? Programa { get; set; }
    }

}
