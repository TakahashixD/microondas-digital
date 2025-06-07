import { Component, OnInit, OnDestroy } from '@angular/core';
import { MicroondasService, MicroondasRequest, StatusResponse, ProgramaAquecimento } from './microondas.service';
import { interval, Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ModalComponent } from '../modal-success/modal.component';
import { ModalErroComponent } from '../modal-error/modal.component';

@Component({
  selector: 'app-microondas',
  templateUrl: './microondas.component.html',
  imports: [FormsModule, ModalComponent, ModalErroComponent],
  styleUrls: ['./microondas.component.scss']
})
export class MicroondasComponent implements OnInit, OnDestroy {
  tempo: string = '';
  potencia: string = '';
  mensagem: string = '';
  stringProcessamento: string = '';
  emAndamento: boolean = false;
  pausado: boolean = false;
  tempoRestante: number = 0;
  potenciaAtual: number = 0;
  concluido: boolean = false;

  // Novos campos para programas
  programaSelecionado: ProgramaAquecimento | null = null;
  programaPreDefinido: boolean = false;

  private statusSubscription?: Subscription;

  // Programas
  listProgramas: ProgramaAquecimento[] = [];

  //Verificar estado do modal
  modalNovoPrograma: boolean = false;
  mostrarModalSucesso: boolean = false;

  //Modal error
  modalErroVisivel: boolean = false;
  mensagemErroAtual: string = '';

  constructor(private microondasService: MicroondasService) { }

  ngOnInit(): void {
    this.buscarProgramas();
    this.iniciarMonitoramento();
  }

  ngOnDestroy(): void {
    this.statusSubscription?.unsubscribe();
  }

  // Método para buscar todos os programas
  buscarProgramas(): void {
    this.microondasService.getProgramasAquecimento().subscribe({
      next: (response) => {
        this.listProgramas = response;
        this.microondasService.listProgramaModal.set(this.listProgramas);

      },
      error: (error) => {
        this.mensagem = "Erro ao consultar os programas."
      }
    });
  }

  adicionarProgramaCustomizado(programa: ProgramaAquecimento): void {
    this.microondasService.adicionarProgramasAquecimento(programa).subscribe({
      next: (response) => {

      },
      error: (error) => {
        this.mensagem = "Erro ao adicionar"
      }
    });
  }
  // Método para selecionar programa pré-definido
  selecionarPrograma(programa: ProgramaAquecimento): void {
    if (!this.emAndamento && !this.pausado) {
      this.programaSelecionado = programa;
      this.programaPreDefinido = true;
      this.tempo = programa.tempo.toString();
      this.potencia = programa.potencia.toString();
      this.mensagem = `Programa "${programa.nome}" selecionado. ${programa.alimento} - ${this.formatarTempo(programa.tempo.toString())} a potência ${programa.potencia}.`;
    }
  }

  // Método para limpar seleção de programa
  limparPrograma(): void {
    if (!this.emAndamento && !this.pausado) {
      this.programaSelecionado = null;
      this.programaPreDefinido = false;
      this.tempo = '';
      this.potencia = '';
      this.mensagem = '';
    }
    this.pausarCancelar();
  }

  adicionarDigito(digito: string): void {
    if (!this.emAndamento && !this.pausado && !this.programaPreDefinido) {
      if (this.tempo.length < 3) {
        this.tempo += digito;
      }
    }
  }

  adicionarDigitoPotencia(digito: string): void {
    if (!this.emAndamento && !this.pausado && !this.programaPreDefinido) {
      if (this.potencia.length < 2) {
        this.potencia += digito;
      }
    }
  }

  limparTempo(): void {
    if (!this.emAndamento && !this.pausado && !this.programaPreDefinido) {
      this.tempo = '';
    }
  }

  limparPotencia(): void {
    if (!this.emAndamento && !this.pausado && !this.programaPreDefinido) {
      this.potencia = '';
    }
  }

  iniciarAquecimento(): void {
    const request: MicroondasRequest = {
      tempo: this.tempo ? parseInt(this.tempo) : undefined,
      potencia: this.potencia ? parseInt(this.potencia) : undefined,
      programaId: this.programaSelecionado?.id
    };

    this.microondasService.iniciarAquecimento(request).subscribe({
      next: (response) => {
        this.mensagem = response.mensagem || '';
        if (response.sucesso) {
          this.emAndamento = response.emAndamento;
          this.pausado = response.pausado;
          this.tempoRestante = response.tempoRestante;
          this.programaPreDefinido = response.programaPreDefinido;
          if (response.programa) {
            this.programaSelecionado = response.programa;
          }
        }
      },
      error: (error) => {
        this.mensagem = 'Erro ao iniciar aquecimento: ' + error.message;
      }
    });
  }

