using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Pages.Alertas
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Alerta Alerta { get; set; } = default!;

        public SelectList Ocorrencias { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectListsAsync();
            
            // Inicializar com valores padrão - apenas os campos necessários
            Alerta = new Alerta
            {
                NivelUrgencia = "medio",
                Fonte = "Sistema"
            };
            
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

            // Normalizar valores - apenas os campos do JSON
            Alerta.Titulo = Alerta.Titulo?.Trim() ?? "";
            Alerta.Mensagem = Alerta.Mensagem?.Trim() ?? "";
            Alerta.NivelUrgencia = Alerta.NivelUrgencia.ToLower();
            Alerta.Fonte = Alerta.Fonte?.Trim() ?? "Sistema";
            Alerta.DataEmissao = DateTime.Now;
            
            // Definir campos opcionais como null
            Alerta.Latitude = null;
            Alerta.Longitude = null;
            Alerta.RaioAfetado = null;

            try
            {
                _context.Alertas.Add(Alerta);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Alerta criado com sucesso!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao salvar o alerta: {ex.Message}");
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