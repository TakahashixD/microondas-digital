using backend_microondas.Data;
using backend_microondas.DTOs;
using backend_microondas.Exceptions;
using backend_microondas.Models;
using backend_microondas.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace backend_microondas.UnitTests
{
    public class MicroondasServiceTests : IDisposable
    {
        private readonly Mock<DataContext> _mockContext;
        private readonly Mock<DbSet<ProgramaAquecimento>> _mockSet;
        private readonly MicroondasService _service;
        private readonly List<ProgramaAquecimento> _programasAquecimento;

        public MicroondasServiceTests()
        {

            _programasAquecimento = new List<ProgramaAquecimento>
            {
                new ProgramaAquecimento { Id = 1, Nome = "Palmito", Tempo = 180, Potencia = 7, CaractereAquecimento = "P" },
                new ProgramaAquecimento { Id = 2, Nome = "Lasanha", Tempo = 300, Potencia = 9, CaractereAquecimento = "L" },
                new ProgramaAquecimento { Id = 3, Nome = "Calabresa", Tempo = 900, Potencia = 4, CaractereAquecimento = "C" }
            };

            _mockContext = new Mock<DataContext>();
            _mockSet = new Mock<DbSet<ProgramaAquecimento>>();

            _mockContext.CallBase = true;

            var queryable = _programasAquecimento.AsQueryable();
            _mockSet.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockContext.Setup(x => x.ProgramasAquecimento).Returns(_mockSet.Object);
            _service = new MicroondasService(_mockContext.Object);
        }

        public void Dispose()
        {
            _service?.PausarCancelar();
        }

        [Fact]
        public void ObterProgramas_DeveRetornarListaDeProgramas()
        {
            
            var resultado = _service.ObterProgramas();

            
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count);
            // Corrigir os nomes esperados conforme os dados mockados
            Assert.Equal("Palmito", resultado[0].Nome);
            Assert.Equal("Lasanha", resultado[1].Nome);
            Assert.Equal("Calabresa", resultado[2].Nome);
        }

        [Fact]
        public void ObterProgramas_ComListaVazia_DeveRetornarListaVazia()
        {
            
            var listaVazia = new List<ProgramaAquecimento>();
            var mockSetVazio = new Mock<DbSet<ProgramaAquecimento>>();
            var queryableVazio = listaVazia.AsQueryable();

            mockSetVazio.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.Provider).Returns(queryableVazio.Provider);
            mockSetVazio.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.Expression).Returns(queryableVazio.Expression);
            mockSetVazio.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.ElementType).Returns(queryableVazio.ElementType);
            mockSetVazio.As<IQueryable<ProgramaAquecimento>>().Setup(m => m.GetEnumerator()).Returns(queryableVazio.GetEnumerator());

            _mockContext.Setup(x => x.ProgramasAquecimento).Returns(mockSetVazio.Object);
            var serviceComListaVazia = new MicroondasService(_mockContext.Object);

            
            var resultado = serviceComListaVazia.ObterProgramas();

            
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public void ObterProgramaPorId_ComIdValido_DeveRetornarPrograma()
        {
            
            var resultado = _service.ObterProgramaPorId(1);

            
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Palmito", resultado.Nome); 
            Assert.Equal(180, resultado.Tempo);
            Assert.Equal(7, resultado.Potencia);
            Assert.Equal("P", resultado.CaractereAquecimento);
        }

        [Fact]
        public void ObterProgramaPorId_ComIdInvalido_DeveLancarExcecao()
        {
             
            Assert.Throws<MicroondasException>(() => _service.ObterProgramaPorId(999));
        }

        [Fact]
        public void AdicionarProgramaCustomizado_ComDadosValidos_DeveAdicionarPrograma()
        {
            
            var novoPrograma = new ProgramaAquecimentoDTO
            {
                Nome = "Chocolate",
                Tempo = 60,
                Potencia = 5,
                CaractereAquecimento = "H"
            };

            _mockSet.Setup(m => m.Add(It.IsAny<ProgramaAquecimento>())).Verifiable();
            _mockContext.Setup(x => x.SaveChanges()).Returns(1).Verifiable();

            
            _service.AdicionarProgramaCustomizado(novoPrograma);

            
            _mockContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AdicionarProgramaCustomizado_ComCaractereExistente_DeveLancarExcecao()
        {
            
            var programaComCaractereExistente = new ProgramaAquecimentoDTO
            {
                Nome = "Teste",
                Tempo = 60,
                Potencia = 5,
                CaractereAquecimento = "P"
            };

             
            Assert.Throws<MicroondasException>(() => _service.AdicionarProgramaCustomizado(programaComCaractereExistente));
        }

        [Fact]
        public void AdicionarProgramaCustomizado_ComCaracterePonto_DeveLancarExcecao()
        {
            
            var programaComPonto = new ProgramaAquecimentoDTO
            {
                Nome = "Teste",
                Tempo = 60,
                Potencia = 5,
                CaractereAquecimento = "."
            };

             
            Assert.Throws<MicroondasException>(() => _service.AdicionarProgramaCustomizado(programaComPonto));
        }

        [Fact]
        public void IniciarAquecimento_InicioRapido_DeveUsarValoresPadrao()
        {
            
            var request = new MicroondasRequest(); // Sem tempo nem potência

            
            var resultado = _service.IniciarAquecimento(request);

            
            Assert.True(resultado.Sucesso);
            Assert.Equal(30, resultado.TempoFinal);
            Assert.Equal(10, resultado.PotenciaFinal);
            Assert.True(resultado.EmAndamento);
            Assert.Equal(30, resultado.TempoRestante);
        }

        [Fact]
        public void IniciarAquecimento_ComTempoEPotenciaValidos_DeveIniciarCorretamente()
        {
            
            var request = new MicroondasRequest
            {
                Tempo = 60,
                Potencia = 5
            };

            
            var resultado = _service.IniciarAquecimento(request);

            
            Assert.True(resultado.Sucesso);
            Assert.Equal(60, resultado.TempoFinal);
            Assert.Equal(5, resultado.PotenciaFinal);
            Assert.True(resultado.EmAndamento);
            Assert.Equal(60, resultado.TempoRestante);
            Assert.Equal("Aquecimento iniciado.", resultado.Mensagem);
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(-1, 5)]
        [InlineData(121, 5)]
        [InlineData(200, 5)]
        public void IniciarAquecimento_ComTempoInvalido_DeveRetornarErro(int tempo, int potencia)
        {
            
            var request = new MicroondasRequest
            {
                Tempo = tempo,
                Potencia = potencia
            };

            
            var resultado = _service.IniciarAquecimento(request);

            
            Assert.False(resultado.Sucesso);
            Assert.Equal("Tempo deve estar entre 1 segundo e 2 minutos (120 segundos).", resultado.Mensagem);
        }

        [Theory]
        [InlineData(60, 0)]
        [InlineData(60, -1)]
        [InlineData(60, 11)]
        [InlineData(60, 15)]
        public void IniciarAquecimento_ComPotenciaInvalida_DeveRetornarErro(int tempo, int potencia)
        {
            
            var request = new MicroondasRequest
            {
                Tempo = tempo,
                Potencia = potencia
            };

            
            var resultado = _service.IniciarAquecimento(request);

            
            Assert.False(resultado.Sucesso);
            Assert.Equal("Potência deve estar entre 1 e 10.", resultado.Mensagem);
        }

        [Theory]
        [InlineData(1, "Palmito", 180, 7, "P")]
        [InlineData(2, "Lasanha", 300, 9, "L")]
        [InlineData(3, "Calabresa", 900, 4, "C")]
        public void IniciarAquecimento_ComDiferentesProgramas_DeveIniciarCorretamente(
            int programaId, string nomeEsperado, int tempoEsperado, int potenciaEsperada, string caractereEsperado)
        {
            
            var request = new MicroondasRequest { ProgramaId = programaId };

            
            var resultado = _service.IniciarAquecimento(request);

            
            Assert.True(resultado.Sucesso);
            Assert.Equal($"Programa {nomeEsperado} iniciado.", resultado.Mensagem);
            Assert.Equal(tempoEsperado, resultado.TempoFinal);
            Assert.Equal(potenciaEsperada, resultado.PotenciaFinal);
            Assert.True(resultado.ProgramaPreDefinido);
            Assert.Equal(nomeEsperado, resultado.Programa.Nome);
            Assert.Equal(caractereEsperado, resultado.Programa.CaractereAquecimento);
        }

        [Fact]
        public void ObterStatus_InicialmenteLimpo_DeveRetornarStatusInicial()
        {
            
            var status = _service.ObterStatus();

            
            Assert.False(status.EmAndamento);
            Assert.False(status.Pausado);
            Assert.Equal(0, status.TempoRestante);
            Assert.Equal(0, status.Potencia);
            Assert.Equal("", status.StringProcessamento);
            Assert.False(status.Concluido);
            Assert.False(status.ProgramaPreDefinido);
            Assert.Null(status.Programa);
        }
    }
}