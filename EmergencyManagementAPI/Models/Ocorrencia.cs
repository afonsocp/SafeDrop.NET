using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("ocorrencias")]
    public class Ocorrencia
    {
        [Key]
        [Column("id_ocorrencia")]
        public int IdOcorrencia { get; set; }

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public int Id => IdOcorrencia;

        [Required(ErrorMessage = "O usuário é obrigatório")]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O tipo de ocorrência é obrigatório")]
        [Column("id_tipo")]
        public int IdTipo { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("latitude")]
        public decimal? Latitude { get; set; }

        [Column("longitude")]
        public decimal? Longitude { get; set; }

        // Propriedade calculada para localização
        [NotMapped]
        public string? Localizacao => Latitude.HasValue && Longitude.HasValue 
            ? $"{Latitude:F6}, {Longitude:F6}" 
            : null;

        [Required]
        [Column("data_ocorrencia")]
        public DateTime DataOcorrencia { get; set; }

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public DateTime DataHora => DataOcorrencia;

        [Required]
        [MaxLength(20)]
        [Column("nivel_risco")]
        public string NivelRisco { get; set; } = string.Empty; // baixo, moderado, alto

        [Required]
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = string.Empty; // em andamento, resolvido

        // Propriedades de navegação - REMOVER [Required] daqui
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("IdTipo")]
        public virtual TipoOcorrencia? TipoOcorrencia { get; set; }

        // Relacionamento com alertas
        public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}