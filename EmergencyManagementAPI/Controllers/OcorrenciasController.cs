using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;
using EmergencyManagementAPI.DTOs;

namespace EmergencyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OcorrenciasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OcorrenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as ocorrências com dados relacionados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OcorrenciaResponseDto>>> GetOcorrencias()
        {
            var ocorrencias = await _context.Ocorrencias
                .Include(o => o.Usuario)
                .Include(o => o.TipoOcorrencia)
                .Select(o => new OcorrenciaResponseDto
                {
                    IdOcorrencia = o.IdOcorrencia,
                    NomeUsuario = o.Usuario.Nome,
                    TipoOcorrencia = o.TipoOcorrencia.Nome,
                    Descricao = o.Descricao,
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    DataOcorrencia = o.DataOcorrencia,
                    NivelRisco = o.NivelRisco,
                    Status = o.Status
                })
                .ToListAsync();

            return Ok(ocorrencias);
        }

        /// <summary>
        /// Obtém uma ocorrência específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OcorrenciaResponseDto>> GetOcorrencia(int id)
        {
            var ocorrencia = await _context.Ocorrencias
                .Include(o => o.Usuario)
                .Include(o => o.TipoOcorrencia)
                .FirstOrDefaultAsync(o => o.IdOcorrencia == id);

            if (ocorrencia == null)
            {
                return NotFound("Ocorrência não encontrada");
            }

            var ocorrenciaDto = new OcorrenciaResponseDto
            {
                IdOcorrencia = ocorrencia.IdOcorrencia,
                NomeUsuario = ocorrencia.Usuario.Nome,
                TipoOcorrencia = ocorrencia.TipoOcorrencia.Nome,
                Descricao = ocorrencia.Descricao,
                Latitude = ocorrencia.Latitude,
                Longitude = ocorrencia.Longitude,
                DataOcorrencia = ocorrencia.DataOcorrencia,
                NivelRisco = ocorrencia.NivelRisco,
                Status = ocorrencia.Status
            };

            return Ok(ocorrenciaDto);
        }

        /// <summary>
        /// Cria uma nova ocorrência
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OcorrenciaResponseDto>> CreateOcorrencia(OcorrenciaCreateDto ocorrenciaDto)
        {
            // Validar se usuário existe
            var usuario = await _context.Usuarios.FindAsync(ocorrenciaDto.IdUsuario);
            if (usuario == null)
            {
                return BadRequest("Usuário não encontrado");
            }

            // Validar se tipo de ocorrência existe
            var tipoOcorrencia = await _context.TiposOcorrencia.FindAsync(ocorrenciaDto.IdTipo);
            if (tipoOcorrencia == null)
            {
                return BadRequest("Tipo de ocorrência não encontrado");
            }

            // Validar nível de risco
            var niveisRiscoValidos = new[] { "baixo", "moderado", "alto" };
            if (!niveisRiscoValidos.Contains(ocorrenciaDto.NivelRisco.ToLower()))
            {
                return BadRequest("Nível de risco inválido. Use: baixo, moderado ou alto");
            }

            // Validar status
            var statusValidos = new[] { "em andamento", "resolvido" };
            if (!statusValidos.Contains(ocorrenciaDto.Status.ToLower()))
            {
                return BadRequest("Status inválido. Use: em andamento ou resolvido");
            }

            var ocorrencia = new Ocorrencia
            {
                IdUsuario = ocorrenciaDto.IdUsuario,
                IdTipo = ocorrenciaDto.IdTipo,
                Descricao = ocorrenciaDto.Descricao,
                Latitude = ocorrenciaDto.Latitude,
                Longitude = ocorrenciaDto.Longitude,
                DataOcorrencia = ocorrenciaDto.DataOcorrencia,
                NivelRisco = ocorrenciaDto.NivelRisco.ToLower(),
                Status = ocorrenciaDto.Status.ToLower()
            };

            _context.Ocorrencias.Add(ocorrencia);
            await _context.SaveChangesAsync();

            // Recarregar com dados relacionados
            await _context.Entry(ocorrencia)
                .Reference(o => o.Usuario)
                .LoadAsync();
            await _context.Entry(ocorrencia)
                .Reference(o => o.TipoOcorrencia)
                .LoadAsync();

            var responseDto = new OcorrenciaResponseDto
            {
                IdOcorrencia = ocorrencia.IdOcorrencia,
                NomeUsuario = ocorrencia.Usuario.Nome,
                TipoOcorrencia = ocorrencia.TipoOcorrencia.Nome,
                Descricao = ocorrencia.Descricao,
                Latitude = ocorrencia.Latitude,
                Longitude = ocorrencia.Longitude,
                DataOcorrencia = ocorrencia.DataOcorrencia,
                NivelRisco = ocorrencia.NivelRisco,
                Status = ocorrencia.Status
            };

            return CreatedAtAction(nameof(GetOcorrencia), new { id = ocorrencia.IdOcorrencia }, responseDto);
        }

        /// <summary>
        /// Atualiza o status de uma ocorrência
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string novoStatus)
        {
            var ocorrencia = await _context.Ocorrencias.FindAsync(id);
            if (ocorrencia == null)
            {
                return NotFound("Ocorrência não encontrada");
            }

            var statusValidos = new[] { "em andamento", "resolvido" };
            if (!statusValidos.Contains(novoStatus.ToLower()))
            {
                return BadRequest("Status inválido. Use: em andamento ou resolvido");
            }

            ocorrencia.Status = novoStatus.ToLower();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtém ocorrências por nível de risco
        /// </summary>
        [HttpGet("risco/{nivel}")]
        public async Task<ActionResult<IEnumerable<OcorrenciaResponseDto>>> GetOcorrenciasPorRisco(string nivel)
        {
            var niveisValidos = new[] { "baixo", "moderado", "alto" };
            if (!niveisValidos.Contains(nivel.ToLower()))
            {
                return BadRequest("Nível de risco inválido");
            }

            var ocorrencias = await _context.Ocorrencias
                .Include(o => o.Usuario)
                .Include(o => o.TipoOcorrencia)
                .Where(o => o.NivelRisco == nivel.ToLower())
                .Select(o => new OcorrenciaResponseDto
                {
                    IdOcorrencia = o.IdOcorrencia,
                    NomeUsuario = o.Usuario.Nome,
                    TipoOcorrencia = o.TipoOcorrencia.Nome,
                    Descricao = o.Descricao,
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    DataOcorrencia = o.DataOcorrencia,
                    NivelRisco = o.NivelRisco,
                    Status = o.Status
                })
                .ToListAsync();

            return Ok(ocorrencias);
        }
    }
}