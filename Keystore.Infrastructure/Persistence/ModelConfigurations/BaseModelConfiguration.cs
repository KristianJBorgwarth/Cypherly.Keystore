using Keystore.Domain.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Keystore.Infrastructure.Persistence.ModelConfigurations;

/// <summary>
/// Column configuration for base entity values:
/// <see cref="Created"/>
/// <see cref="Entity.Updated"/>
/// <see cref="Entity.Deleted"/> 
/// </summary>
/// <typeparam name="TModel"></typeparam>
public abstract class BaseModelConfiguration<TModel> : IEntityTypeConfiguration<TModel> where TModel : Entity 
{
    public virtual void Configure(EntityTypeBuilder<TModel> builder)
    {
        builder.Property(x => x.Created)
            .IsRequired()
            .HasColumnName("created");

        builder.Property(x => x.Updated)
            .HasColumnName("updated");
        
        builder.Property(x=> x.Deleted) 
            .HasColumnName("deleted");
    }
}
