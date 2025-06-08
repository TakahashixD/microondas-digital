import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Router } from '@angular/router';

// Interfaces para tipagem
export interface LoginRequest {
  nome: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  nome: string;
  expiresAt: string;
  sucesso: boolean;
  mensagem: string;
}

export interface Usuario {
  nome: string;
  token: string;
  expiresAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:7000/api/auth'; 
  private tokenKey = 'auth_token';
  private usuarioKey = 'auth_usuario';
  
  usuarioAtivo = signal<Usuario | null>(this.getUsuario());

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Verificar se o token ainda é válido na inicialização
    this.validarToken();
  }

  // Fazer login
  login(login: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, login)
      .pipe(
        tap(response => {
          if (response.sucesso && response.token) {
            this.setSession(response);
          }
        })
      );
  }

  // Fazer logout
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.usuarioKey);
    this.usuarioAtivo.set(null);
    this.router.navigate(['/login']);
  }

  // Verificar se está autenticado
  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const usuario = this.getUsuario();
    if (!usuario) return false;

    // Verificar se o token não expirou
    return new Date() < new Date(usuario.expiresAt);
  }

  // Obter token
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Obter usuário atual
  getUsuario(): Usuario | null {
    const usuarioStr = localStorage.getItem(this.usuarioKey);
    if (usuarioStr) {
      try {
        return JSON.parse(usuarioStr);
      } catch {
        return null;
      }
    }
    return null;
  }

  // Definir sessão após login bem-sucedido
  private setSession(authResult: LoginResponse): void {
    const expiresAt = new Date(authResult.expiresAt);
    
    const usuario: Usuario = {
      nome: authResult.nome,
      token: authResult.token,
      expiresAt: expiresAt
    };

    localStorage.setItem(this.tokenKey, authResult.token);
    localStorage.setItem(this.usuarioKey, JSON.stringify(usuario));
    
    this.usuarioAtivo.set(usuario);
  }

  // Verificar validade do token
  private validarToken(): void {
    if (!this.isAuthenticated()) {
      this.logout();
    }
  }

  // Obter headers com autorização
  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }
}