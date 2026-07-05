using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Domain.Entities;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Application.Api.Services
{
    public class ApoliceService
    {
        private readonly AppDbContext _context;

        public ApoliceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<ApoliceResponseDto>> GetAll(GetApolicesParams queryParams)
        {
            var query = _context.Apolices.AsNoTracking().Include(a => a.Cliente).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Keyword))
            {
                var search = queryParams.Keyword.Trim();
                query = query.Where(a =>
                    EF.Functions.ILike(a.Numero, $"%{search}%") ||
                    EF.Functions.ILike(a.Cliente.Nome, $"%{search}%"));
            }

            if (queryParams.TipoSeguro.HasValue)
            {
                query = query.Where(a => a.TipoSeguro == queryParams.TipoSeguro.Value);
            }

            if (queryParams.Ativo.HasValue)
            {
                query = query.Where(a => a.Ativo == queryParams.Ativo.Value);
            }

            var totalItems = await query.CountAsync();
            var apolices = await query
                .OrderBy(a => a.Numero)
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PagedResultDto<ApoliceResponseDto>
            {
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)queryParams.PageSize),
                Items = apolices.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<ApoliceResponseDto?> GetById(Guid id)
        {
            var apolice = await _context.Apolices.AsNoTracking().Include(a => a.Cliente).FirstOrDefaultAsync(a => a.Id == id);
            return apolice != null ? MapToResponseDto(apolice) : null;
        }

        public async Task<ApoliceResponseDto> Create(CreateApoliceDto dto)
        {
            var clienteExists = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExists)
            {
                throw new InvalidOperationException("Cliente não encontrado.");
            }

            var apolice = new Apolice
            {
                Numero = dto.Numero,
                ClienteId = dto.ClienteId,
                TipoSeguro = dto.TipoSeguro,
                Ativo = dto.Ativo,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim
            };

            _context.Apolices.Add(apolice);
            await _context.SaveChangesAsync();

            return MapToResponseDto(apolice);
        }

        public async Task<ApoliceResponseDto?> Update(Guid id, UpdateApoliceDto dto)
        {
            var apolice = await _context.Apolices.FirstOrDefaultAsync(a => a.Id == id);
            if (apolice == null) return null;

            apolice.TipoSeguro = dto.TipoSeguro;
            apolice.Ativo = dto.Ativo;
            apolice.DataInicio = dto.DataInicio;
            apolice.DataFim = dto.DataFim;

            await _context.SaveChangesAsync();
            return MapToResponseDto(apolice);
        }

        public async Task<bool> Delete(Guid id)
        {
            var apolice = await _context.Apolices.FirstOrDefaultAsync(a => a.Id == id);
            if (apolice == null) return false;

            _context.Apolices.Remove(apolice);
            await _context.SaveChangesAsync();
            return true;
        }

        private ApoliceResponseDto MapToResponseDto(Apolice apolice)
        {
            return new ApoliceResponseDto
            {
                Id = apolice.Id,
                Numero = apolice.Numero,
                ClienteId = apolice.ClienteId,
                TipoSeguro = apolice.TipoSeguro,
                Ativo = apolice.Ativo,
                DataInicio = apolice.DataInicio,
                DataFim = apolice.DataFim
            };
        }
    }
}
