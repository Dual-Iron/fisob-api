using ObjType = AbstractPhysicalObject.AbstractObjectType;
using PastebinMachine.EnumExtender;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fisobs
{
    /// <summary>
    /// Provides methods to register physical object handlers (fisobs) through the <see cref="Fisob"/> type.
    /// </summary>
    /// <remarks>Users should create one instance of this class and pass it around. After creating a new instance, <see cref="ApplyHooks"/> should be called.</remarks>
    public sealed class FisobRegistry
    {
        private readonly Dictionary<string, Fisob> fisobsByID = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new fisob registry from the provided set of <see cref="Fisob"/> instances.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the ID of a fisob is already in use.</exception>
        public FisobRegistry(IEnumerable<Fisob> fisobs)
        {
            var t = typeof(ObjType);

            foreach (Fisob fisob in fisobs) {
                if (Enum.GetNames(t).Contains(fisob.ID, StringComparer.OrdinalIgnoreCase)) {
                    throw new ArgumentException($"A name in AbstractPhysicalObject.AbstractObjectType synonymous with \"{fisob.ID}\" already exists.");
                }

                if (fisobsByID.ContainsKey(fisob.ID)) {
                    throw new ArgumentException($"A fisob with the ID \"{fisob.ID}\" already exists.");
                }

                fisobsByID[fisob.ID] = fisob;

                EnumExtender.AddDeclaration(t, fisob.ID);
            }

            EnumExtender.ExtendEnumsAgain();

            foreach (Fisob fisob in fisobs) {
                fisob.Type = (ObjType)Enum.Parse(t, fisob.ID, false);
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
            On.Player.IsObjectThrowable += Player_IsObjectThrowable;
            On.Player.Grabability += Player_Grabability;
            On.ScavengerAI.CollectScore_PhysicalObject_bool += ScavengerAI_CollectScore_PhysicalObject_bool;
            On.SaveState.AbstractPhysicalObjectFromString += SaveState_AbstractPhysicalObjectFromString;
        }

        /// <summary>
        /// Undoes the hooks applied through <see cref="ApplyHooks"/>.
        /// </summary>
        public void UndoHooks()
        {
            On.Player.IsObjectThrowable -= Player_IsObjectThrowable;
            On.Player.Grabability -= Player_Grabability;
            On.ScavengerAI.CollectScore_PhysicalObject_bool -= ScavengerAI_CollectScore_PhysicalObject_bool;
            On.SaveState.AbstractPhysicalObjectFromString -= SaveState_AbstractPhysicalObjectFromString;
        }

        private bool Player_IsObjectThrowable(On.Player.orig_IsObjectThrowable orig, Player self, PhysicalObject obj)
        {
            bool ret = orig(self, obj);

            if (TryGet(obj.abstractPhysicalObject.type, out var fisob)) {
                fisob.GetBehavior(obj).CanThrow(self, ref ret);
            }

            return ret;
        }

        private int Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            Player.ObjectGrabability ret = (Player.ObjectGrabability)orig(self, obj);

            if (TryGet(obj.abstractPhysicalObject.type, out var fisob)) {
                fisob.GetBehavior(obj).GetGrabability(self, ref ret);
            }

            return (int)ret;
        }

        private int ScavengerAI_CollectScore_PhysicalObject_bool(On.ScavengerAI.orig_CollectScore_PhysicalObject_bool orig, ScavengerAI self, PhysicalObject obj, bool weaponFiltered)
        {
            int ret = orig(self, obj, weaponFiltered);

            if (TryGet(obj.abstractPhysicalObject.type, out var fisob)) {
                if (weaponFiltered && self.NeedAWeapon)
                    fisob.GetBehavior(obj).GetScavengerWeaponScore(self.scavenger, ref ret);
                else
                    fisob.GetBehavior(obj).GetScavengerCollectScore(self.scavenger, ref ret);
            }

            return ret;
        }

        private AbstractPhysicalObject? SaveState_AbstractPhysicalObjectFromString(On.SaveState.orig_AbstractPhysicalObjectFromString orig, World world, string objString)
        {
            string[] array = objString.Split(new[] { "<oA>" }, 3, StringSplitOptions.None);

            if (fisobsByID.TryGetValue(array[1], out Fisob o)) {
                EntityID id = EntityID.FromString(array[0]);

                string[] coordParts = array[2].Split('.');

                WorldCoordinate coord;

                if (int.TryParse(coordParts[0], out int room) &&
                    int.TryParse(coordParts[1], out int x) &&
                    int.TryParse(coordParts[2], out int y) &&
                    int.TryParse(coordParts[3], out int node)) {
                    coord = new(room, x, y, node);
                } else {
                    Debug.Log($"Corrupt world coordinate on object \"{id}\", type \"{o.ID}\"");
                    return null;
                }

                string extraData = objString.Substring(array[0].Length + array[1].Length + array[2].Length);

                return o.Parse(world, new(o.Type, id, coord, extraData));
            }

            return orig(world, objString);
        }
    }
}