using System;
using System.Collections.Generic;

namespace Fisobs.Core
{
    /// <summary>
    /// Used to register custom content.
    /// </summary>
    public static class Content
    {
        /// <summary>
        /// Registers some content. Call this from your mod's entry point.
        /// </summary>
        /// <param name="content">A bunch of content. Currently, fisobs provides <see cref="Items.Fisob"/> and <see cref="Creatures.Critob"/> for content types.</param>
        public static void Register(params IContent[] content)
        {
            if (UnityEngine.Object.FindObjectOfType<RainWorld>() is RainWorld r && r.processManager != null) {
                TryRegister(content);
            }
            else {
                On.RainWorld.hook_Start? hook = null;

                On.RainWorld.Start += hook = (orig, self) => {
                    TryRegister(content);
                    orig(self);
                    On.RainWorld.Start -= hook;
                };
            }
        }

        private static void TryRegister(IContent[] content)
        {
            try {
                DoRegister(content);
            } catch (Exception e) {
                UnityEngine.Debug.LogException(e);
                Console.WriteLine(e);
                throw;
            }
        }

        private static void DoRegister(IContent[] entries)
        {
            Dictionary<Registry, List<IContent>> registries = new();

            foreach (var entry in entries) {
                foreach (var registry in entry.Registries()) {
                    if (registries.TryGetValue(registry, out List<IContent> list)) {
                        list.Add(entry);
                    } else {
                        registries[registry] = new List<IContent> { entry };
                    }
                }
            }

            foreach (var reg in registries) {
                reg.Key.InitializeInternal();
                reg.Key.ProcessInternal(reg.Value);
            }
        }
    }
}
