using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Alertas
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Alerta Alerta { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alerta = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .FirstOrDefaultAsync(m => m.IdAlerta == id);

            if (alerta == null)
            {
                return NotFound();
            }
            else
            {
                Alerta = alerta;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta != null)
            {
                try
                {
                    Alerta = alerta;
                    _context.Alertas.Remove(Alerta);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Alerta excluído com sucesso!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Erro ao excluir o alerta: {ex.Message}";
                    return RedirectToPage("./Edit", new { id = id });
                }
            }

            return RedirectToPage("./Index");
        }
    }
}