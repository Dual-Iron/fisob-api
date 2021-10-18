using ObjType = AbstractPhysicalObject.AbstractObjectType;

namespace Fisobs;

/// <summary>
/// Represents saved information about <see cref="AbstractPhysicalObject"/> instances.
/// </summary>
public struct EntitySaveData
{
    /// <summary>
    /// The abstract object's type.
    /// </summary>
    public readonly ObjType ObjectType;

    /// <summary>
    /// The abstract object's ID.
    /// </summary>
    public readonly EntityID ID;

    /// <summary>
    /// The abstract object's position.
    /// </summary>
    public readonly WorldCoordinate Pos;

    /// <summary>
    /// Any extra data associated with the object. This can be an empty string, but not null.
    /// </summary>
    public readonly string ExtraData;

    internal EntitySaveData(ObjType objectType, EntityID id, WorldCoordinate pos, string extraData)
    {
        ObjectType = objectType;
        ID = id;
        Pos = pos;
        ExtraData = extraData;
    }

    /// <summary>
    /// Creates an instance of the <see cref="EntitySaveData"/> struct.
    /// </summary>
    /// <param name="o">The abstract physical object to get basic data from.</param>
    /// <param name="extraData">Extra data associated with the physical object. This data should never contain &lt; characters.</param>
    /// <returns>A new instance of <see cref="EntitySaveData"/>.</returns>
    /// <exception cref="ArgumentException"/>
    public static EntitySaveData CreateFrom(AbstractPhysicalObject o, string extraData)
    {
        if (extraData is null) {
            throw new ArgumentNullException(nameof(extraData));
        }

        if (extraData.Contains('<')) {
            throw new ArgumentException("Custom data cannot contain < characters.");
        }

        return new(o.type, o.ID, o.pos, extraData);
    }
}
