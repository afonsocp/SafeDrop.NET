using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;
using EmergencyManagementAPI.DTOs;

namespace EmergencyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os alertas com informações da ocorrência relacionada
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertaResponseDto>>> GetAlertas()
        {
            var alertas = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .Select(a => new AlertaResponseDto
                {
                    IdAlerta = a.IdAlerta,
                    Titulo = a.Titulo,
                    Mensagem = a.Mensagem,
                    NivelUrgencia = a.NivelUrgencia,
                    DataEmissao = a.DataEmissao,
                    IdOcorrencia = a.IdOcorrencia,
                    DescricaoOcorrencia = a.Ocorrencia != null ? a.Ocorrencia.Descricao : null,
                    Fonte = a.Fonte
                })
                .OrderByDescending(a => a.DataEmissao)
                .ToListAsync();

            return Ok(alertas);
        }

        /// <summary>
        /// Obtém um alerta específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AlertaResponseDto>> GetAlerta(int id)
        {
            var alerta = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .Where(a => a.IdAlerta == id)
                .Select(a => new AlertaResponseDto
                {
                    IdAlerta = a.IdAlerta,
                    Titulo = a.Titulo,
                    Mensagem = a.Mensagem,
                    NivelUrgencia = a.NivelUrgencia,
                    DataEmissao = a.DataEmissao,
                    IdOcorrencia = a.IdOcorrencia,
                    DescricaoOcorrencia = a.Ocorrencia != null ? a.Ocorrencia.Descricao : null,
                    Fonte = a.Fonte
                })
                .FirstOrDefaultAsync();

            if (alerta == null)
            {
                return NotFound($"Alerta com ID {id} não encontrado.");
            }

            return Ok(alerta);
        }

        /// <summary>
        /// Cria um novo alerta
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AlertaResponseDto>> CreateAlerta(AlertaCreateDto alertaDto)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(alertaDto.Titulo))
            {
                return BadRequest("Título é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(alertaDto.Mensagem))
            {
                return BadRequest("Mensagem é obrigatória.");
            }

            var niveisValidos = new[] { "baixo", "medio", "alto", "critico" };
            if (!niveisValidos.Contains(alertaDto.NivelUrgencia.ToLower()))
            {
                return BadRequest("Nível de urgência deve ser: baixo, medio, alto ou critico.");
            }

            // Verificar se a ocorrência existe (se fornecida)
            if (alertaDto.IdOcorrencia.HasValue)
            {
                var ocorrenciaExiste = await _context.Ocorrencias
                    .AnyAsync(o => o.IdOcorrencia == alertaDto.IdOcorrencia.Value);
                
                if (!ocorrenciaExiste)
                {
                    return BadRequest($"Ocorrência com ID {alertaDto.IdOcorrencia} não encontrada.");
                }
            }

            var alerta = new Alerta
            {
                Titulo = alertaDto.Titulo.Trim(),
                Mensagem = alertaDto.Mensagem.Trim(),
                NivelUrgencia = alertaDto.NivelUrgencia.ToLower(),
                DataEmissao = DateTime.Now,
                // Na linha 116, alterar de:
                // IdOcorrencia = alertaDto.IdOcorrencia,
                // Para:
                IdOcorrencia = alertaDto.IdOcorrencia ?? 0,
                Fonte = alertaDto.Fonte?.Trim() ?? "Sistema"
            };

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            // Retornar o alerta criado com informações completas
            var alertaCriado = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .Where(a => a.IdAlerta == alerta.IdAlerta)
                .Select(a => new AlertaResponseDto
                {
                    IdAlerta = a.IdAlerta,
                    Titulo = a.Titulo,
                    Mensagem = a.Mensagem,
                    NivelUrgencia = a.NivelUrgencia,
                    DataEmissao = a.DataEmissao,
                    IdOcorrencia = a.IdOcorrencia,
                    DescricaoOcorrencia = a.Ocorrencia != null ? a.Ocorrencia.Descricao : null,
                    Fonte = a.Fonte
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetAlerta), new { id = alerta.IdAlerta }, alertaCriado);
        }

        /// <summary>
        /// Obtém alertas por nível de urgência
        /// </summary>
        [HttpGet("nivel/{nivel}")]
        public async Task<ActionResult<IEnumerable<AlertaResponseDto>>> GetAlertasPorNivel(string nivel)
        {
            var niveisValidos = new[] { "baixo", "medio", "alto", "critico" };
            if (!niveisValidos.Contains(nivel.ToLower()))
            {
                return BadRequest("Nível de urgência deve ser: baixo, medio, alto ou critico.");
            }

            var alertas = await _context.Alertas
                .Include(a => a.Ocorrencia)
                .Where(a => a.NivelUrgencia == nivel.ToLower())
                .Select(a => new AlertaResponseDto
                {
                    IdAlerta = a.IdAlerta,
                    Titulo = a.Titulo,
                    Mensagem = a.Mensagem,
                    NivelUrgencia = a.NivelUrgencia,
                    DataEmissao = a.DataEmissao,
                    IdOcorrencia = a.IdOcorrencia,
                    DescricaoOcorrencia = a.Ocorrencia != null ? a.Ocorrencia.Descricao : null,
                    Fonte = a.Fonte
                })
                .OrderByDescending(a => a.DataEmissao)
                .ToListAsync();

            return Ok(alertas);
        }

        /// <summary>
        /// Exclui um alerta
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlerta(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);
            if (alerta == null)
            {
                return NotFound($"Alerta com ID {id} não encontrado.");
            }

            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}