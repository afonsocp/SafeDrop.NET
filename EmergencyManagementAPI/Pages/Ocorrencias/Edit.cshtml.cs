using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Ocorrencias
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ocorrencia Ocorrencia { get; set; } = default!;

        public SelectList TiposOcorrencia { get; set; } = default!;
        public SelectList Usuarios { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string? status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocorrencia = await _context.Ocorrencias.FirstOrDefaultAsync(m => m.IdOcorrencia == id);
            if (ocorrencia == null)
            {
                return NotFound();
            }
            
            Ocorrencia = ocorrencia;
            
            // Se status foi passado como parâmetro, atualizar
            if (!string.IsNullOrEmpty(status))
            {
                Ocorrencia.Status = status;
            }
            
            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Validar se o tipo de ocorrência existe
            var tipoExists = await _context.TiposOcorrencia
                .AnyAsync(t => t.IdTipo == Ocorrencia.IdTipo);
            if (!tipoExists)
            {
                ModelState.AddModelError("Ocorrencia.IdTipo", "Tipo de ocorrência inválido.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Validar se o usuário existe
            var usuarioExists = await _context.Usuarios
                .AnyAsync(u => u.IdUsuario == Ocorrencia.IdUsuario);
            if (!usuarioExists)
            {
                ModelState.AddModelError("Ocorrencia.IdUsuario", "Usuário inválido.");
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                // Buscar a ocorrência existente no banco
                var existingOcorrencia = await _context.Ocorrencias
                    .FirstOrDefaultAsync(o => o.IdOcorrencia == Ocorrencia.IdOcorrencia);
                    
                if (existingOcorrencia == null)
                {
                    return NotFound();
                }

                // Atualizar apenas as propriedades que podem ser editadas
                existingOcorrencia.IdTipo = Ocorrencia.IdTipo;
                existingOcorrencia.IdUsuario = Ocorrencia.IdUsuario;
                existingOcorrencia.Descricao = Ocorrencia.Descricao;
                existingOcorrencia.Latitude = Ocorrencia.Latitude;
                existingOcorrencia.Longitude = Ocorrencia.Longitude;
                existingOcorrencia.DataOcorrencia = Ocorrencia.DataOcorrencia;
                existingOcorrencia.Status = Ocorrencia.Status;
                existingOcorrencia.NivelRisco = Ocorrencia.NivelRisco;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ocorrência atualizada com sucesso!";
                return RedirectToPage("./Details", new { id = Ocorrencia.IdOcorrencia });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OcorrenciaExists(Ocorrencia.IdOcorrencia))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Erro de concorrência. A ocorrência foi modificada por outro usuário.");
                    await LoadSelectListsAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar a ocorrência: " + ex.Message);
                await LoadSelectListsAsync();
                return Page();
            }
        }

        private bool OcorrenciaExists(int id)
        {
            return _context.Ocorrencias.Any(e => e.IdOcorrencia == id);
        }

        // Se existir um método similar, aplique a mesma correção
        private async Task LoadSelectListsAsync()
        {
            TiposOcorrencia = new SelectList(
                await _context.TiposOcorrencia.OrderBy(t => t.Nome).ToListAsync(),
                "IdTipo", "Nome", Ocorrencia.IdTipo);
                
            Usuarios = new SelectList(
                await _context.Usuarios
                    .OrderBy(u => u.Nome)
                    .ToListAsync(),
                "IdUsuario", "Nome", Ocorrencia.IdUsuario);
        }
    }
}