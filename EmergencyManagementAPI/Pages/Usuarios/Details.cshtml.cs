using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Usuarios
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Usuario Usuario { get; set; } = default!;
        public List<dynamic> OcorrenciasUsuario { get; set; } = new();
        public List<dynamic> CheckinsAbrigos { get; set; } = new();
        public int TotalOcorrencias { get; set; }
        public int OcorrenciasAbertas { get; set; }
        
        // Missing properties that are referenced in the Razor page
        public int OcorrenciasResolvidas { get; set; }
        public int OcorrenciasAndamento { get; set; }
        public int TotalCheckinsAbrigos { get; set; }
        public List<dynamic> OcorrenciasRecentes { get; set; } = new();
        public List<dynamic> CheckinsRecentes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }
            Usuario = usuario;

            // Carregar ocorrências do usuário
            OcorrenciasUsuario = await _context.Ocorrencias
                .Include(o => o.TipoOcorrencia)
                .Where(o => o.IdUsuario == id)
                .OrderByDescending(o => o.DataOcorrencia)
                .Take(10)
                .Select(o => new {
                    Id = o.IdOcorrencia,
                    Descricao = o.Descricao.Length > 100 ? o.Descricao.Substring(0, 100) + "..." : o.Descricao,
                    Tipo = o.TipoOcorrencia.Nome,
                    Data = o.DataOcorrencia,
                    Status = o.Status,
                    NivelRisco = o.NivelRisco
                })
                .Cast<dynamic>()
                .ToListAsync();

            // Set OcorrenciasRecentes to the same data
            OcorrenciasRecentes = OcorrenciasUsuario;

            // Carregar histórico de abrigos
            CheckinsAbrigos = await _context.CheckinsAbrigos
                .Include(c => c.Abrigo)
                .Where(c => c.IdUsuario == id)
                .OrderByDescending(c => c.DataEntrada)
                .Take(10)
                .Select(c => new {
                    Abrigo = c.Abrigo.Nome,
                    DataEntrada = c.DataEntrada,
                    DataSaida = c.DataSaida,
                    Status = c.DataSaida == null ? "Abrigado" : "Saiu",
                    TempoAbrigado = c.DataSaida != null ? c.DataSaida - c.DataEntrada : DateTime.Now - c.DataEntrada
                })
                .Cast<dynamic>()
                .ToListAsync();

            // Set CheckinsRecentes to the same data
            CheckinsRecentes = CheckinsAbrigos;

            // Estatísticas
            TotalOcorrencias = await _context.Ocorrencias.CountAsync(o => o.IdUsuario == id);
            OcorrenciasAbertas = await _context.Ocorrencias.CountAsync(o => o.IdUsuario == id && o.Status == "aberta");
            OcorrenciasResolvidas = await _context.Ocorrencias.CountAsync(o => o.IdUsuario == id && o.Status == "resolvida");
            OcorrenciasAndamento = await _context.Ocorrencias.CountAsync(o => o.IdUsuario == id && o.Status == "em_andamento");
            TotalCheckinsAbrigos = await _context.CheckinsAbrigos.CountAsync(c => c.IdUsuario == id);

            return Page();
        }
    }
}