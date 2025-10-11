using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Enums;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Infrastructure.Persistence.Configurations;

public class WeatherReadingConfiguration : IEntityTypeConfiguration<WeatherReading>
{
    public void Configure(EntityTypeBuilder<WeatherReading> builder)
    {
        builder.ToTable("Clima");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ObservedAt).HasColumnName("data_hora").IsRequired();
        builder.Property(x => x.Temperature).HasColumnName("temp").HasPrecision(8, 2);
        builder.Property(x => x.TemperatureUnit).HasConversion<string>().HasColumnName("temp_unit");
        builder.Property(x => x.HumidityPercent).HasColumnName("humidity");
        builder.Property(x => x.WindSpeedKmh).HasColumnName("wind_speed").HasPrecision(8, 2);
        builder.Property(x => x.Condition).HasConversion<string>().HasColumnName("condition");

        builder.OwnsOne(x => x.PrecipitationProbability, prob =>
        {
            prob.Property(p => p.Value).HasColumnName("precip_prob").HasPrecision(6, 4);
        });

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey("id_localizacao")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Id).HasDatabaseName("Clima__IDX");
    }
}
