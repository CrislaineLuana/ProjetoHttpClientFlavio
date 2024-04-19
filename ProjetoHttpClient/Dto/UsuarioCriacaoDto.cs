using System.ComponentModel.DataAnnotations;

namespace ProjetoHttpClient.Dto
{
    public class UsuarioCriacaoDto
    {
        [Required(ErrorMessage = "Digite o usuário")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Digite o nome")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Digite o sobrenome")]
        public string Sobrenome { get; set; }
        [Required(ErrorMessage = "Digite o email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite a senha")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Digite a confirmação de senha")]
        public string ConfirmaSenha { get; set; }

    }
}
