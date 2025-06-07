import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService, LoginRequest } from '../auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  showPassword = false;
  errorMenssagem = '';
  sucessoMenssagem = '';
  returnUrl = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // Redirecionar se já estiver logado
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/microondas']);
    }
  }

  ngOnInit(): void {
    this.initForm();
    this.getReturnUrl();
  }

  private initForm(): void {
    this.loginForm = this.formBuilder.group({
      nome: ['', [
        Validators.required
      ]],
      senha: ['', [
        Validators.required
      ]]
    });
  }

  private getReturnUrl(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/microondas';
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.performLogin();
    } else {
      this.markFormGroupTouched();
    }
  }

  private performLogin(): void {
    this.isLoading = true;
    this.errorMenssagem = '';
    this.sucessoMenssagem = '';

    const loginRequest: LoginRequest = {
      nome: this.loginForm.get('nome')?.value,
      senha: this.loginForm.get('senha')?.value
    };

    this.authService.login(loginRequest)
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          
          if (response.sucesso) {
            this.sucessoMenssagem = response.mensagem;
            
            // Pequeno delay para mostrar mensagem de sucesso
            setTimeout(() => {
              this.router.navigate([this.returnUrl]);
            }, 1000);
          } else {
            this.errorMenssagem = response.mensagem || 'Erro na autenticação';
          }
        },
        error: (error) => {
          this.isLoading = false;
          console.error('Erro no login:', error);
          
          if (error.status === 401) {
            this.errorMenssagem = 'Credenciais inválidas';
          } else if (error.status === 0) {
            this.errorMenssagem = 'Erro de conexão. Verifique sua internet.';
          } else {
            this.errorMenssagem = error.error?.mensagem || 'Erro interno do servidor';
          }
        }
      });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  // Métodos para facilitar acesso aos controles no template
  get nome() { return this.loginForm.get('nome'); }
  get senha() { return this.loginForm.get('senha'); }

  // Limpar mensagens quando usuário começar a digitar
  onInputChange(): void {
    if (this.errorMenssagem) {
      this.errorMenssagem = '';
    }
    if (this.sucessoMenssagem) {
      this.sucessoMenssagem = '';
    }
  }
}