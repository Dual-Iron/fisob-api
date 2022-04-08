#nullable enable

namespace Fisobs.Sandbox
{
    /// <summary>
    /// Represents a sandbox unlock.
    /// </summary>
    public class SandboxUnlock
    {
        /// <summary>
        /// The sandbox unlock's data value. This takes the place of <see cref="Core.Icon.Data(AbstractPhysicalObject)"/> when spawning objects from sandbox mode.
        /// </summary>
        public int Data { get; }

        /// <summary>
        /// The creature unlock's kill score. This is ignored for items.
        /// </summary>
        public CreatureKillScore KillScore { get; }

        /// <summary>
        /// The sandbox unlock type.
        /// </summary>
        public MultiplayerUnlocks.SandboxUnlockID Type { get; }

        /// <summary>
        /// Creates a new <see cref="SandboxUnlock"/>.
        /// </summary>
        /// <param name="type">The sandbox unlock type.</param>
        public SandboxUnlock(MultiplayerUnlocks.SandboxUnlockID type) : this(type, 0, new(0, true))
        { }

        /// <summary>
        /// Creates a new <see cref="SandboxUnlock"/>.
        /// </summary>
        /// <param name="type">The sandbox unlock type.</param>
        /// <param name="data">The sandbox unlock's data value. This takes the place of <see cref="Core.Icon.Data(AbstractPhysicalObject)"/> when spawning objects from sandbox mode.</param>
        public SandboxUnlock(MultiplayerUnlocks.SandboxUnlockID type, int data) : this(type, data, new(0, true))
        { }

        /// <summary>
        /// Creates a new <see cref="SandboxUnlock"/>.
        /// </summary>
        /// <param name="type">The sandbox unlock type.</param>
        /// <param name="data">The sandbox unlock's data value. This takes the place of <see cref="Core.Icon.Data(AbstractPhysicalObject)"/> when spawning objects from sandbox mode.</param>
        /// <param name="killScore">The creature unlock's kill score. This is ignored for items.</param>
        public SandboxUnlock(MultiplayerUnlocks.SandboxUnlockID type, int data, CreatureKillScore killScore)
        {
            Type = type;
            Data = data;
            KillScore = killScore;
        }

        /// <summary>
        /// Determines if the sandbox unlock is actually unlocked and can be used in sandbox mode.
        /// </summary>
        /// <remarks>Unless overridden, this always returns true.</remarks>
        public virtual bool IsUnlocked(MultiplayerUnlocks unlocks) => true;
    }
}
