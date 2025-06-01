using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Ocorrencias
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Ocorrencia Ocorrencia { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocorrencia = await _context.Ocorrencias
                .Include(o => o.TipoOcorrencia)
                .Include(o => o.Usuario)
                .Include(o => o.Alertas)
                .FirstOrDefaultAsync(m => m.IdOcorrencia == id);

            if (ocorrencia == null)
            {
                return NotFound();
            }
            else
            {
                Ocorrencia = ocorrencia;
            }
            return Page();
        }
    }
}