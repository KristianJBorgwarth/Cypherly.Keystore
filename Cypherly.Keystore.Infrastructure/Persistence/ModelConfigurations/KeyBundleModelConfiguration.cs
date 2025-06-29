using Cypherly.Keystore.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.Keystore.Infrastructure.Persistence.ModelConfigurations;

public sealed class KeyBundleModelConfiguration : IEntityTypeConfiguration<KeyBundle>
{
    public void Configure(EntityTypeBuilder<KeyBundle> builder)
    {
        builder.ToTable("key_bundle");

        builder.HasKey(kb => kb.Id);

        builder.Property(kb => kb.AccessKey)
            .IsRequired();

        builder.HasIndex(kb=> kb.AccessKey)
            .IsUnique();

        builder.Property(kb => kb.IdentityKey)
            .IsRequired();

        builder.Property(kb=> kb.RegistrationId)
            .IsRequired();

        builder.Property(kb => kb.SignedPrekeyId)
            .IsRequired();

        builder.Property(kb => kb.SignedPreKeySignature)
            .IsRequired();

        builder.Property(kb=> kb.SignedPreKeyTimestamp)
            .IsRequired();

        builder.Property(kb => kb.SignedPreKeyPublic)
            .IsRequired();

        builder.HasMany(x => x.PreKeys)
            .WithOne(x => x.KeyBundle)
            .HasForeignKey(x => x.KeyBundleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
    }
}