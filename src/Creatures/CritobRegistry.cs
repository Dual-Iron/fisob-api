#nullable enable
using Fisobs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CreatureType = CreatureTemplate.Type;

namespace Fisobs.Creatures
{
    /// <summary>
    /// A registry that stores <see cref="Critob"/> instances and the hooks relevant to them.
    /// </summary>
    public sealed class CritobRegistry : Registry
    {
        /// <summary>
        /// The singleton instance of this class.
        /// </summary>
        public static CritobRegistry Instance { get; } = new CritobRegistry();

        readonly Dictionary<CreatureType, Critob> critobs = new();

        /// <inheritdoc/>
        protected override void Process(IContent entry)
        {
            if (entry is Critob critob) {
                critobs[critob.Type] = critob;
            }
        }

        /// <inheritdoc/>
        protected override void Initialize()
        {
            On.RainWorld.Start += ApplyCustomCreaturesOnStart;
            On.RainWorld.LoadResources += LoadResources;

            On.Player.Grabbed += PlayerGrabbed;
            On.AbstractCreature.Realize += Realize;
            On.AbstractCreature.InitiateAI += InitiateAI;
            On.AbstractCreature.ctor += Ctor;
            On.CreatureSymbol.DoesCreatureEarnATrophy += KillsMatter;

            On.CreatureSymbol.SymbolDataFromCreature += CreatureSymbol_SymbolDataFromCreature;
            On.CreatureSymbol.ColorOfCreature += CreatureSymbol_ColorOfCreature;
            On.CreatureSymbol.SpriteNameOfCreature += CreatureSymbol_SpriteNameOfCreature;
        }

        private void ApplyCustomCreatures()
        {
            var newTemplates = new List<CreatureTemplate>();

            // Get new critob templates
            foreach (Critob critob in critobs.Values) {
                var templates = critob.GetTemplates()?.ToList() ?? throw new InvalidOperationException($"Critob \"{critob.Type}\" returned null in GetTemplates().");

                if (!templates.Any(t => t.type == critob.Type)) {
                    throw new InvalidOperationException($"Critob \"{critob.Type}\" does not have a template for its type, \"CreatureTemplate.Type::{critob.Type}\".");
                }
                if (templates.FirstOrDefault(t => t.TopAncestor().type != critob.Type) is CreatureTemplate offender) {
                    throw new InvalidOperationException($"The template with type \"{offender.type}\" from critob \"{critob.Type}\" must have an ancestor of type \"CreatureTemplate.Type::{critob.Type}\".");
                }

                newTemplates.AddRange(templates);
            }

            // Add new critob templates to StaticWorld.creatureTemplates
            Array.Resize(ref StaticWorld.creatureTemplates, StaticWorld.creatureTemplates.Length + newTemplates.Count);

            foreach (CreatureTemplate extraTemplate in newTemplates) {
                // Make sure we're not overwriting vanilla or causing index-out-of-bound errors
                if ((int)extraTemplate.type < 46) {
                    throw new InvalidOperationException($"The CreatureTemplate.Type value {extraTemplate.type} ({(int)extraTemplate.type}) must be greater than 45 to not overwrite vanilla.");
                }
                if ((int)extraTemplate.type >= StaticWorld.creatureTemplates.Length) {
                    throw new InvalidOperationException(
                        $"The CreatureTemplate.Type value {extraTemplate.type} ({(int)extraTemplate.type}) must be less than StaticWorld.creatureTemplates.Length ({StaticWorld.creatureTemplates.Length}).");
                }
                StaticWorld.creatureTemplates[(int)extraTemplate.type] = extraTemplate;
            }

            // Avoid null refs at all costs here
            int nullIndex = StaticWorld.creatureTemplates.IndexOf(null);
            if (nullIndex != -1) {
                throw new InvalidOperationException($"StaticWorld.creatureTemplates has a null value at index {nullIndex}.");
            }

            // Add default relationship to existing creatures
            foreach (CreatureTemplate template in StaticWorld.creatureTemplates) {
                int oldRelationshipsLength = template.relationships.Length;

                Array.Resize(ref template.relationships, StaticWorld.creatureTemplates.Length);

                for (int i = oldRelationshipsLength; i < StaticWorld.creatureTemplates.Length; i++) {
                    template.relationships[i] = template.relationships[0];
                }
            }

            foreach (Critob critob in critobs.Values) {
                critob.EstablishRelationships();
            }
        }

