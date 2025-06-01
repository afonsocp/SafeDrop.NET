using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; } = string.Empty; // cidadao, voluntario, orgao_publico

        // Propriedades adicionais para compatibilidade com as páginas
        [MaxLength(15)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        [MaxLength(200)]
        [Column("endereco")]
        public string? Endereco { get; set; }

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public string Tipo => TipoUsuario;

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Relacionamentos 1:N
        public virtual ICollection<Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia>();
        public virtual ICollection<CheckinAbrigo> CheckinsAbrigos { get; set; } = new List<CheckinAbrigo>();
    }
}