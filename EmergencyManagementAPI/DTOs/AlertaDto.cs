namespace EmergencyManagementAPI.DTOs
{
    public class AlertaCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string NivelUrgencia { get; set; } = string.Empty;
        public int? IdOcorrencia { get; set; }
        public string Fonte { get; set; } = string.Empty;
    }

    public class AlertaResponseDto
    {
        public int IdAlerta { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string NivelUrgencia { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public int? IdOcorrencia { get; set; }
        public string? DescricaoOcorrencia { get; set; }
        public string Fonte { get; set; } = string.Empty;
    }
}