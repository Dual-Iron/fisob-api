namespace Fisobs;

/// <summary>
/// Provides methods to simplify the creation of physical objects.
/// </summary>
public abstract class Fisob
{
    static bool IsValid(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Fisob"/> class.
    /// </summary>
    /// <param name="id">This fisob's unique identifier. This must consist of characters a-z and _ only.</param>
    protected Fisob(string id)
    {
        if (string.IsNullOrEmpty(id)) {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
        }

        if (!id.All(IsValid)) {
            throw new ArgumentException("ID must only consist of a-z and _.");
        }

        ID = id;
    }

    /// <summary>
    /// This fisob's unique identifier.
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// This fisob's enum value.
    /// </summary>
    public AbstractPhysicalObject.AbstractObjectType Type { get; internal set; }

    /// <summary>
    /// Gets an abstract physical object from saved data.
    /// </summary>
    /// <returns>A newly created abstract physical object, or null.</returns>
    public abstract AbstractPhysicalObject? Parse(World world, EntitySaveData saveData);

    /// <summary>
    /// Gets a behavior object for a specified physical object.
    /// </summary>
    /// <returns>A newly created behavior object.</returns>
    public virtual FisobBehavior GetBehavior(PhysicalObject forObject) => FisobBehavior.Default;
}
