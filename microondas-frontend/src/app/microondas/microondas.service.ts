import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface MicroondasRequest {
  tempo?: number;
  potencia?: number;
  programaId?: number;
}

export interface MicroondasResponse {
  sucesso: boolean;
  mensagem?: string;
  tempoFinal: number;
  potenciaFinal: number;
  stringProcessamento?: string;
  emAndamento: boolean;
  pausado: boolean;
  tempoRestante: number;
  programaPreDefinido: boolean;
  programa?: ProgramaAquecimento;
}

export interface StatusResponse {
  emAndamento: boolean;
  pausado: boolean;
  tempoRestante: number;
  potencia: number;
  stringProcessamento: string;
  concluido: boolean;
  programaPreDefinido: boolean;
  programa?: ProgramaAquecimento;
}

export interface ProgramaAquecimento {
  id?: number;
  nome: string;
  alimento: string;
  tempo: number;
  potencia: number;
  caractereAquecimento: string;
  instrucoes: string;
  customizado: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MicroondasService {
  private apiUrl = 'http://localhost:7000/api/microondas'; // Ajuste conforme sua API

  listProgramaModal = signal<ProgramaAquecimento[]>([]);
  headers

  constructor(private http: HttpClient, private authService: AuthService) { 
    this.headers =this.authService.getAuthHeaders();
  }

  formatarTempo(tempo: string | undefined): string {
    if(!tempo) return "tempo vazio"
    const segundos = parseInt(tempo);
    if (segundos >= 60) {
      const minutos = Math.floor(segundos / 60);
      const segundosRestantes = segundos % 60;
      return `${minutos}:${segundosRestantes.toString().padStart(2, '0')}`;
    }
    return segundos.toString() + 's';
  }

  getProgramasAquecimento(): Observable<ProgramaAquecimento[]> {
    return this.http.get<ProgramaAquecimento[]>(`${this.apiUrl}/programas`);
  }

  obterStatus(): Observable<StatusResponse> {
    return this.http.get<StatusResponse>(`${this.apiUrl}/status`);
  }

  iniciarAquecimento(request: MicroondasRequest): Observable<MicroondasResponse> {
    return this.http.post<MicroondasResponse>(`${this.apiUrl}/iniciar`, request);
  }

  pausarCancelar(): Observable<MicroondasResponse> {
    return this.http.post<MicroondasResponse>(`${this.apiUrl}/pausar-cancelar`, null);
  }

  inicioRapido(): Observable<MicroondasResponse> {
    return this.http.post<MicroondasResponse>(`${this.apiUrl}/inicio-rapido`, null);
  }

  adicionarProgramasAquecimento(request: ProgramaAquecimento): Observable<ProgramaAquecimento> {
    return this.http.post<ProgramaAquecimento>(`${this.apiUrl}/adicionar`, request);
  }
}