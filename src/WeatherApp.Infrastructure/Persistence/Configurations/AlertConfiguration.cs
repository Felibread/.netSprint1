using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;

namespace WeatherApp.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerta");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasColumnName("tipo_alerta").HasMaxLength(50);
        builder.Property(x => x.Title).HasColumnName("titulo").HasMaxLength(80);
        builder.Property(x => x.Message).HasColumnName("mensagem").HasMaxLength(400);
        builder.Property(x => x.CreatedAt).HasColumnName("data_envio");
        builder.Property(x => x.ExpiresAt).HasColumnName("expira_em");

        builder.HasIndex(x => x.Id).HasDatabaseName("Alerta__IDX");
    }
}
