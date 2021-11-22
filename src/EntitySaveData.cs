using System;

namespace Fisobs
{
    /// <summary>
    /// Represents saved information about <see cref="AbstractPhysicalObject"/> instances.
    /// </summary>
    public readonly struct EntitySaveData
    {
        /// <summary>
        /// The APO's type.
        /// </summary>
        public readonly AbstractPhysicalObject.AbstractObjectType ObjectType;

        /// <summary>
        /// The APO's ID.
        /// </summary>
        public readonly EntityID ID;

        /// <summary>
        /// The APO's position.
        /// </summary>
        public readonly WorldCoordinate Pos;

        /// <summary>
        /// Any extra data associated with the APO. This can be <see cref="string.Empty"/>, but not <see langword="null"/>.
        /// </summary>
        public readonly string CustomData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySaveData"/> struct.
        /// </summary>
        /// <remarks>Do not use this constructor. Call <see cref="CreateFrom(AbstractPhysicalObject, string)"/> instead.</remarks>
        internal EntitySaveData(AbstractPhysicalObject.AbstractObjectType objectType, EntityID id, WorldCoordinate pos, string customData)
        {
            ObjectType = objectType;
            ID = id;
            Pos = pos;
            CustomData = customData;
        }

        /// <summary>
        /// Creates an instance of the <see cref="EntitySaveData"/> struct.
        /// </summary>
        /// <param name="apo">The abstract physical object to get basic data from.</param>
        /// <param name="customData">Extra data associated with the abstract physical object. This data should never contain &lt; characters.</param>
        /// <returns>A new instance of <see cref="EntitySaveData"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="customData"/> contains &lt; characters.</exception>
        public static EntitySaveData CreateFrom(AbstractPhysicalObject apo, string customData = "")
        {
            if (customData is null) {
                throw new ArgumentNullException(nameof(customData));
            }

            if (customData.IndexOf('<') != -1) {
                throw new ArgumentException("Custom data cannot contain < characters.");
            }

            return new EntitySaveData(apo.type, apo.ID, apo.pos, customData);
        }

        /// <summary>
        /// Gets this entity's save data as a string.
        /// </summary>
        /// <returns>A string representation of this data.</returns>
        public override string ToString()
        {
            string customDataStr = string.IsNullOrEmpty(CustomData) ? "" : $"<oA>{CustomData}";
            return $"{ID}<oA>{ObjectType}<oA>{Pos.room}.{Pos.x}.{Pos.y}.{Pos.abstractNode}{customDataStr}";
        }
    }
}