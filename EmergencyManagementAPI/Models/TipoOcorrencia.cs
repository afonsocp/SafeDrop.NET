using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("tipos_ocorrencia")]
    public class TipoOcorrencia
    {
        [Key]
        [Column("id_tipo")]
        public int IdTipo { get; set; }

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public int Id => IdTipo;

        [Required]
        [MaxLength(50)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        // Relacionamento 1:N
        public virtual ICollection<Ocorrencia> Ocorrencias { get; set; } = new List<Ocorrencia>();
    }
}