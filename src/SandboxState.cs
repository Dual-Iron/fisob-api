namespace Fisobs
{
    /// <summary>
    /// Represents how an object should be displayed in sandbox mode.
    /// </summary>
    public enum SandboxState
    {
        /// <summary>
        /// The object is not listed in sandbox mode.
        /// </summary>
        Hidden,
        /// <summary>
        /// The object is listed but not usable in sandbox mode.
        /// </summary>
        Locked,
        /// <summary>
        /// The object is usable in sandbox mode.
        /// </summary>
        Unlocked
    }
}
