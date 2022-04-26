#nullable enable
using Fisobs.Properties;
using Fisobs.Core;
using Fisobs.Sandbox;
using System.Collections.Generic;
using ObjectType = AbstractPhysicalObject.AbstractObjectType;

namespace Fisobs.Items
{
    /// <summary>
    /// Represents the "metadata" for a custom item.
    /// </summary>
    public abstract class Fisob : IContent, IPropertyHandler, ISandboxHandler
    {
        private readonly List<SandboxUnlock> sandboxUnlocks = new();

        /// <summary>
        /// Creates a new <see cref="Fisob"/> instance for the given <paramref name="type"/>.
        /// </summary>
        protected Fisob(ObjectType type)
        {
            Type = type;
        }

        /// <summary>The fisob's type.</summary>
        public ObjectType Type { get; }
        /// <summary>The fisob's icon; a <see cref="DefaultIcon"/> by default.</summary>
        /// <remarks>When <see cref="LoadResources(RainWorld)"/> is called, an embedded resource with the name <c>$"icon_{Type}"</c> will be auto-loaded as a <see cref="SimpleIcon"/>, if it exists.</remarks>
        public Icon Icon { get; set; } = new DefaultIcon();

        /// <summary>
        /// Gets a new <see cref="AbstractPhysicalObject"/> instance from custom data.
        /// </summary>
        /// <param name="world">The world the entity lives in.</param>
        /// <param name="entitySaveData">The entity's save data.</param>
        /// <param name="unlock">The sandbox unlock that spawned this entity, or <see langword="null"/> if the entity wasn't spawned by one.</param>
        public abstract AbstractPhysicalObject Parse(World world, EntitySaveData entitySaveData, SandboxUnlock? unlock);

        /// <inheritdoc/>
        public virtual ItemProperties? Properties(PhysicalObject forObject) => null;
        
        /// <summary>
        /// Loads <see cref="FAtlas"/> and <see cref="FAtlasElement"/> sprites. The <see cref="Ext.LoadAtlasFromEmbRes(System.Reflection.Assembly, string)"/> is recommended for this.
        /// </summary>
        /// <param name="rainWorld">The current <see cref="RainWorld"/> instance.</param>
        public virtual void LoadResources(RainWorld rainWorld)
        {
            string iconName = Ext.LoadAtlasFromEmbRes(GetType().Assembly, $"icon_{Type}")?.name ?? "Futile_White";

            if (Icon is DefaultIcon) {
                Icon = new SimpleIcon(iconName, Ext.MenuGrey);
            }
        }

        /// <summary>
        /// Registers a sandbox unlock under this fisob.
        /// </summary>
        public void RegisterUnlock(SandboxUnlock unlock)
        {
            sandboxUnlocks.Add(unlock);
        }

        PhysobType IPropertyHandler.Type => Type;
        PhysobType ISandboxHandler.Type => Type;

        IList<SandboxUnlock> ISandboxHandler.SandboxUnlocks => sandboxUnlocks;

        IEnumerable<Registry> IContent.Registries()
        {
            yield return FisobRegistry.Instance;
            yield return PropertyRegistry.Instance;
            yield return SandboxRegistry.Instance;
        }

        AbstractWorldEntity ISandboxHandler.ParseFromSandbox(World world, EntitySaveData data, SandboxUnlock unlock)
        {
            return Parse(world, data, unlock);
        }
    }
}
