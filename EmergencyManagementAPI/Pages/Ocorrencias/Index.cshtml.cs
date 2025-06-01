using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Ocorrencias
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Ocorrencia> Ocorrencias { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? NivelRiscoFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? TipoOcorrenciaFilter { get; set; }

        public IList<TipoOcorrencia> TiposOcorrencia { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Carregar tipos de ocorrência para o filtro
            TiposOcorrencia = await _context.TiposOcorrencia
                .OrderBy(t => t.Nome)
                .ToListAsync();

            var query = _context.Ocorrencias
                .Include(o => o.TipoOcorrencia)
                .Include(o => o.Usuario)
                .AsQueryable();

            // Filtro por texto (descrição ou coordenadas)
            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(o => o.Descricao.Contains(SearchString) ||
                                        (o.Latitude.HasValue && o.Latitude.ToString().Contains(SearchString)) ||
                                        (o.Longitude.HasValue && o.Longitude.ToString().Contains(SearchString)));
            }

            // Filtro por status
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                query = query.Where(o => o.Status == StatusFilter);
            }

            // Filtro por nível de risco
            if (!string.IsNullOrEmpty(NivelRiscoFilter))
            {
                query = query.Where(o => o.NivelRisco == NivelRiscoFilter);
            }

            // Filtro por tipo de ocorrência
            if (TipoOcorrenciaFilter.HasValue)
            {
                query = query.Where(o => o.IdTipo == TipoOcorrenciaFilter.Value);
            }

            Ocorrencias = await query
                .OrderByDescending(o => o.DataOcorrencia)
                .ToListAsync();
        }
    }
}