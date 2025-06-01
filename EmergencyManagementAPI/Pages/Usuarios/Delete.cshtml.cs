using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Usuarios
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;
        public bool TemOcorrencias { get; set; }
        public bool EstaAbrigado { get; set; }
        public string? ErrorMessage { get; set; }

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
            else
            {
                Usuario = usuario;
            }

            // Verificar dependências
            TemOcorrencias = await _context.Ocorrencias.AnyAsync(o => o.IdUsuario == id);
            EstaAbrigado = await _context.CheckinsAbrigos.AnyAsync(c => c.IdUsuario == id && c.DataSaida == null);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Verificar se está abrigado
                var estaAbrigado = await _context.CheckinsAbrigos
                    .AnyAsync(c => c.IdUsuario == id && c.DataSaida == null);
                
                if (estaAbrigado)
                {
                    ErrorMessage = "Não é possível excluir o usuário pois ele está atualmente abrigado.";
                    Usuario = usuario;
                    TemOcorrencias = await _context.Ocorrencias.AnyAsync(o => o.IdUsuario == id);
                    EstaAbrigado = true;
                    return Page();
                }

                Usuario = usuario;
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Usuário '{usuario.Nome}' excluído com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}