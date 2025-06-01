namespace EmergencyManagementAPI.DTOs
{
    public class AbrigoCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public int CapacidadeTotal { get; set; }
        public int VagasDisponiveis { get; set; }
        public string? Telefone { get; set; }
        public string Status { get; set; } = "disponivel";
    }

    public class AbrigoResponseDto
    {
        public int IdAbrigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public int CapacidadeTotal { get; set; }
        public int VagasDisponiveis { get; set; }
        public string? Telefone { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PessoasAbrigadas { get; set; }
        public decimal TaxaOcupacao { get; set; }
    }

    public class AbrigoUpdateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Endereco { get; set; }
        public int CapacidadeTotal { get; set; }
        public int VagasDisponiveis { get; set; }
        public string? Telefone { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}