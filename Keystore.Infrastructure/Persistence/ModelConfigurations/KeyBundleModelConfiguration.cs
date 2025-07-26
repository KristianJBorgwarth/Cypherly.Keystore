using Keystore.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Keystore.Infrastructure.Persistence.ModelConfigurations;

public sealed class KeyBundleModelConfiguration : BaseModelConfiguration<KeyBundle>
{
    public override void Configure(EntityTypeBuilder<KeyBundle> builder)
    {
        builder.ToTable("key_bundle");

        builder.HasKey(kb => kb.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id");

        builder.Property(kb => kb.AccessKey)
            .HasColumnName("access_key")
            .IsRequired();

        builder.HasIndex(kb => kb.AccessKey)
            .HasDatabaseName("idx_access_key")
            .IsUnique();

        builder.Property(kb => kb.IdentityKey)
            .HasColumnName("identity_key")
            .IsRequired();

        builder.Property(kb => kb.RegistrationId)
            .HasColumnName("registration_id")
            .IsRequired();

        builder.Property(kb => kb.SignedPrekeyId)
            .HasColumnName("signed_pre_key_id")
            .IsRequired();

        builder.Property(kb => kb.SignedPreKeySignature)
            .HasColumnName("signed_pre_key_signature")
            .IsRequired();

        builder.Property(kb => kb.SignedPreKeyTimestamp)
            .HasColumnName("signed_pre_key_timestamp")
            .IsRequired();

        builder.Property(kb => kb.SignedPreKeyPublic)
            .HasColumnName("signed_pre_key_public")
            .IsRequired();

        builder.HasMany(x => x.PreKeys)
            .WithOne(x => x.KeyBundle)
            .HasForeignKey(x => x.KeyBundleId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        
        base.Configure(builder);
    }
}