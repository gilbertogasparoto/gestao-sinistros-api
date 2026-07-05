using gestao_sinistros_api.Application.Api.Commom;
using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.DTOs.Sinistro;
using gestao_sinistros_api.Application.Domain.Entities;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace gestao_sinistros_api.Application.Api.Services
{
    public class SinistroService
    {
        private readonly AppDbContext _context;

        public SinistroService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<SinistroResponseDto>> GetAll(GetSinistrosParams queryParams)
        {
            var pageNumber = queryParams.PageNumber <= 0 ? 1 : queryParams.PageNumber;
            var pageSize = queryParams.PageSize <= 0 ? 10 : queryParams.PageSize;

            var query = _context.Sinistros.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Keyword))
            {
                query = query.Where(s => EF.Functions.ILike(s.Numero, $"%{queryParams.Keyword.Trim()}%"));
            }

            if (queryParams.Status.HasValue)
            {
                query = query.Where(s => s.Status == queryParams.Status.Value);
            }

            if (queryParams.DataInicio.HasValue)
            {
                query = query.Where(s => s.DataOcorrencia >= queryParams.DataInicio.Value);
            }

            if (queryParams.DataFim.HasValue)
            {
                query = query.Where(s => s.DataOcorrencia <= queryParams.DataFim.Value);
            }

            var totalItems = await query.CountAsync();
            var sinistros = await query
                .OrderByDescending(s => s.DataOcorrencia)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<SinistroResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = sinistros.Select(MapToResponseDto).ToList()
            };
        }

        public async Task<SinistroResponseDto?> GetById(Guid id)
        {
            var sinistro = await _context.Sinistros.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            return sinistro != null ? MapToResponseDto(sinistro) : null;
        }

        public async Task<List<HistoricoSinistroResponseDto>> GetHistorico(Guid sinistroId)
        {
            var historico = await _context.HistoricoSinistros
                .AsNoTracking()
                .Where(h => h.SinistroId == sinistroId)
                .OrderByDescending(h => h.DataAlteracao)
                .ToListAsync();

            return historico.Select(MapToHistoricoResponseDto).ToList();
        }

        public async Task<SinistroResponseDto> Create(CreateSinistroDto dto)
        {
            var apolice = await _context.Apolices.FirstOrDefaultAsync(a => a.Id == dto.ApoliceId);
            if (apolice == null)
            {
                throw new BusinessException("Apólice não encontrada.");
            }

            if (!apolice.Ativo || apolice.DataFim < DateTime.UtcNow.Date)
            {
                throw new BusinessException("A apólice está inativa ou expirada para abertura de sinistro.");
            }

            var sinistro = new Sinistro
            {
                Numero = dto.Numero,
                ApoliceId = dto.ApoliceId,
                Descricao = dto.Descricao,
                Status = SinistroStatus.Aberto,
                DataOcorrencia = dto.DataOcorrencia,
                ValorEstimado = dto.ValorEstimado,
                ValorAprovado = 0,
                MotivoNegacao = null
            };

            _context.Sinistros.Add(sinistro);
            await _context.SaveChangesAsync();

            await AddHistoricoAsync(sinistro.Id, SinistroStatus.Aberto, "Sinistro criado.");

            return MapToResponseDto(sinistro);
        }

        public async Task<SinistroResponseDto?> Update(Guid id, UpdateSinistroDto dto)
        {
            var sinistro = await _context.Sinistros.FirstOrDefaultAsync(s => s.Id == id);
            if (sinistro == null) return null;

            sinistro.Descricao = dto.Descricao;
            sinistro.DataOcorrencia = dto.DataOcorrencia;
            sinistro.ValorEstimado = dto.ValorEstimado;

            await _context.SaveChangesAsync();
            return MapToResponseDto(sinistro);
        }

        public async Task<SinistroResponseDto?> ChangeStatus(Guid id, ChangeStatusSinistroDto dto)
        {
            var sinistro = await _context.Sinistros.FirstOrDefaultAsync(s => s.Id == id);
            if (sinistro == null) return null;

            if (sinistro.Status == dto.Status)
            {
                throw new BusinessException($"O sinistro já está com o status {dto.Status}.");
            }

            if (!AllowedTransitions[sinistro.Status].Contains(dto.Status))
            {
                throw new BusinessException(
                    $"Não é permitido alterar o status de {sinistro.Status} para {dto.Status}.");
            }

            ValidateChangeStatusFields(dto);


            sinistro.Status = dto.Status;
            sinistro.ValorAprovado = dto.ValorAprovado ?? sinistro.ValorAprovado;
            sinistro.MotivoNegacao = dto.Status == SinistroStatus.Negado ? dto.MotivoNegacao : null;

            await _context.SaveChangesAsync();
            await AddHistoricoAsync(sinistro.Id, dto.Status, dto.Observacao ?? $"Status alterado para {dto.Status}.");


            return MapToResponseDto(sinistro);
        }

        public async Task<bool> Delete(Guid id)
        {
            var sinistro = await _context.Sinistros.FirstOrDefaultAsync(s => s.Id == id);
            if (sinistro == null) return false;

            _context.Sinistros.Remove(sinistro);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task AddHistoricoAsync(Guid sinistroId, SinistroStatus status, string observacao)
        {
            var historico = new HistoricoSinistro
            {
                SinistroId = sinistroId,
                Status = status,
                Observacao = observacao,
                DataAlteracao = DateTime.UtcNow
            };

            _context.HistoricoSinistros.Add(historico);
            await _context.SaveChangesAsync();
        }

        private SinistroResponseDto MapToResponseDto(Sinistro sinistro)
        {
            return new SinistroResponseDto
            {
                Id = sinistro.Id,
                Numero = sinistro.Numero,
                ApoliceId = sinistro.ApoliceId,
                Descricao = sinistro.Descricao,
                Status = sinistro.Status,
                DataOcorrencia = sinistro.DataOcorrencia,
                ValorEstimado = sinistro.ValorEstimado,
                ValorAprovado = sinistro.ValorAprovado,
                MotivoNegacao = sinistro.MotivoNegacao
            };
        }

        private HistoricoSinistroResponseDto MapToHistoricoResponseDto(HistoricoSinistro historico)
        {
            return new HistoricoSinistroResponseDto
            {
                Id = historico.Id,
                SinistroId = historico.SinistroId,
                Status = historico.Status,
                Observacao = historico.Observacao,
                DataAlteracao = historico.DataAlteracao
            };
        }

        private static readonly Dictionary<SinistroStatus, SinistroStatus[]> AllowedTransitions = new()
        {
            { SinistroStatus.Aberto, [SinistroStatus.EmAnalise] },
            { SinistroStatus.EmAnalise, [SinistroStatus.Aprovado, SinistroStatus.Negado] },
            { SinistroStatus.Aprovado, [SinistroStatus.Encerrado] },
            { SinistroStatus.Encerrado, [] },
            { SinistroStatus.Negado, [] }
        };

        private static void ValidateChangeStatusFields(ChangeStatusSinistroDto dto)
        {
            switch (dto.Status)
            {
                case SinistroStatus.Encerrado:
                    if (dto.ValorAprovado is null)
                        throw new BusinessException("O valorAprovado é obrigatório ao encerrar um sinistro.");

                    if (!string.IsNullOrWhiteSpace(dto.MotivoNegacao))
                        throw new BusinessException("motivoNegacao só pode ser informado quando o sinistro é negado.");
                    break;
                case SinistroStatus.Negado:
                    if (string.IsNullOrWhiteSpace(dto.MotivoNegacao))
                        throw new BusinessException("O motivoNegacao é obrigatório ao negar um sinistro.");

                    if (dto.ValorAprovado is not null)
                        throw new BusinessException("valorAprovado não pode ser informado para um sinistro negado.");
                    break;
                default:
                    if (dto.ValorAprovado is not null)
                        throw new BusinessException("valorAprovado só pode ser informado ao encerrar um sinistro.");

                    if (!string.IsNullOrWhiteSpace(dto.MotivoNegacao))
                        throw new BusinessException("motivoNegacao só pode ser informado ao negar um sinistro.");
                    break;
            }
        }
    }
}

