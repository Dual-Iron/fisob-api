#nullable enable

namespace Fisobs.Sandbox
{
    /// <summary>
    /// A creature unlock's score when killed in hunter and arena modes.
    /// </summary>
    public readonly struct CreatureKillScore
    {
        /// <summary>
        /// The score gained when the creature is killed.
        /// </summary>
        public readonly int Value { get; }

        /// <summary>
        /// If true, the score is hidden from the sandbox mode config.
        /// </summary>
        /// <remarks>Leviathans, for example, do not have a configurable score, so they are hidden from the config.</remarks>
        public readonly bool Hidden { get; }

        /// <summary>
        /// Creates a new <see cref="CreatureKillScore"/> instance.
        /// </summary>
        /// <param name="score">The score gained when the creature is killed.</param>
        /// <param name="hidden">If true, the score will be hidden from the sandbox menu.</param>
        public CreatureKillScore(int score, bool hidden)
        {
            Value = score;
            Hidden = hidden;
        }
    }
}
