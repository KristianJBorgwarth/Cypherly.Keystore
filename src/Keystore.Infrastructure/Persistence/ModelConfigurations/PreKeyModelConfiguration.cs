using Keystore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Keystore.Infrastructure.Persistence.ModelConfigurations;

public sealed class PreKeyModelConfiguration : BaseModelConfiguration<PreKey>
{
    public override void Configure(EntityTypeBuilder<PreKey> builder)
    {
        builder.ToTable("pre_key");

        builder.HasKey(pk => pk.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(pk => pk.KeyBundleId)
            .HasColumnName("key_bundle_id")
            .IsRequired();

        builder.Property(x => x.PublicKey)
            .HasColumnName("public_key")
            .IsRequired();

        builder.Property(pk => pk.KeyId)
            .HasColumnName("key_id")
            .IsRequired();

        builder.Property(pk => pk.Consumed)
            .HasColumnName("consumed")
            .IsRequired();
        
        base.Configure(builder);
    }
}