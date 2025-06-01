using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("alertas")]
    public class Alerta
    {
        [Key]
        [Column("id_alerta")]
        public int IdAlerta { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        [Column("mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("nivel_urgencia")]
        public string NivelUrgencia { get; set; } = string.Empty; // baixa, media, alta

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public string TipoAlerta => NivelUrgencia;

        [Column("data_emissao")]
        public DateTime DataEmissao { get; set; } = DateTime.Now;

        // Propriedade calculada para compatibilidade
        [NotMapped]
        public DateTime DataHora => DataEmissao;

        [MaxLength(100)]
        [Column("fonte")]
        public string Fonte { get; set; } = "Sistema";

        [Column("id_ocorrencia")]
        public int IdOcorrencia { get; set; }

        [Column("latitude")]
        public decimal? Latitude { get; set; }

        [Column("longitude")]
        public decimal? Longitude { get; set; }

        [Column("raio_afetado")]
        public decimal? RaioAfetado { get; set; }

        // Relacionamento
        [ForeignKey("IdOcorrencia")]
        public virtual Ocorrencia Ocorrencia { get; set; } = null!;
    }
}