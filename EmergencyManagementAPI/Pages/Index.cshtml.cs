using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;

namespace EmergencyManagementAPI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalUsuarios { get; set; }
        public int TotalOcorrencias { get; set; }
        public int OcorrenciasAbertas { get; set; }
        public int TotalAlertas { get; set; }
        public int AlertasCriticos { get; set; }
        public int TotalAbrigos { get; set; }
        public int AbrigosDisponiveis { get; set; }
        public int PessoasAbrigadas { get; set; }
        public List<dynamic> UltimasOcorrencias { get; set; } = new();
        public List<dynamic> AlertasRecentes { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Estatísticas gerais
            TotalUsuarios = await _context.Usuarios.CountAsync();
            TotalOcorrencias = await _context.Ocorrencias.CountAsync();
            OcorrenciasAbertas = await _context.Ocorrencias.CountAsync(o => o.Status == "aberta");
            TotalAlertas = await _context.Alertas.CountAsync();
            AlertasCriticos = await _context.Alertas.CountAsync(a => a.NivelUrgencia == "critico");
            TotalAbrigos = await _context.Abrigos.CountAsync();
            AbrigosDisponiveis = await _context.Abrigos.CountAsync(a => a.Status == "disponivel");
            PessoasAbrigadas = await _context.CheckinsAbrigos.CountAsync(c => c.DataSaida == null);

            // Últimas ocorrências
            UltimasOcorrencias = await _context.Ocorrencias
                .Include(o => o.TipoOcorrencia)
                .Include(o => o.Usuario)
                .OrderByDescending(o => o.DataOcorrencia)
                .Take(5)
                .Select(o => new {
                    Id = o.IdOcorrencia,
                    Descricao = o.Descricao.Length > 50 ? o.Descricao.Substring(0, 50) + "..." : o.Descricao,
                    Tipo = o.TipoOcorrencia.Nome,
                    Usuario = o.Usuario.Nome,
                    Data = o.DataOcorrencia,
                    Status = o.Status,
                    NivelRisco = o.NivelRisco
                })
                .Cast<dynamic>()
                .ToListAsync();

            // Alertas recentes
            AlertasRecentes = await _context.Alertas
                .OrderByDescending(a => a.DataEmissao)
                .Take(5)
                .Select(a => new {
                    Id = a.IdAlerta,
                    Titulo = a.Titulo,
                    NivelUrgencia = a.NivelUrgencia,
                    DataEmissao = a.DataEmissao,
                    Fonte = a.Fonte
                })
                .Cast<dynamic>()
                .ToListAsync();
        }
    }
}