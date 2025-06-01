using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmergencyManagementAPI.Models
{
    [Table("checkins_abrigos")]
    public class CheckinAbrigo
    {
        [Key]
        [Column("id_checkin")]
        public int IdCheckin { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_abrigo")]
        public int IdAbrigo { get; set; }

        [Required]
        [Column("data_entrada")]
        public DateTime DataEntrada { get; set; }

        [Column("data_saida")]
        public DateTime? DataSaida { get; set; }

        // Relacionamentos
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("IdAbrigo")]
        public virtual Abrigo Abrigo { get; set; } = null!;
    }
}