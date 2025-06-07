using microondas_backend.Data;
using microondas_backend.Exceptions;
using microondas_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace microondas_backend.Services
{
    public class MicroondasService
    {
        private readonly DataContext _context;

        private static bool _emAndamento = false;
        private static bool _pausado = false;
        private static int _tempoRestante = 0;
        private static int _potencia = 0;
        private static int _tempoTotalOriginal = 0;
        private static Timer? _timer;
        private static string _stringProcessamento = "";
        private static bool _programaPreDefinido = false;
        private static ProgramaAquecimento? _programaAtual = null;
        private static string _caractereAquecimento = ".";

        public MicroondasService(DataContext context)
        {
            _context = context;
        }

        public List<ProgramaAquecimento> ObterProgramasPreDefinidos()
        {
            return _context.ProgramasAquecimento.ToList();
        }

        public ProgramaAquecimento? ObterProgramaPorId(int id)
        {
            return _context.ProgramasAquecimento.FirstOrDefault(p => p.Id == id);
        }

        public void AdicionarProgramaCustomizado(ProgramaAquecimento programa)
        {
            var programas = _context.ProgramasAquecimento.ToList();
            foreach (ProgramaAquecimento p in programas)
                if (p.CaractereAquecimento == programa.CaractereAquecimento ||
                    programa.CaractereAquecimento == ".")
                    throw new MicroondasException("Caracter já cadastrado");
            _context.ProgramasAquecimento.Add(programa);
            _context.SaveChanges();
        }

        public MicroondasResponse IniciarAquecimento(MicroondasRequest request)
        {
            var response = new MicroondasResponse();

            // Verificar se é programa pré-definido
            if (request.ProgramaId.HasValue)
            {
                var programa = ObterProgramaPorId(request.ProgramaId.Value);
                if (programa == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Programa não encontrado.";
                    return response;
                }

                // Para programas pré-definidos, não permite adição de tempo se já estiver em andamento
                if (_emAndamento && _programaPreDefinido)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Não é possível adicionar tempo em programas pré-definidos.";
                    return response;
                }

                // Se pausado e é o mesmo programa, retoma
                if (_pausado && _programaPreDefinido && _programaAtual?.Id == programa.Id)
                {
                    _pausado = false;
                    _emAndamento = true;
                    IniciarTimer();

                    response.Sucesso = true;
                    response.Mensagem = "Aquecimento retomado.";
                    response.EmAndamento = true;
                    response.TempoRestante = _tempoRestante;
                    response.ProgramaPreDefinido = true;
                    response.Programa = programa;
                    return response;
                }

                // Novo aquecimento com programa
                _tempoRestante = programa.Tempo;
                _tempoTotalOriginal = programa.Tempo;
                _potencia = programa.Potencia;
                _programaPreDefinido = true;
                _programaAtual = programa;
                _caractereAquecimento = programa.CaractereAquecimento;
                _emAndamento = true;
                _pausado = false;
                _stringProcessamento = "";

                IniciarTimer();

                response.Sucesso = true;
                response.Mensagem = $"Programa {programa.Nome} iniciado.";
                response.TempoFinal = programa.Tempo;
                response.PotenciaFinal = programa.Potencia;
                response.EmAndamento = true;
                response.TempoRestante = _tempoRestante;
                response.ProgramaPreDefinido = true;
                response.Programa = programa;

                return response;
            }

            // Lógica original para aquecimento manual
            _programaPreDefinido = false;
            _programaAtual = null;
            _caractereAquecimento = ".";

            // Início rápido - sem tempo nem potência
            if (!request.Tempo.HasValue && !request.Potencia.HasValue)
            {
                request.Tempo = 30;
                request.Potencia = 10;
            }

            // Validação de tempo
            int tempo = request.Tempo ?? 30;
            if (tempo < 1 || tempo > 120)
            {
                response.Sucesso = false;
                response.Mensagem = "Tempo deve estar entre 1 segundo e 2 minutos (120 segundos).";
                return response;
            }

            // Validação de potência
            int potencia = request.Potencia ?? 10;
            if (potencia < 1 || potencia > 10)
            {
                response.Sucesso = false;
                response.Mensagem = "Potência deve estar entre 1 e 10.";
                return response;
            }

            // Se já está em andamento (modo manual), adiciona 30 segundos
            if (_emAndamento && !_programaPreDefinido && !_pausado)
            {
                _tempoRestante += 30;
                if (_tempoRestante > 120)
                    _tempoRestante = 120;

                response.Sucesso = true;
                response.Mensagem = "30 segundos adicionados ao aquecimento.";
                response.TempoRestante = _tempoRestante;
                response.EmAndamento = true;
                return response;
            }

            // Se pausado (modo manual), retoma o aquecimento
            if (_pausado && !_programaPreDefinido)
            {
                _pausado = false;
                _emAndamento = true;
                IniciarTimer();

                response.Sucesso = true;
                response.Mensagem = "Aquecimento retomado.";
                response.EmAndamento = true;
                response.TempoRestante = _tempoRestante;
                return response;
            }

            // Novo aquecimento manual
            _tempoRestante = tempo;
            _tempoTotalOriginal = tempo;
            _potencia = potencia;
            _emAndamento = true;
            _pausado = false;
            _stringProcessamento = "";

            IniciarTimer();

            response.Sucesso = true;
            response.Mensagem = "Aquecimento iniciado.";
            response.TempoFinal = tempo;
            response.PotenciaFinal = potencia;
            response.EmAndamento = true;
            response.TempoRestante = _tempoRestante;

            return response;
        }

        public MicroondasResponse PausarCancelar()
        {
            var response = new MicroondasResponse();

            // Se não está em andamento nem pausado, limpa as informações
            if (!_emAndamento && !_pausado)
            {
                LimparInformacoes();
                response.Sucesso = true;
                response.Mensagem = "Informações limpas.";
                return response;
            }

            // Se está em andamento, pausa
            if (_emAndamento && !_pausado)
            {
                _pausado = true;
                _emAndamento = false;
                _timer?.Dispose();

                response.Sucesso = true;
                response.Mensagem = "Aquecimento pausado.";
                response.Pausado = true;
                response.TempoRestante = _tempoRestante;
                response.ProgramaPreDefinido = _programaPreDefinido;
                response.Programa = _programaAtual;
                return response;
            }

            // Se está pausado, cancela
            if (_pausado)
            {
                LimparInformacoes();
                response.Sucesso = true;
                response.Mensagem = "Aquecimento cancelado.";
                return response;
            }

            return response;
        }

        public StatusResponse ObterStatus()
        {
            return new StatusResponse
            {
                EmAndamento = _emAndamento,
                Pausado = _pausado,
                TempoRestante = _tempoRestante,
                Potencia = _potencia,
                StringProcessamento = _stringProcessamento,
                Concluido = !_emAndamento && !_pausado && _tempoRestante == 0 && !string.IsNullOrEmpty(_stringProcessamento),
                ProgramaPreDefinido = _programaPreDefinido,
                Programa = _programaAtual
            };
        }

        private void IniciarTimer()
        {
            _timer?.Dispose();
            _timer = new Timer(AtualizarAquecimento, null, 0, 1000);
        }

        private void AtualizarAquecimento(object? state)
        {
            if (!_emAndamento || _pausado)
                return;

            _tempoRestante--;

            // Atualizar string de processamento
            AtualizarStringProcessamento();

            if (_tempoRestante <= 0)
            {
                _emAndamento = false;
                _stringProcessamento += " Aquecimento concluído";
                _timer?.Dispose();
            }
        }

        private void AtualizarStringProcessamento()
        {
            int segundosDecorridos = _tempoTotalOriginal - _tempoRestante;
            var sb = new StringBuilder();

            for (int i = 0; i < segundosDecorridos; i++)
            {
                for (int j = 0; j < _potencia; j++)
                {
                    sb.Append(_caractereAquecimento);
                }
                if (i < segundosDecorridos - 1)
                    sb.Append(" ");
            }

            _stringProcessamento = sb.ToString();
        }

        private void LimparInformacoes()
        {
            _emAndamento = false;
            _pausado = false;
            _tempoRestante = 0;
            _potencia = 0;
            _stringProcessamento = "";
            _programaPreDefinido = false;
            _programaAtual = null;
            _caractereAquecimento = ".";
            _timer?.Dispose();
        }

        public string FormatarTempo(int segundos)
        {
            if (segundos >= 60)
            {
                int minutos = segundos / 60;
                int segundosRestantes = segundos % 60;
                return $"{minutos}:{segundosRestantes:D2}";
            }
            return segundos.ToString();
        }
    }
}