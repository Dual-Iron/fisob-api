using System.Collections.Generic;

namespace Fisobs.Core
{
    /// <summary>
    /// Registries hook into the vanilla game to simplify content creation.
    /// </summary>
    public abstract class Registry
    {
        private bool applied;

        /// <summary>
        /// Processes some content. The registry must ignore content not relevant to it and should throw an exception if content is relevant but malformed.
        /// </summary>
        /// <param name="content">The content entry to process.</param>
        protected abstract void Process(IList<IContent> content);

        internal void ProcessInternal(IList<IContent> content)
        {
            Process(content);
        }

        /// <summary>
        /// Should contain initialization logic. This is called a maximum of once and before <see cref="Process(IContent)"/>. Should be used to apply things like MonoMod hooks.
        /// </summary>
        protected abstract void Initialize();

        internal void InitializeInternal()
        {
            if (!applied) {
                applied = true;
                Initialize();
            }
        }
    }
}
