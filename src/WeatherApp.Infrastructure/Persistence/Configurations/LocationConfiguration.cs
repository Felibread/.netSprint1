using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Localizacao");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasColumnName("nome")
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Coordinates, coords =>
        {
            coords.Property(c => c.Latitude).HasColumnName("latitude").HasPrecision(8, 4);
            coords.Property(c => c.Longitude).HasColumnName("longitude").HasPrecision(8, 4);
        });

        builder.HasIndex(x => x.Id).HasDatabaseName("Localizacao__IDX");
    }
}
