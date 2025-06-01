using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EmergencyManagementAPI.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Usuario Usuario { get; set; } = default!;

        public IActionResult OnGet()
        {
            Usuario = new Usuario();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validações adicionais - Versão mais robusta para Oracle
            var emailCount = await _context.Usuarios
                .CountAsync(u => u.Email == Usuario.Email);
            
            if (emailCount > 0)
            {
                ModelState.AddModelError("Usuario.Email", "Este email já está cadastrado.");
                return Page();
            }

            // Definir data de cadastro
            Usuario.DataCadastro = DateTime.Now;

            _context.Usuarios.Add(Usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Usuário '{Usuario.Nome}' criado com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}