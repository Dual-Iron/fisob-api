using static Menu.Menu;
using static Menu.SandboxEditorSelector;
using ObjType = AbstractPhysicalObject.AbstractObjectType;
using PastebinMachine.EnumExtender;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using ArenaBehaviors;

namespace Fisobs
{
    /// <summary>
    /// Provides methods to register physical object handlers (fisobs) through the <see cref="Fisob"/> type.
    /// </summary>
    /// <remarks>Users should create one instance of this class and pass it around. After creating a new instance, <see cref="ApplyHooks"/> should be called.</remarks>
    public sealed class FisobRegistry
    {
        private readonly Dictionary<string, Fisob> fisobsByID = new Dictionary<string, Fisob>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new fisob registry from the provided set of <see cref="Fisob"/> instances.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when a fisob can't be added to this registry.</exception>
        public FisobRegistry(IEnumerable<Fisob> fisobs)
        {
            var t = typeof(ObjType);
            var names = Enum.GetNames(t);

            // Verify fisobs first
            foreach (Fisob fisob in fisobs) {
                if (fisob.IsInRegistry) {
                    throw new InvalidOperationException($"The fisob \"{fisob.ID}\" is already in a registry.");
                }

                if (fisobsByID.ContainsKey(fisob.ID)) {
                    throw new ArgumentException($"A fisob with the ID \"{fisob.ID}\" is already in this registry.");
                }

                if (names.Contains(fisob.ID, StringComparer.OrdinalIgnoreCase)) {
                    throw new ArgumentException($"The ID \"{fisob.ID}\" is synonymous with a name in AbstractPhysicalObject.AbstractObjectType.");
                }
            }

            // Add them to enums
            foreach (Fisob fisob in fisobs) {
                fisobsByID[fisob.ID] = fisob;

                EnumExtender.AddDeclaration(t, fisob.ID);
            }

            EnumExtender.ExtendEnumsAgain();

            // Assign their types
            foreach (Fisob fisob in fisobs) {
                fisob.Type = (ObjType)Enum.Parse(t, fisob.ID, true);
            }
        }

        /// <summary>
        /// Gets a fisob from its ID.
        /// </summary>
        /// <returns>The fisob whose ID is <paramref name="id"/>.</returns>
        /// <exception cref="KeyNotFoundException"/>
        public Fisob this[string id] => fisobsByID[id];

        /// <summary>
        /// Gets a fisob from an object type. This is sugar for <see cref="this[string]"/>.
        /// </summary>
        /// <returns>The fisob whose type is <paramref name="type"/>.</returns>
        /// <exception cref="KeyNotFoundException"/>
        public Fisob this[ObjType type] => fisobsByID[type.ToString()];

        /// <summary>
        /// Gets a fisob from its ID.
        /// </summary>
        /// <param name="id">The ID of the fisob.</param>
        /// <param name="fisob">If it exists, the fisob; otherwise, <see langword="null"/>.</param>
        /// <returns>If the fisob exists, <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGet(string id, out Fisob fisob)
        {
            return fisobsByID.TryGetValue(id, out fisob);
        }

        /// <summary>
        /// Gets a fisob from an object type.
        /// </summary>
        /// <param name="type">The type of the fisob.</param>
        /// <param name="fisob">If it exists, the fisob; otherwise, <see langword="null"/>.</param>
        /// <returns>If the fisob exists, <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGet(ObjType type, out Fisob fisob)
        {
            return fisobsByID.TryGetValue(type.ToString(), out fisob);
        }

        /// <summary>
        /// Applies hooks that enable fisob behavior.
        /// </summary>
        public void ApplyHooks()
        {
            On.Menu.SandboxEditorSelector.ctor += SandboxEditorSelector_ctor;
            On.RainWorld.LoadResources += RainWorld_LoadResources;
            On.ItemSymbol.ColorForItem += ItemSymbol_ColorForItem;
            On.ItemSymbol.SpriteNameForItem += ItemSymbol_SpriteNameForItem;
            On.SandboxGameSession.SpawnEntity += SandboxGameSession_SpawnEntity;
            On.Menu.SandboxEditorSelector.AddButton_Button_refInt32 += SandboxEditorSelector_AddButton_Button_refInt32;

            On.Player.IsObjectThrowable += Player_IsObjectThrowable;
            On.Player.Grabability += Player_Grabability;
            On.ScavengerAI.RealWeapon += ScavengerAI_RealWeapon;
            On.ScavengerAI.WeaponScore += ScavengerAI_WeaponScore;
            On.ScavengerAI.CollectScore_PhysicalObject_bool += ScavengerAI_CollectScore_PhysicalObject_bool;
            On.SaveState.AbstractPhysicalObjectFromString += SaveState_AbstractPhysicalObjectFromString;
        }

