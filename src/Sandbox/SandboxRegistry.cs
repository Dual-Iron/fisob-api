#nullable enable
using Fisobs.Core;
using System.Collections.Generic;

namespace Fisobs.Sandbox
{
    /// <summary>
    /// A registry that stores <see cref="ISandboxHandler"/> instances and the hooks relevant to them.
    /// </summary>
    public sealed partial class SandboxRegistry : Registry
    {
        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        public static SandboxRegistry Instance { get; } = new SandboxRegistry();

        readonly Dictionary<PhysobType, ISandboxHandler> sboxes = new();

        /// <inheritdoc/>
        protected override void Process(IContent content)
        {
            if (content is ISandboxHandler handler) {
                sboxes[handler.Type] = handler;
            }
        }

        /// <inheritdoc/>
        protected override void Initialize()
        {
            // Sandbox bits that apply to both items and creatures
            IL.Menu.SandboxEditorSelector.ctor += AddCustomFisobs;
            On.Menu.SandboxEditorSelector.ctor += ResetWidthAndHeight;
            On.SandboxGameSession.SpawnEntity += SpawnEntity;
            On.MultiplayerUnlocks.SandboxItemUnlocked += IsUnlocked;
            On.MultiplayerUnlocks.SymbolDataForSandboxUnlock += FromUnlock;
            On.MultiplayerUnlocks.SandboxUnlockForSymbolData += FromSymbolData;

            // Sandbox bits that apply to creatures only
            On.Menu.SandboxSettingsInterface.ctor += AddPages;
            On.Menu.SandboxSettingsInterface.DefaultKillScores += DefaultKillScores;
        }
    }
}
