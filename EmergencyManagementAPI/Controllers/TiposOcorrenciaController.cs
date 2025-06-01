using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TiposOcorrenciaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TiposOcorrenciaController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os tipos de ocorrência
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoOcorrencia>>> GetTiposOcorrencia()
        {
            return await _context.TiposOcorrencia.ToListAsync();
        }

        /// <summary>
        /// Obtém um tipo de ocorrência específico
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoOcorrencia>> GetTipoOcorrencia(int id)
        {
            var tipoOcorrencia = await _context.TiposOcorrencia.FindAsync(id);

            if (tipoOcorrencia == null)
            {
                return NotFound("Tipo de ocorrência não encontrado");
            }

            return tipoOcorrencia;
        }

        /// <summary>
        /// Cria um novo tipo de ocorrência
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TipoOcorrencia>> CreateTipoOcorrencia(TipoOcorrencia tipoOcorrencia)
        {
            // Validar se já existe um tipo com o mesmo nome
            if (await _context.TiposOcorrencia.AnyAsync(t => t.Nome.ToLower() == tipoOcorrencia.Nome.ToLower()))
            {
                return BadRequest("Já existe um tipo de ocorrência com este nome");
            }

            _context.TiposOcorrencia.Add(tipoOcorrencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTipoOcorrencia), new { id = tipoOcorrencia.IdTipo }, tipoOcorrencia);
        }

        /// <summary>
        /// Atualiza um tipo de ocorrência
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoOcorrencia(int id, TipoOcorrencia tipoOcorrencia)
        {
            if (id != tipoOcorrencia.IdTipo)
            {
                return BadRequest("ID não confere");
            }

            // Verificar se existe outro tipo com o mesmo nome
            if (await _context.TiposOcorrencia.AnyAsync(t => t.Nome.ToLower() == tipoOcorrencia.Nome.ToLower() && t.IdTipo != id))
            {
                return BadRequest("Já existe um tipo de ocorrência com este nome");
            }

            _context.Entry(tipoOcorrencia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TipoOcorrenciaExists(id))
                {
                    return NotFound("Tipo de ocorrência não encontrado");
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um tipo de ocorrência
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoOcorrencia(int id)
        {
            var tipoOcorrencia = await _context.TiposOcorrencia.FindAsync(id);
            if (tipoOcorrencia == null)
            {
                return NotFound("Tipo de ocorrência não encontrado");
            }

            // Verificar se há ocorrências vinculadas
            if (await _context.Ocorrencias.AnyAsync(o => o.IdTipo == id))
            {
                return BadRequest("Não é possível excluir. Existem ocorrências vinculadas a este tipo.");
            }

            _context.TiposOcorrencia.Remove(tipoOcorrencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> TipoOcorrenciaExists(int id)
        {
            return await _context.TiposOcorrencia.AnyAsync(e => e.IdTipo == id);
        }
    }
}