using gestao_sinistros_api.Application.Api.Commom;
using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.DTOs.Cliente;
using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Application.Api.Services
{
    public class ClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<ClienteResponseDto>> GetAll(GetClientesParams queryParams)
        {
            var pageNumber = queryParams.PageNumber <= 0 ? 1 : queryParams.PageNumber;
            var pageSize = queryParams.PageSize <= 0 ? 10 : queryParams.PageSize;

            var query = _context.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Keyword))
            {
                var search = queryParams.Keyword.Trim();
                query = query.Where(c =>
                    EF.Functions.ILike(c.Nome, $"%{search}%") ||
                    EF.Functions.ILike(c.Email, $"%{search}%") ||
                    EF.Functions.ILike(c.Documento, $"%{search}%") ||
                    EF.Functions.ILike(c.Telefone, $"%{search}%"));
            }

            var totalItems = await query.CountAsync();
            var clientes = await query
                .OrderBy(c => c.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<ClienteResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = clientes.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<ClienteResponseDto?> GetById(Guid id)
        {
            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            return cliente != null ? MapToResponseDto(cliente) : null;
        }

        public async Task<ClienteResponseDto> Create(UpsertClienteDto dto)
        {
            var emailExists = await _context.Clientes.AnyAsync(c => c.Email == dto.Email);
            if (emailExists)
                throw new BusinessException("Já existe um cliente com este email.");

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Documento = dto.Documento,
                Email = dto.Email,
                Telefone = dto.Telefone
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return MapToResponseDto(cliente);
        }

        public async Task<ClienteResponseDto?> Update(Guid id, UpsertClienteDto dto)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return null;

            // Verifica se o email já existe e não pertence ao cliente atual
            if (cliente.Email != dto.Email)
            {
                var emailExists = await _context.Clientes.AnyAsync(c => c.Email == dto.Email);
                if (emailExists)
                    throw new BusinessException("Já existe um cliente com este email.");
            }

            cliente.Nome = dto.Nome;
            cliente.Documento = dto.Documento;
            cliente.Email = dto.Email;
            cliente.Telefone = dto.Telefone;

            await _context.SaveChangesAsync();

            return MapToResponseDto(cliente);
        }

        public async Task<bool> Delete(Guid id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return true;
        }

        private ClienteResponseDto MapToResponseDto(Cliente cliente)
        {
            return new ClienteResponseDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Documento = cliente.Documento,
                Email = cliente.Email,
                Telefone = cliente.Telefone
            };
        }
    }
}