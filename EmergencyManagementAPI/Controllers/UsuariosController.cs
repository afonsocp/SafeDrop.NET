using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmergencyManagementAPI.Data;
using EmergencyManagementAPI.Models;
using EmergencyManagementAPI.DTOs;

namespace EmergencyManagementAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de usuários do sistema de emergência
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os usuários cadastrados no sistema
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Retorna a lista de usuários</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioResponseDto
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Email = u.Email,
                    TipoUsuario = u.TipoUsuario,
                    DataCadastro = u.DataCadastro
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        /// <summary>
        /// Obtém um usuário específico por ID
        /// </summary>
        /// <param name="id">ID único do usuário</param>
        /// <returns>Dados do usuário encontrado</returns>
        /// <response code="200">Retorna o usuário encontrado</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var usuarioDto = new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataCadastro = usuario.DataCadastro
            };

            return Ok(usuarioDto);
        }

        /// <summary>
        /// Cria um novo usuário no sistema
        /// </summary>
        /// <param name="usuarioCreateDto">Dados do usuário a ser criado</param>
        /// <returns>Usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario(UsuarioCreateDto usuarioCreateDto)
        {
            // Validar se email já existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioCreateDto.Email))
            {
                return BadRequest("Email já está em uso");
            }

            var usuario = new Usuario
            {
                Nome = usuarioCreateDto.Nome,
                Email = usuarioCreateDto.Email,
                Senha = usuarioCreateDto.Senha, // Em produção, usar hash
                TipoUsuario = usuarioCreateDto.TipoUsuario,
                DataCadastro = DateTime.Now
            };
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var responseDto = new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataCadastro = usuario.DataCadastro
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, responseDto);
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioUpdateDto usuarioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Verificar se o novo email já está em uso por outro usuário
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email && u.IdUsuario != id))
            {
                return BadRequest("Email já está em uso");
            }

            usuario.Nome = usuarioDto.Nome;
            usuario.Email = usuarioDto.Email;
            usuario.TipoUsuario = usuarioDto.TipoUsuario;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtém usuários por tipo
        /// </summary>
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuariosPorTipo(string tipo)
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.TipoUsuario == tipo)
                .Select(u => new UsuarioResponseDto
                {
                    IdUsuario = u.IdUsuario,
                    Nome = u.Nome,
                    Email = u.Email,
                    TipoUsuario = u.TipoUsuario,
                    DataCadastro = u.DataCadastro
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}