  inicioRapido(): void {
    if (!this.programaPreDefinido) {
      this.microondasService.inicioRapido().subscribe({
        next: (response) => {
          this.mensagem = response.mensagem || '';
          if (response.sucesso) {
            this.emAndamento = response.emAndamento;
            this.pausado = response.pausado;
            this.tempoRestante = response.tempoRestante;
            this.tempo = '30';
            this.potencia = '10';
            this.programaPreDefinido = false;
            this.programaSelecionado = null;
          }
        },
        error: (error) => {
          this.mensagem = 'Erro no início rápido: ' + error.message;
        }
      });
    }
  }

  pausarCancelar(): void {
    this.microondasService.pausarCancelar().subscribe({
      next: (response) => {
        this.mensagem = response.mensagem || '';
        if (response.sucesso) {
          this.emAndamento = response.emAndamento;
          this.pausado = response.pausado;
          this.tempoRestante = response.tempoRestante;

          // Se cancelou ou limpou, limpar campos
          if (!response.emAndamento && !response.pausado && response.tempoRestante === 0) {
            this.tempo = '';
            this.potencia = '';
            this.stringProcessamento = '';
            this.concluido = false;
            this.programaPreDefinido = false;
            this.programaSelecionado = null;
          }
        }
      },
      error: (error) => {
        this.mensagem = 'Erro ao pausar/cancelar: ' + error.message;
      }
    });
  }

  formatarTempo(tempo: string): string {
    return this.microondasService.formatarTempo(tempo);
  }

  private iniciarMonitoramento(): void {
    this.statusSubscription = interval(1000).subscribe(() => {
      if (this.tempoRestante > 0) {
        this.microondasService.obterStatus().subscribe({
          next: (status: StatusResponse) => {
            this.emAndamento = status.emAndamento;
            this.pausado = status.pausado;
            this.tempoRestante = status.tempoRestante;
            this.potenciaAtual = status.potencia;
            this.stringProcessamento = status.stringProcessamento;
            this.concluido = status.concluido;
            this.programaPreDefinido = status.programaPreDefinido;
            if (status.programa) {
              this.programaSelecionado = status.programa;
            }
          },
          error: (error) => {
            console.error('Erro ao obter status:', error);
          }
        });
      }
    });
  }

  getBotaoPausarTexto(): string {
    if (!this.emAndamento && !this.pausado) {
      return 'Limpar';
    }
    return this.pausado ? 'Cancelar' : 'Pausar';
  }

  getBotaoIniciarTexto(): string {
    if (this.pausado) {
      return 'Continuar';
    }
    // Para programas pré-definidos, não permite +30s
    if (this.emAndamento && this.programaPreDefinido) {
      return 'Em Andamento';
    }
    return this.emAndamento ? '+30s' : 'Iniciar';
  }

  // Verificar se os controles devem estar desabilitados
  isControlesDesabilitados(): boolean {
    return this.emAndamento || this.pausado || this.programaPreDefinido;
  }

  // Verificar se início rápido deve estar desabilitado
  isInicioRapidoDesabilitado(): boolean {
    return this.emAndamento || this.pausado || this.programaPreDefinido;
  }


  abrirModalNovoPrograma(): void {
    this.modalNovoPrograma = true;
  }

  fecharModalNovoPrograma(): void {
    this.modalNovoPrograma = false;
  }

  adicionarNovoPrograma(novoPrograma: ProgramaAquecimento): void {
    this.microondasService.adicionarProgramasAquecimento(novoPrograma).subscribe({
      next: (response) => {
        console.log(response);
        this.listProgramas.push(novoPrograma);
        this.mostrarModalSucesso = true;
      },
      error: (error) => {
        this.mostrarErro(error.error.erro);
        this.mostrarModalSucesso = false;
        
      }
    })
  }

  mostrarErro(mensagem: string): void {
    this.mensagemErroAtual = mensagem;
    this.modalErroVisivel = true;
  }

  fecharModalErro(): void {
    this.modalErroVisivel = false;
    this.mensagemErroAtual = '';
  }

  tentarNovamenteAcao(): void {
    this.fecharModalErro();
    this.abrirModalNovoPrograma();
  }

}