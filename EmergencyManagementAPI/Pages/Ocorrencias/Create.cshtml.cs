using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Ocorrencias
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ocorrencia Ocorrencia { get; set; } = default!;

        public SelectList TiposOcorrencia { get; set; } = default!;
        public SelectList Usuarios { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectListsAsync();
            
            // Inicializar com valores padrão
            Ocorrencia = new Ocorrencia
            {
                DataOcorrencia = DateTime.Now,
                Status = "em andamento",
                NivelRisco = "moderado"
            };
            
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
                _context.Ocorrencias.Add(Ocorrencia);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Ocorrência criada com sucesso!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar a ocorrência: " + ex.Message);
                await LoadSelectListsAsync();
                return Page();
            }
        }

        private async Task LoadSelectListsAsync()
        {
            TiposOcorrencia = new SelectList(
                await _context.TiposOcorrencia.OrderBy(t => t.Nome).ToListAsync(),
                "IdTipo", "Nome");
                
            Usuarios = new SelectList(
                await _context.Usuarios
                    .OrderBy(u => u.Nome)
                    .ToListAsync(),
                "IdUsuario", "Nome");
        }
    }
}