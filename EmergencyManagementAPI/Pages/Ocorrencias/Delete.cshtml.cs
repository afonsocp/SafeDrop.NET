using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Ocorrencias
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ocorrencia Ocorrencia { get; set; } = default!;
        
        public bool HasRelatedAlerts { get; set; }
        public int RelatedAlertsCount { get; set; }

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
                HasRelatedAlerts = ocorrencia.Alertas.Any();
                RelatedAlertsCount = ocorrencia.Alertas.Count;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocorrencia = await _context.Ocorrencias
                .Include(o => o.Alertas)
                .FirstOrDefaultAsync(m => m.IdOcorrencia == id);

            if (ocorrencia != null)
            {
                try
                {
                    // Verificar se há alertas relacionados
                    if (ocorrencia.Alertas.Any())
                    {
                        TempData["ErrorMessage"] = $"Não é possível excluir esta ocorrência pois ela possui {ocorrencia.Alertas.Count} alerta(s) relacionado(s). Exclua os alertas primeiro.";
                        return RedirectToPage("./Details", new { id = ocorrencia.IdOcorrencia });
                    }

                    Ocorrencia = ocorrencia;
                    _context.Ocorrencias.Remove(Ocorrencia);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Ocorrência excluída com sucesso!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erro ao excluir a ocorrência: " + ex.Message;
                    return RedirectToPage("./Details", new { id = ocorrencia.IdOcorrencia });
                }
            }

            return RedirectToPage("./Index");
        }
    }
}