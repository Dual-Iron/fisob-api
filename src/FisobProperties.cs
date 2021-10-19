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
        public static FisobProperties Default { get; } = new();

        /// <summary>
        /// Modifies how highly a scavenger values an item as a collectable.
        /// </summary>
        /// <param name="scavenger">The scavenger.</param>
        /// <param name="score">The score.</param>
        public virtual void GetScavengerCollectScore(Scavenger scavenger, ref int score) { }

        /// <summary>
        /// Modifies how highly a scavenger values an item as a weapon.
        /// </summary>
        /// <param name="scavenger">The scavenger.</param>
        /// <param name="score">The score.</param>
        public virtual void GetScavengerWeaponScore(Scavenger scavenger, ref int score) { }

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