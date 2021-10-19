using System;
using System.Linq;
using ObjType = AbstractPhysicalObject.AbstractObjectType;

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
        public readonly ObjType ObjectType;

        /// <summary>
        /// The APO's ID.
        /// </summary>
        public readonly EntityID ID;

        /// <summary>
        /// The APO's position.
        /// </summary>
        public readonly WorldCoordinate Pos;

        /// <summary>
        /// Any extra data associated with the APO. This can be an empty string, but not null.
        /// </summary>
        public readonly string CustomData;

        internal EntitySaveData(ObjType objectType, EntityID id, WorldCoordinate pos, string customData)
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
        public static EntitySaveData CreateFrom(AbstractPhysicalObject apo, string customData)
        {
            if (customData is null) {
                throw new ArgumentNullException(nameof(customData));
            }

            if (customData.Contains('<')) {
                throw new ArgumentException("Custom data cannot contain < characters.");
            }

            return new EntitySaveData(apo.type, apo.ID, apo.pos, customData);
        }

        /// <summary>
        /// Gets this entity's saved data as a string.
        /// </summary>
        /// <returns>A string representation of this data.</returns>
        public override string ToString()
        {
            string customDataStr = string.IsNullOrEmpty(CustomData) ? "" : $"<oA>{CustomData}";
            return $"{ID}<oA>{ObjectType}<oA>{Pos.room}.{Pos.x}.{Pos.y}.{Pos.abstractNode}{customDataStr}";
        }
    }
}