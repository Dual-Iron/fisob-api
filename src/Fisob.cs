using System;
using System.Linq;

#error Right-click "Fisobs" on the line below and click Rename. Once you've renamed it, remove this line.
namespace Fisobs
{
    /// <summary>
    /// Provides methods to simplify the creation of custom <see cref="PhysicalObject"/> and <see cref="AbstractPhysicalObject"/> types.
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
        /// <param name="id">This fisob's case-insensitive unique identifier. This must consist of characters a-z and _ only.</param>
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

        private AbstractPhysicalObject.AbstractObjectType? type;

        /// <summary>
        /// If this fisob has been added to a <see cref="FisobRegistry"/>, <see langword="true"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public bool IsInRegistry => type != null;

        /// <summary>
        /// This fisob's unique identifier.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// This fisob's enum value.
        /// </summary> 
        public AbstractPhysicalObject.AbstractObjectType Type {
            get => type ?? throw new InvalidOperationException($"The fisob \"{ID}\" hasn't been added to a registry yet.");
            internal set {
                if (type == null)
                    type = value;
                else
                    throw new InvalidOperationException($"The fisob \"{ID}\" already has a type.");
            }
        }

        /// <summary>
        /// Gets an APO from saved data.
        /// </summary>
        /// <returns>A newly created abstract physical object, or <see langword="null"/>.</returns>
        public abstract AbstractPhysicalObject Parse(World world, EntitySaveData saveData);

        /// <summary>
        /// Gets an object representing the properties of a PO.
        /// </summary>
        /// <returns>An instance of <see cref="FisobProperties"/>.</returns>
        /// <remarks>Do not return <see langword="null"/> from this method. Return <see cref="FisobProperties.Default"/> instead.</remarks>
        public virtual FisobProperties GetProperties(PhysicalObject forObject) => FisobProperties.Default;
    }
}
