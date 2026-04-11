using System;

namespace USUARIOminimalSolution.Domain.Entities
{
    public class Usuario
    {
        
        public int Id_Usuario { get; set; } // ID_USUARIO NUMBER
        public string Nome { get; private set; } // NOME VARCHAR2(100)
        public string Email { get; private set; } // EMAIL VARCHAR2(150)
        public string Senha { get; private set; } // SENHA VARCHAR2(200)
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        
        protected Usuario() { }

        public Usuario(string nome, string email, string senha)
        {
            SetNome(nome);
            SetEmail(email);
            SetSenha(senha);
        }

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome não pode ser vazio");
            Nome = nome.Trim();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) throw new ArgumentException("Email inválido");
            Email = email.Trim().ToLowerInvariant();
        }

        public void SetSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha)) throw new ArgumentException("Senha inválida");
            Senha = senha;
        }
    }
}