        /// <summary>
        /// Undoes the hooks applied through <see cref="ApplyHooks"/>.
        /// </summary>
        public void UndoHooks()
        {
            On.Menu.SandboxEditorSelector.ctor -= SandboxEditorSelector_ctor;
            On.RainWorld.LoadResources -= RainWorld_LoadResources;
            On.ItemSymbol.ColorForItem -= ItemSymbol_ColorForItem;
            On.ItemSymbol.SpriteNameForItem -= ItemSymbol_SpriteNameForItem;
            On.SandboxGameSession.SpawnEntity -= SandboxGameSession_SpawnEntity;
            On.Menu.SandboxEditorSelector.AddButton_Button_refInt32 -= SandboxEditorSelector_AddButton_Button_refInt32;

            On.Player.IsObjectThrowable -= Player_IsObjectThrowable;
            On.Player.Grabability -= Player_Grabability;
            On.ScavengerAI.RealWeapon -= ScavengerAI_RealWeapon;
            On.ScavengerAI.WeaponScore -= ScavengerAI_WeaponScore;
            On.ScavengerAI.CollectScore_PhysicalObject_bool -= ScavengerAI_CollectScore_PhysicalObject_bool;
            On.SaveState.AbstractPhysicalObjectFromString -= SaveState_AbstractPhysicalObjectFromString;
        }

        private FisobProperties P(PhysicalObject po)
        {
            if (po is null) return null;
            if (!fisobsByID.TryGetValue(po.abstractPhysicalObject.type.ToString(), out var f)) return null;
            return f.GetProperties(po);
        }

        private void SandboxEditorSelector_ctor(On.Menu.SandboxEditorSelector.orig_ctor orig, Menu.SandboxEditorSelector self, Menu.Menu menu, Menu.MenuObject owner, SandboxOverlayOwner overlayOwner)
        {
            Width = 19;
            Height = 4;
            orig(self, menu, owner, overlayOwner);
        }

        private void RainWorld_LoadResources(On.RainWorld.orig_LoadResources orig, RainWorld self)
        {
            orig(self);

            foreach (var fisob in fisobsByID.Values) {
                fisob.LoadResources(self);
            }
        }

        private Color ItemSymbol_ColorForItem(On.ItemSymbol.orig_ColorForItem orig, ObjType itemType, int intData)
        {
            if (fisobsByID.TryGetValue(itemType.ToString(), out var fisob)) {
                return fisob.IconColor;
            }
            return orig(itemType, intData);
        }

        private string ItemSymbol_SpriteNameForItem(On.ItemSymbol.orig_SpriteNameForItem orig, ObjType itemType, int intData)
        {
            if (fisobsByID.TryGetValue(itemType.ToString(), out var fisob)) {
                return fisob.IconName;
            }
            return orig(itemType, intData);
        }

