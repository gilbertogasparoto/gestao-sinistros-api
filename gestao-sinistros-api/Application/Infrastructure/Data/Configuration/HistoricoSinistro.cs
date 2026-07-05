using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HistoricoSinistroConfiguration : IEntityTypeConfiguration<HistoricoSinistro>
{
    public void Configure(EntityTypeBuilder<HistoricoSinistro> builder)
    {
        builder.ToTable("historico_sinistros");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(h => h.SinistroId)
            .HasColumnName("sinistro_id")
            .IsRequired();

        builder.Property(h => h.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(h => h.DataAlteracao)
            .HasColumnName("data_alteracao")
            .IsRequired();

        builder.HasOne(h => h.Sinistro)
            .WithMany(s => s.Historicos)
            .HasForeignKey(h => h.SinistroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}