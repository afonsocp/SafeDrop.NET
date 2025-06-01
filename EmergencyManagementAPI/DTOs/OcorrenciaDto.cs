namespace EmergencyManagementAPI.DTOs
{
    public class OcorrenciaCreateDto
    {
        public int IdUsuario { get; set; }
        public int IdTipo { get; set; }
        public string? Descricao { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string NivelRisco { get; set; } = string.Empty;
        public string Status { get; set; } = "em andamento";
    }

    public class OcorrenciaResponseDto
    {
        public int IdOcorrencia { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string TipoOcorrencia { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string NivelRisco { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}