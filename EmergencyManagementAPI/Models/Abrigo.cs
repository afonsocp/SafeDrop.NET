using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("abrigos")]
    public class Abrigo
    {
        [Key]
        [Column("id_abrigo")]
        public int IdAbrigo { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("endereco")]
        public string? Endereco { get; set; }

        [Required]
        [Column("capacidade_total")]
        public int CapacidadeTotal { get; set; }

        [Required]
        [Column("vagas_disponiveis")]
        public int VagasDisponiveis { get; set; }

        [MaxLength(20)]
        [Column("telefone")]
        public string? Telefone { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = string.Empty; // disponivel, lotado, inativo

        // Relacionamento 1:N
        public virtual ICollection<CheckinAbrigo> CheckinsAbrigos { get; set; } = new List<CheckinAbrigo>();
    }
}