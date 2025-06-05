namespace microondas_backend.Models
{
    public class ProgramaAquecimento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Alimento { get; set; } = "";
        public int Tempo { get; set; }
        public int Potencia { get; set; }
        public string CaractereAquecimento { get; set; } = "";
        public string Instrucoes { get; set; } = "";
    }
}
