using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Alertas
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Alerta Alerta { get; set; } = default!;

        public SelectList Ocorrencias { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alerta = await _context.Alertas.FirstOrDefaultAsync(m => m.IdAlerta == id);
            if (alerta == null)
            {
                return NotFound();
            }
            
            Alerta = alerta;
            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remover validações desnecessárias do ModelState para campos opcionais
            ModelState.Remove("Alerta.Latitude");
            ModelState.Remove("Alerta.Longitude");
            ModelState.Remove("Alerta.RaioAfetado");
            ModelState.Remove("Alerta.DataEmissao");
            ModelState.Remove("Alerta.Ocorrencia");

            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Validar se a ocorrência existe (se fornecida)
            if (Alerta.IdOcorrencia > 0)
            {
                var ocorrenciaExists = await _context.Ocorrencias
                    .AnyAsync(o => o.IdOcorrencia == Alerta.IdOcorrencia);
                
                if (!ocorrenciaExists)
                {
                    ModelState.AddModelError("Alerta.IdOcorrencia", "Ocorrência selecionada não existe.");
                    await LoadSelectListsAsync();
                    return Page();
                }
            }
            else
            {
                // Se não foi selecionada uma ocorrência, definir como 0
                Alerta.IdOcorrencia = 0;
            }

            // Validar nível de urgência
            var niveisValidos = new[] { "baixo", "medio", "alto", "critico" };
            if (!niveisValidos.Contains(Alerta.NivelUrgencia.ToLower()))
            {
                ModelState.AddModelError("Alerta.NivelUrgencia", "Nível de urgência deve ser: baixo, medio, alto ou critico.");
                await LoadSelectListsAsync();
                return Page();
            }

            try
            {
                // Buscar o alerta existente no banco
                var existingAlerta = await _context.Alertas
                    .FirstOrDefaultAsync(a => a.IdAlerta == Alerta.IdAlerta);
                    
                if (existingAlerta == null)
                {
                    return NotFound();
                }

                // Atualizar apenas as propriedades que podem ser editadas
                existingAlerta.Titulo = Alerta.Titulo?.Trim() ?? "";
                existingAlerta.Mensagem = Alerta.Mensagem?.Trim() ?? "";
                existingAlerta.NivelUrgencia = Alerta.NivelUrgencia.ToLower();
                existingAlerta.IdOcorrencia = Alerta.IdOcorrencia;
                existingAlerta.Fonte = Alerta.Fonte?.Trim() ?? "Sistema";
                // DataEmissao não deve ser alterada na edição

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Alerta atualizado com sucesso!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao atualizar o alerta: {ex.Message}");
                await LoadSelectListsAsync();
                return Page();
            }
        }

        private async Task LoadSelectListsAsync()
        {
            var ocorrencias = await _context.Ocorrencias
                .Select(o => new { o.IdOcorrencia, o.Descricao })
                .ToListAsync();

            Ocorrencias = new SelectList(ocorrencias, "IdOcorrencia", "Descricao");
        }
    }
}