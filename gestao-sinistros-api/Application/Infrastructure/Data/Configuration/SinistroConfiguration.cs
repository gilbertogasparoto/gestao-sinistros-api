using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SinistroConfiguration : IEntityTypeConfiguration<Sinistro>
{
    public void Configure(EntityTypeBuilder<Sinistro> builder)
    {
        builder.ToTable("sinistros");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Numero)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.DataOcorrencia)
            .IsRequired();

        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.ValorEstimado)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ValorAprovado)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.Apolice)
            .WithMany(a => a.Sinistros)
            .HasForeignKey(s => s.ApoliceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}