        private void ApplyCustomCreaturesOnStart(On.RainWorld.orig_Start orig, RainWorld self)
        {
            orig(self);
            try {
                ApplyCustomCreatures();
            } catch (Exception e) {
                Debug.LogException(e);
                Debug.LogError($"An exception was thrown in {nameof(Fisobs)}.{nameof(Creatures)}::{nameof(ApplyCustomCreatures)} with details logged.");
                throw;
            }
        }

        private void LoadResources(On.RainWorld.orig_LoadResources orig, RainWorld self)
        {
            orig(self);

            foreach (var common in critobs.Values) {
                common.LoadResources(self);
            }
        }

        private void PlayerGrabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp g)
        {
            orig(self, g);

            if (g?.grabber?.abstractCreature != null && critobs.TryGetValue(g.grabber.abstractCreature.creatureTemplate.TopAncestor().type, out var critob) && critob.GraspParalyzesPlayer(g)) {
                self.dangerGraspTime = 0;
                self.dangerGrasp = g;
            }
        }

        private void InitiateAI(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature self)
        {
            orig(self);

            if (critobs.TryGetValue(self.creatureTemplate.TopAncestor().type, out var crit)) {
                if (self.abstractAI != null && self.creatureTemplate.AI) {
                    self.abstractAI.RealAI = crit.GetRealizedAI(self) ?? throw new InvalidOperationException($"{crit.GetType()}::GetRealizedAI returned null but template.AI was true!");
                } else if (!self.creatureTemplate.AI && crit.GetRealizedAI(self) != null) {
                    Debug.LogError($"{crit.GetType()}::GetRealizedAI returned a non-null object but template.AI was false!");
                }
            }
        }

        private void Realize(On.AbstractCreature.orig_Realize orig, AbstractCreature self)
        {
            if (self.realizedCreature == null && critobs.TryGetValue(self.creatureTemplate.TopAncestor().type, out var crit)) {
                self.realizedObject = crit.GetRealizedCreature(self) ?? throw new InvalidOperationException($"{crit.GetType()}::GetRealizedCreature returned null!");

                self.InitiateAI();

                foreach (var stuck in self.stuckObjects) {
                    if (stuck.A.realizedObject == null) {
                        stuck.A.Realize();
                    }
                    if (stuck.B.realizedObject == null) {
                        stuck.B.Realize();
                    }
                }
            }

            orig(self);
        }

        private void Ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature self, World world, CreatureTemplate template, Creature real, WorldCoordinate pos, EntityID id)
        {
            orig(self, world, template, real, pos, id);

            if (critobs.TryGetValue(template.TopAncestor().type, out var critob)) {
                // Set creature state
                self.state = critob.GetState(self);

                // Set creature AI
                AbstractCreatureAI? abstractAI = critob.GetAbstractAI(self);

                if (template.AI && abstractAI != null) {
                    self.abstractAI = abstractAI;

                    bool setDenPos = pos.abstractNode > -1 && pos.abstractNode < self.Room.nodes.Length
                        && self.Room.nodes[pos.abstractNode].type == AbstractRoomNode.Type.Den && !pos.TileDefined;

                    if (setDenPos) {
                        self.abstractAI.denPosition = pos;
                    }
                } else if (abstractAI != null) {
                    Debug.LogError($"{critob.GetType()}::GetAbstractAI returned a non-null object but template.AI was false!");
                }

                // Arbitrary setup
                critob.Init(self, world, pos, id);
            }
        }

        private bool KillsMatter(On.CreatureSymbol.orig_DoesCreatureEarnATrophy orig, CreatureType creature)
        {
            var ret = orig(creature);
            if (critobs.TryGetValue(StaticWorld.GetCreatureTemplate(creature).TopAncestor().type, out var critob)) {
                critob.KillsMatter(creature, ref ret);
            }
            return ret;
        }

        private IconSymbol.IconSymbolData CreatureSymbol_SymbolDataFromCreature(On.CreatureSymbol.orig_SymbolDataFromCreature orig, AbstractCreature creature)
        {
            if (critobs.TryGetValue(creature.creatureTemplate.type, out var critob)) {
                return new IconSymbol.IconSymbolData(creature.creatureTemplate.type, 0, critob.Icon.Data(creature));
            }
            return orig(creature);
        }

        private Color CreatureSymbol_ColorOfCreature(On.CreatureSymbol.orig_ColorOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (critobs.TryGetValue(iconData.critType, out var critob)) {
                return critob.Icon.SpriteColor(iconData.intData);
            }
            return orig(iconData);
        }

        private string CreatureSymbol_SpriteNameOfCreature(On.CreatureSymbol.orig_SpriteNameOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (critobs.TryGetValue(iconData.critType, out var critob)) {
                return critob.Icon.SpriteName(iconData.intData);
            }
            return orig(iconData);
        }
    }
}
