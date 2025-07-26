using Keystore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Keystore.Infrastructure.Persistence.ModelConfigurations;

public sealed class PreKeyModelConfiguration : IEntityTypeConfiguration<PreKey>
{
    public void Configure(EntityTypeBuilder<PreKey> builder)
    {
        builder.ToTable("pre_key");

        builder.HasKey(pk => pk.Id);

        builder.Property(pk => pk.KeyBundleId)
            .IsRequired();

        builder.Property(x => x.PublicKey)
            .IsRequired();

        builder.Property(pk => pk.KeyId)
            .IsRequired();

        builder.Property(pk => pk.Consumed)
            .IsRequired();
    }
}