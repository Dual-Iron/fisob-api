namespace Fisobs
{
    /// <summary>
    /// Provides methods to define the properties of custom <see cref="PhysicalObject"/> types.
    /// </summary>
    public class FisobProperties
    {
        /// <summary>
        /// The default behavior for a fisob. Does nothing unique.
        /// </summary>
        public static FisobProperties Default { get; } = new FisobProperties();

        /// <summary>
        /// Modifies how much a scavenger wants to pick up a collectible.
        /// </summary>
        /// <param name="scav">The scavenger.</param>
        /// <param name="score">The score. Items with higher scores will be picked up first.</param>
        public virtual void GetScavCollectibleScore(Scavenger scav, ref int score) { }

        /// <summary>
        /// Modifies how much a scavenger wants to pick up a weapon.
        /// </summary>
        /// <param name="scav">The scavenger.</param>
        /// <param name="score">The score. Items with higher scores will be picked up first.</param>
        public virtual void GetScavWeaponPickupScore(Scavenger scav, ref int score) { }

        /// <summary>
        /// Modifies how much a scavenger wants to use a weapon.
        /// </summary>
        /// <remarks>Bombs, for instance, have a high pickup score but a low use score—they're valuable, but they're a last resort in combat.</remarks>
        /// <param name="scav">The scavenger.</param>
        /// <param name="score">The score. Items with higher scores will be used first.</param>
        public virtual void GetScavWeaponUseScore(Scavenger scav, ref int score) { }

        /// <summary>
        /// Modifies when scavengers will use a weapon. Scavengers only use lethal weapons when they intend to kill.
        /// </summary>
        /// <remarks>In vanilla, only <see cref="Spear"/> and <see cref="ScavengerBomb"/> objects are considered lethal.</remarks>
        /// <param name="scav">The scavenger.</param>
        /// <param name="isLethal">If the item is lethal, <see langword="true"/>; otherwise, <see langword="false"/>.</param>
        public virtual void IsLethalWeapon(Scavenger scav, ref bool isLethal) { }

        /// <summary>
        /// Modifies how easily a player can grasp an item.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="grabability">Whether the object should be light one-handed, bulky one-handed, two-handed, dragged, or unable to be grabbed.</param>
        public virtual void GetGrabability(Player player, ref Player.ObjectGrabability grabability) { }

        /// <summary>
        /// Modifies if the player can throw or toss an object.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="throwable">Whether or not the player can throw or toss the object. For example, vulture masks cannot be thrown.</param>
        public virtual void CanThrow(Player player, ref bool throwable) { }
    }
}