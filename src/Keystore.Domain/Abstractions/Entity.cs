// ReSharper disable ConvertConstructorToMemberInitializers
namespace Keystore.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTime Updated { get; private set; }
    public DateTime Created { get; private set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {

    }

    public void SetCreated() => Created = DateTime.UtcNow;

    public void SetLastUpdated() => Updated = DateTime.UtcNow;
}
