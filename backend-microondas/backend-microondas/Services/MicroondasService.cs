using backend_microondas.Data;
using backend_microondas.DTOs;
using backend_microondas.Exceptions;
using backend_microondas.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace backend_microondas.Services
{
    public class MicroondasService
    {
        private readonly DataContext _context;

        private static bool _emAndamento = false;
        private static bool _pausado = false;
        private static int _tempoRestante = 0;
        private static int _potencia = 0;
        private static int _tempoTotalOriginal = 0;
        private static Timer _timer;
        private static string _stringProcessamento = "";
        private static bool _programa = false;
        private static ProgramaAquecimentoDTO _programaAtual = null;
        private static string _caractereAquecimento = ".";

        public MicroondasService(DataContext context)
        {
            _context = context;
        }

        private void LogException(Exception ex)
        {
            string logFilePath = "exceptions.log"; // Caminho do arquivo de log
            var logMessage = new StringBuilder();
            logMessage.AppendLine($"Data: {DateTime.Now}");
            logMessage.AppendLine($"Exception: {ex.Message}");
            logMessage.AppendLine($"Inner Exception: {ex.InnerException?.Message}");
            logMessage.AppendLine($"Stack Trace: {ex.StackTrace}");
            logMessage.AppendLine("--------------------------------------------------");
            File.AppendAllText(logFilePath, logMessage.ToString());
        }

        public List<ProgramaAquecimentoDTO> ObterProgramas()
        {
            try
            {
                var programa = _context.ProgramasAquecimento.ToList();
                if (programa == null) throw new MicroondasException("Erro ao obter programas.");
                return programa.Select(p => new ProgramaAquecimentoDTO(p)).ToList();
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        public ProgramaAquecimentoDTO ObterProgramaPorId(int id)
        {
            try
            {
                var programa = _context.ProgramasAquecimento.FirstOrDefault(p => p.Id == id);
                if (programa == null) throw new MicroondasException("Programa não encontrado");
                return new ProgramaAquecimentoDTO(programa);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        public void AdicionarProgramaCustomizado(ProgramaAquecimentoDTO programa)
        {
            try
            {
                var programas = _context.ProgramasAquecimento.ToList();
                foreach (ProgramaAquecimento p in programas)
                    if (p.CaractereAquecimento == programa.CaractereAquecimento ||
                        programa.CaractereAquecimento == ".")
                        throw new MicroondasException("Caracter já cadastrado");
                _context.ProgramasAquecimento.Add(new ProgramaAquecimento(programa));
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        public MicroondasResponse IniciarAquecimento(MicroondasRequest request)
        {
                var response = new MicroondasResponse();
            try
            {

                // Verificar se é programa
                if (request.ProgramaId.HasValue)
                {
                    var programa = ObterProgramaPorId(request.ProgramaId.Value);
                    if (programa == null)
                    {
                        response.Sucesso = false;
                        response.Mensagem = "Programa não encontrado.";
                        return response;
                    }

                    // Para programas, não permite adição de tempo se já estiver em andamento
                    if (_emAndamento && _programa)
                    {
                        response.Sucesso = false;
                        response.Mensagem = "Não é possível adicionar tempo em programas.";
                        return response;
                    }

                    // Se pausado e é o mesmo programa, retoma
                    if (_pausado && _programa && _programaAtual?.Id == programa.Id)
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
                    _programa = true;
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
                _programa = false;
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
                if (_emAndamento && !_programa && !_pausado)
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
                if (_pausado && !_programa)
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
            catch (MicroondasException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }

        }

        public MicroondasResponse PausarCancelar()
        {
            var response = new MicroondasResponse();

            try
            {
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
                    response.ProgramaPreDefinido = _programa;
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
            catch(Exception ex) 
            {
                LogException(ex);
                throw;
            }
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
                ProgramaPreDefinido = _programa,
                Programa = _programaAtual
            };
        }

        private void IniciarTimer()
        {   
            //evitar memory leak, finaliza timer
            _timer?.Dispose();
            _timer = new Timer(AtualizarAquecimento, null, 0, 1000);
        }

        private void AtualizarAquecimento(object state)
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
            _programa = false;
            _programaAtual = null;
            _caractereAquecimento = ".";
            _timer?.Dispose();
        }
    }
}