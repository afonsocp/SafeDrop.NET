using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Buscar o usuário existente do banco de dados primeiro
            var usuarioExistente = await _context.Usuarios.FindAsync(Usuario.IdUsuario);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Preservar a senha existente
            Usuario.Senha = usuarioExistente.Senha;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validações adicionais
            if (await _context.Usuarios.AnyAsync(u => u.Email == Usuario.Email && u.IdUsuario != Usuario.IdUsuario))
            {
                ModelState.AddModelError("Usuario.Email", "Este email já está sendo usado por outro usuário.");
                return Page();
            }

            // Atualizar apenas as propriedades necessárias
            usuarioExistente.Nome = Usuario.Nome;
            usuarioExistente.Email = Usuario.Email;
            usuarioExistente.TipoUsuario = Usuario.TipoUsuario;
            usuarioExistente.Endereco = Usuario.Endereco;
            // Senha permanece inalterada

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Usuário '{Usuario.Nome}' atualizado com sucesso!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(Usuario.IdUsuario))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao atualizar usuário: {ex.Message}");
                return Page();
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}