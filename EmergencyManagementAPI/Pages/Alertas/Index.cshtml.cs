using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Alertas
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Alerta> Alertas { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Alertas = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .OrderByDescending(a => a.DataEmissao)
                .ToListAsync();
        }
    }
}