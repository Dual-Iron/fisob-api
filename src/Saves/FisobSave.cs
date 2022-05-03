using Mono.Cecil;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace Fisobs.Saves
{
    readonly partial struct FisobSave
    {
        // By design, this entire struct must be mutable, so that any changes are reflected on disk immediately.

        private readonly Dictionary<string, FisobSaveSlot> slots;
        private static FisobSave current = ReadOrCreate();

        public static FisobSaveSlot CurrentSlot => current.slots.TryGetValue(CurrentSlotName, out var slot) ? slot : new(new());
        private static string CurrentSlotName => UnityEngine.Object.FindObjectOfType<RainWorld>().options.SaveFileName;

        public FisobSave(Dictionary<string, FisobSaveSlot> slots)
        {
            this.slots = slots;
        }

        public static void Unlock(string token)
        {
            Dictionary<string, FisobSaveSlot> slots = new(current.slots);
            List<string> unlocks = new() { token };

            if (slots.TryGetValue(CurrentSlotName, out FisobSaveSlot slot)) {
                unlocks.AddRange(slot.Unlocked);
            }

            slots[CurrentSlotName] = new(unlocks);

            current = new(slots);
            current.WriteOrLogError();
        }
    }
}