        private void SandboxGameSession_SpawnEntity(On.SandboxGameSession.orig_SpawnEntity orig, SandboxGameSession self, SandboxEditor.PlacedIconData p)
        {
            if (fisobsByID.TryGetValue(p.data.itemType.ToString(), out var fisob)) {
                WorldCoordinate coord = new WorldCoordinate(0, Mathf.RoundToInt(p.pos.x / 20f), Mathf.RoundToInt(p.pos.y / 20f), -1);
                EntitySaveData data = new EntitySaveData(p.data.itemType, p.ID, coord, "");

                try {
                    var entity = fisob.Parse(self.game.world, data);
                    if (entity != null) {
                        self.game.world.GetAbstractRoom(0).AddEntity(entity);
                    } else {
                        Debug.LogError($"The fisob {fisob.ID} returned null when being parsed in sandbox mode.");
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    Debug.LogError($"The fisob {fisob.ID} threw an exception when being parsed in sandbox mode.");
                }
            } else {
                orig(self, p);
            }
        }

        private void SandboxEditorSelector_AddButton_Button_refInt32(On.Menu.SandboxEditorSelector.orig_AddButton_Button_refInt32 orig, Menu.SandboxEditorSelector self, Button button, ref int counter)
        {
            if (counter == MultiplayerUnlocks.ItemsUnlocks + 3 - 1) {
                orig(self, button, ref counter);

                foreach (var fisob in fisobsByID.Values) {
                    var sandboxState = fisob.GetSandboxState(self.unlocks);

                    if (sandboxState == SandboxState.Hidden) continue;

                    if (counter >= Width * Height - 51) {
                        self.bkgRect.size.y += ButtonSize;
                        self.size.y += ButtonSize;
                        self.pos.y += ButtonSize;
                        Height += 1;

                        Button[,] newArr = new Button[Width, Height];

                        for (int i = 0; i < Width; i++) {
                            for (int j = 0; j < Height - 1; j++) {
                                newArr[i, j + 1] = self.buttons[i, j];
                            }
                        }

                        self.buttons = newArr;
                    }

                    if (sandboxState == SandboxState.Unlocked)
                        orig(self, new CreatureOrItemButton(self.menu, self, new IconSymbol.IconSymbolData(0, fisob.Type, 0)), ref counter);
                    else
                        orig(self, new LockedButton(self.menu, self), ref counter);
                }
            } else {
                orig(self, button, ref counter);
            }
        }

        private bool Player_IsObjectThrowable(On.Player.orig_IsObjectThrowable orig, Player self, PhysicalObject obj)
        {
            bool ret = orig(self, obj);

            P(obj)?.CanThrow(self, ref ret);

            return ret;
        }

        private int Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            Player.ObjectGrabability ret = (Player.ObjectGrabability)orig(self, obj);

            P(obj)?.GetGrabability(self, ref ret);

            return (int)ret;
        }

        private bool ScavengerAI_RealWeapon(On.ScavengerAI.orig_RealWeapon orig, ScavengerAI self, PhysicalObject obj)
        {
            bool ret = orig(self, obj);

            P(obj)?.IsLethalWeapon(self.scavenger, ref ret);

            return ret;
        }

        private int ScavengerAI_WeaponScore(On.ScavengerAI.orig_WeaponScore orig, ScavengerAI self, PhysicalObject obj, bool pickupDropInsteadOfWeaponSelection)
        {
            int ret = orig(self, obj, pickupDropInsteadOfWeaponSelection);

            if (pickupDropInsteadOfWeaponSelection)
                P(obj)?.GetScavWeaponPickupScore(self.scavenger, ref ret);
            else
                P(obj)?.GetScavWeaponUseScore(self.scavenger, ref ret);

            return ret;
        }

        private int ScavengerAI_CollectScore_PhysicalObject_bool(On.ScavengerAI.orig_CollectScore_PhysicalObject_bool orig, ScavengerAI self, PhysicalObject obj, bool weaponFiltered)
        {
            if (weaponFiltered) return orig(self, obj, true);

            int ret = orig(self, obj, weaponFiltered);

            P(obj)?.GetScavCollectibleScore(self.scavenger, ref ret);

            return ret;
        }

        private AbstractPhysicalObject SaveState_AbstractPhysicalObjectFromString(On.SaveState.orig_AbstractPhysicalObjectFromString orig, World world, string objString)
        {
            string[] array = objString.Split(new[] { "<oA>" }, StringSplitOptions.None);

            if (fisobsByID.TryGetValue(array[1], out Fisob o) && array.Length > 2) {
                EntityID id = EntityID.FromString(array[0]);

                string[] coordParts = array[2].Split('.');

                WorldCoordinate coord;

                if (int.TryParse(coordParts[0], out int room) &&
                    int.TryParse(coordParts[1], out int x) &&
                    int.TryParse(coordParts[2], out int y) &&
                    int.TryParse(coordParts[3], out int node)) {
                    coord = new WorldCoordinate(room, x, y, node);
                } else {
                    Debug.Log($"{nameof(Fisobs)} : Corrupt world coordinate on object \"{id}\", type \"{o.ID}\"");
                    return null;
                }

                string customData = array.Length == 4 ? array[3] : "";

                if (array.Length > 4) {
                    Debug.LogError($"{nameof(Fisobs)} : Save data had more than 4 <oA> sections in \"{o.ID}\". Override `APO.ToString()` to return `this.SaveAsString(...)`.");
                    return null;
                }

                try {
                    return o.Parse(world, new EntitySaveData(o.Type, id, coord, customData));
                } catch (Exception e) {
                    Debug.LogError($"{nameof(Fisobs)} : {e}");
                    return null;
                }
            }

            return orig(world, objString);
        }
    }
}