using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApoliceConfiguration : IEntityTypeConfiguration<Apolice>
{
    public void Configure(EntityTypeBuilder<Apolice> builder)
    {
        builder.ToTable("apolices");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Numero)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.TipoSeguro)
            .IsRequired();

        builder.HasOne(a => a.Cliente)
            .WithMany(c => c.Apolices)
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}