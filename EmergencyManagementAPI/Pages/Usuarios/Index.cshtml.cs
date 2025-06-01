using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Usuario> Usuarios { get; set; } = default!;
        public string? SearchTerm { get; set; }
        public string? FilterType { get; set; }

        public async Task OnGetAsync(string? searchTerm, string? filterType)
        {
            SearchTerm = searchTerm;
            FilterType = filterType;

            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Nome.Contains(searchTerm) || 
                                        u.Email.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                query = query.Where(u => u.TipoUsuario == filterType);
            }

            Usuarios = await query.OrderBy(u => u.Nome).ToListAsync();
        }
    }
}