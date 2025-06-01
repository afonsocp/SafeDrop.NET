using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;

namespace EmergencyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckinsAbrigosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckinsAbrigosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Realiza check-in de uma pessoa em um abrigo
        /// </summary>
        [HttpPost("checkin")]
        public async Task<ActionResult> CheckIn(int idUsuario, int idAbrigo)
        {
            // Verificar se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null)
            {
                return BadRequest($"Usuário com ID {idUsuario} não encontrado.");
            }

            // Verificar se o abrigo existe
            var abrigo = await _context.Abrigos.FindAsync(idAbrigo);
            if (abrigo == null)
            {
                return BadRequest($"Abrigo com ID {idAbrigo} não encontrado.");
            }

            // Verificar se o abrigo está disponível
            if (abrigo.Status != "disponivel")
            {
                return BadRequest($"Abrigo '{abrigo.Nome}' não está disponível para check-in.");
            }

            // Verificar se há vagas disponíveis
            if (abrigo.VagasDisponiveis <= 0)
            {
                return BadRequest($"Abrigo '{abrigo.Nome}' não possui vagas disponíveis.");
            }

            // Verificar se o usuário já está abrigado em algum lugar
            var jaAbrigado = await _context.CheckinsAbrigos
                .AnyAsync(c => c.IdUsuario == idUsuario && c.DataSaida == null);
            
            if (jaAbrigado)
            {
                return BadRequest($"Usuário '{usuario.Nome}' já está abrigado em outro local.");
            }

            // Realizar o check-in
            var checkin = new CheckinAbrigo
            {
                IdUsuario = idUsuario,
                IdAbrigo = idAbrigo,
                DataEntrada = DateTime.Now
            };

            _context.CheckinsAbrigos.Add(checkin);
            
            // Atualizar vagas disponíveis
            abrigo.VagasDisponiveis--;
            
            // Atualizar status se lotou
            if (abrigo.VagasDisponiveis == 0)
            {
                abrigo.Status = "lotado";
            }

            await _context.SaveChangesAsync();

            return Ok(new { 
                Mensagem = $"Check-in realizado com sucesso para {usuario.Nome} no abrigo {abrigo.Nome}.",
                IdCheckin = checkin.IdCheckin,
                DataEntrada = checkin.DataEntrada,
                VagasRestantes = abrigo.VagasDisponiveis
            });
        }

        /// <summary>
        /// Realiza check-out de uma pessoa de um abrigo
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult> CheckOut(int idUsuario, int idAbrigo)
        {
            // Buscar o check-in ativo
            var checkin = await _context.CheckinsAbrigos
                .Include(c => c.Usuario)
                .Include(c => c.Abrigo)
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && 
                                         c.IdAbrigo == idAbrigo && 
                                         c.DataSaida == null);
            
            if (checkin == null)
            {
                return BadRequest($"Não foi encontrado check-in ativo para este usuário neste abrigo.");
            }

            // Realizar o check-out
            checkin.DataSaida = DateTime.Now;
            
            // Atualizar vagas disponíveis
            var abrigo = checkin.Abrigo;
            abrigo.VagasDisponiveis++;
            
            // Atualizar status se não está mais lotado
            if (abrigo.Status == "lotado" && abrigo.VagasDisponiveis > 0)
            {
                abrigo.Status = "disponivel";
            }

            await _context.SaveChangesAsync();

            return Ok(new { 
                Mensagem = $"Check-out realizado com sucesso para {checkin.Usuario.Nome} do abrigo {abrigo.Nome}.",
                DataSaida = checkin.DataSaida,
                TempoAbrigado = checkin.DataSaida - checkin.DataEntrada,
                VagasDisponiveis = abrigo.VagasDisponiveis
            });
        }

        /// <summary>
        /// Obtém histórico de check-ins de um abrigo
        /// </summary>
        [HttpGet("abrigo/{idAbrigo}")]
        public async Task<ActionResult> GetHistoricoAbrigo(int idAbrigo)
        {
            var abrigo = await _context.Abrigos.FindAsync(idAbrigo);
            if (abrigo == null)
            {
                return NotFound($"Abrigo com ID {idAbrigo} não encontrado.");
            }

            var historico = await _context.CheckinsAbrigos
                .Include(c => c.Usuario)
                .Where(c => c.IdAbrigo == idAbrigo)
                .OrderByDescending(c => c.DataEntrada)
                .Select(c => new {
                    IdCheckin = c.IdCheckin,
                    Usuario = c.Usuario.Nome,
                    DataEntrada = c.DataEntrada,
                    DataSaida = c.DataSaida,
                    Status = c.DataSaida == null ? "Abrigado" : "Saiu",
                    TempoAbrigado = c.DataSaida != null ? c.DataSaida - c.DataEntrada : DateTime.Now - c.DataEntrada
                })
                .ToListAsync();

            return Ok(new {
                Abrigo = abrigo.Nome,
                TotalCheckins = historico.Count,
                PessoasAtualmente = historico.Count(h => h.Status == "Abrigado"),
                Historico = historico
            });
        }

        /// <summary>
        /// Obtém pessoas atualmente abrigadas
        /// </summary>
        [HttpGet("ativos")]
        public async Task<ActionResult> GetPessoasAbrigadas()
        {
            var pessoasAbrigadas = await _context.CheckinsAbrigos
                .Include(c => c.Usuario)
                .Include(c => c.Abrigo)
                .Where(c => c.DataSaida == null)
                .Select(c => new {
                    IdCheckin = c.IdCheckin,
                    Usuario = new {
                        Id = c.Usuario.IdUsuario,
                        Nome = c.Usuario.Nome,
                        Email = c.Usuario.Email  
                    },
                    Abrigo = new {
                        Id = c.Abrigo.IdAbrigo,
                        Nome = c.Abrigo.Nome,
                        Endereco = c.Abrigo.Endereco
                    },
                    DataEntrada = c.DataEntrada,
                    TempoAbrigado = DateTime.Now - c.DataEntrada
                })
                .OrderBy(c => c.Abrigo.Nome)
                .ThenBy(c => c.DataEntrada)
                .ToListAsync();

            return Ok(new {
                TotalPessoasAbrigadas = pessoasAbrigadas.Count,
                PessoasAbrigadas = pessoasAbrigadas
            });
        }
    }
}