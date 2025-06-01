using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;
using EmergencyManagementAPI.DTOs;

namespace EmergencyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbrigosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AbrigosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os abrigos com informações de ocupação
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AbrigoResponseDto>>> GetAbrigos()
        {
            var abrigos = await _context.Abrigos
                .Include(a => a.CheckinsAbrigos)
                .Select(a => new AbrigoResponseDto
                {
                    IdAbrigo = a.IdAbrigo,
                    Nome = a.Nome,
                    Endereco = a.Endereco,
                    CapacidadeTotal = a.CapacidadeTotal,
                    VagasDisponiveis = a.VagasDisponiveis,
                    Telefone = a.Telefone,
                    Status = a.Status,
                    PessoasAbrigadas = a.CheckinsAbrigos.Count(c => c.DataSaida == null),
                    TaxaOcupacao = a.CapacidadeTotal > 0 ? 
                        Math.Round((decimal)(a.CapacidadeTotal - a.VagasDisponiveis) / a.CapacidadeTotal * 100, 2) : 0
                })
                .ToListAsync();

            return Ok(abrigos);
        }

        /// <summary>
        /// Obtém um abrigo específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AbrigoResponseDto>> GetAbrigo(int id)
        {
            var abrigo = await _context.Abrigos
                .Include(a => a.CheckinsAbrigos)
                .Where(a => a.IdAbrigo == id)
                .Select(a => new AbrigoResponseDto
                {
                    IdAbrigo = a.IdAbrigo,
                    Nome = a.Nome,
                    Endereco = a.Endereco,
                    CapacidadeTotal = a.CapacidadeTotal,
                    VagasDisponiveis = a.VagasDisponiveis,
                    Telefone = a.Telefone,
                    Status = a.Status,
                    PessoasAbrigadas = a.CheckinsAbrigos.Count(c => c.DataSaida == null),
                    TaxaOcupacao = a.CapacidadeTotal > 0 ? 
                        Math.Round((decimal)(a.CapacidadeTotal - a.VagasDisponiveis) / a.CapacidadeTotal * 100, 2) : 0
                })
                .FirstOrDefaultAsync();

            if (abrigo == null)
            {
                return NotFound($"Abrigo com ID {id} não encontrado.");
            }

            return Ok(abrigo);
        }

        /// <summary>
        /// Cria um novo abrigo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AbrigoResponseDto>> CreateAbrigo(AbrigoCreateDto abrigoDto)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(abrigoDto.Nome))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (abrigoDto.CapacidadeTotal <= 0)
            {
                return BadRequest("Capacidade total deve ser maior que zero.");
            }

            if (abrigoDto.VagasDisponiveis < 0 || abrigoDto.VagasDisponiveis > abrigoDto.CapacidadeTotal)
            {
                return BadRequest("Vagas disponíveis deve estar entre 0 e a capacidade total.");
            }

            var statusValidos = new[] { "disponivel", "lotado", "manutencao", "inativo" };
            if (!statusValidos.Contains(abrigoDto.Status.ToLower()))
            {
                return BadRequest("Status deve ser: disponivel, lotado, manutencao ou inativo.");
            }

            // Verificar se já existe abrigo com o mesmo nome
            var nomeExiste = await _context.Abrigos
                .AnyAsync(a => a.Nome.ToLower() == abrigoDto.Nome.ToLower().Trim());
            
            if (nomeExiste)
            {
                return BadRequest($"Já existe um abrigo com o nome '{abrigoDto.Nome}'.");
            }

            var abrigo = new Abrigo
            {
                Nome = abrigoDto.Nome.Trim(),
                Endereco = abrigoDto.Endereco?.Trim(),
                CapacidadeTotal = abrigoDto.CapacidadeTotal,
                VagasDisponiveis = abrigoDto.VagasDisponiveis,
                Telefone = abrigoDto.Telefone?.Trim(),
                Status = abrigoDto.Status.ToLower()
            };

            _context.Abrigos.Add(abrigo);
            await _context.SaveChangesAsync();

            var abrigoCriado = new AbrigoResponseDto
            {
                IdAbrigo = abrigo.IdAbrigo,
                Nome = abrigo.Nome,
                Endereco = abrigo.Endereco,
                CapacidadeTotal = abrigo.CapacidadeTotal,
                VagasDisponiveis = abrigo.VagasDisponiveis,
                Telefone = abrigo.Telefone,
                Status = abrigo.Status,
                PessoasAbrigadas = 0,
                TaxaOcupacao = 0
            };

            return CreatedAtAction(nameof(GetAbrigo), new { id = abrigo.IdAbrigo }, abrigoCriado);
        }

        /// <summary>
        /// Atualiza um abrigo existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAbrigo(int id, AbrigoUpdateDto abrigoDto)
        {
            var abrigo = await _context.Abrigos.FindAsync(id);
            if (abrigo == null)
            {
                return NotFound($"Abrigo com ID {id} não encontrado.");
            }

            // Validações
            if (string.IsNullOrWhiteSpace(abrigoDto.Nome))
            {
                return BadRequest("Nome é obrigatório.");
            }

            if (abrigoDto.CapacidadeTotal <= 0)
            {
                return BadRequest("Capacidade total deve ser maior que zero.");
            }

            if (abrigoDto.VagasDisponiveis < 0 || abrigoDto.VagasDisponiveis > abrigoDto.CapacidadeTotal)
            {
                return BadRequest("Vagas disponíveis deve estar entre 0 e a capacidade total.");
            }

            var statusValidos = new[] { "disponivel", "lotado", "manutencao", "inativo" };
            if (!statusValidos.Contains(abrigoDto.Status.ToLower()))
            {
                return BadRequest("Status deve ser: disponivel, lotado, manutencao ou inativo.");
            }

            // Verificar se já existe outro abrigo com o mesmo nome
            var nomeExiste = await _context.Abrigos
                .AnyAsync(a => a.Nome.ToLower() == abrigoDto.Nome.ToLower().Trim() && a.IdAbrigo != id);
            
            if (nomeExiste)
            {
                return BadRequest($"Já existe outro abrigo com o nome '{abrigoDto.Nome}'.");
            }

            // Atualizar propriedades
            abrigo.Nome = abrigoDto.Nome.Trim();
            abrigo.Endereco = abrigoDto.Endereco?.Trim();
            abrigo.CapacidadeTotal = abrigoDto.CapacidadeTotal;
            abrigo.VagasDisponiveis = abrigoDto.VagasDisponiveis;
            abrigo.Telefone = abrigoDto.Telefone?.Trim();
            abrigo.Status = abrigoDto.Status.ToLower();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Abrigos.AnyAsync(a => a.IdAbrigo == id))
                {
                    return NotFound($"Abrigo com ID {id} não encontrado.");
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Obtém abrigos por status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AbrigoResponseDto>>> GetAbrigosPorStatus(string status)
        {
            var statusValidos = new[] { "disponivel", "lotado", "manutencao", "inativo" };
            if (!statusValidos.Contains(status.ToLower()))
            {
                return BadRequest("Status deve ser: disponivel, lotado, manutencao ou inativo.");
            }

            var abrigos = await _context.Abrigos
                .Include(a => a.CheckinsAbrigos)
                .Where(a => a.Status == status.ToLower())
                .Select(a => new AbrigoResponseDto
                {
                    IdAbrigo = a.IdAbrigo,
                    Nome = a.Nome,
                    Endereco = a.Endereco,
                    CapacidadeTotal = a.CapacidadeTotal,
                    VagasDisponiveis = a.VagasDisponiveis,
                    Telefone = a.Telefone,
                    Status = a.Status,
                    PessoasAbrigadas = a.CheckinsAbrigos.Count(c => c.DataSaida == null),
                    TaxaOcupacao = a.CapacidadeTotal > 0 ? 
                        Math.Round((decimal)(a.CapacidadeTotal - a.VagasDisponiveis) / a.CapacidadeTotal * 100, 2) : 0
                })
                .ToListAsync();

            return Ok(abrigos);
        }

        /// <summary>
        /// Exclui um abrigo (apenas se não houver pessoas abrigadas)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbrigo(int id)
        {
            var abrigo = await _context.Abrigos
                .Include(a => a.CheckinsAbrigos)
                .FirstOrDefaultAsync(a => a.IdAbrigo == id);
            
            if (abrigo == null)
            {
                return NotFound($"Abrigo com ID {id} não encontrado.");
            }

            // Verificar se há pessoas ainda abrigadas
            var pessoasAbrigadas = abrigo.CheckinsAbrigos.Any(c => c.DataSaida == null);
            if (pessoasAbrigadas)
            {
                return BadRequest("Não é possível excluir o abrigo pois ainda há pessoas abrigadas.");
            }

            _context.Abrigos.Remove(abrigo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}