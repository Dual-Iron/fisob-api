using ObjType = AbstractPhysicalObject.AbstractObjectType;
using PastebinMachine.EnumExtender;
using UnityEngine;

namespace Fisobs;

/// <summary>
/// Provides methods to register physical object handlers (fisobs) through the <see cref="Fisob"/> type.
/// </summary>
public sealed class FisobRegistry
{
    private readonly Dictionary<string, Fisob> fisobsByID = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new fisob registry from the provided set of <see cref="Fisob"/> instances.
    /// </summary>
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
    /// Applies hooks that enable fisob behavior.
    /// </summary>
    public void ApplyHooks()
    {
        // TODO add hooks for FisobBehavior
        On.SaveState.AbstractPhysicalObjectFromString += SaveState_AbstractPhysicalObjectFromString;
    }

    /// <summary>
    /// Undoes the hooks applied through <see cref="ApplyHooks"/>.
    /// </summary>
    public void UndoHooks()
    {
        On.SaveState.AbstractPhysicalObjectFromString -= SaveState_AbstractPhysicalObjectFromString;
    }

    /// <summary>
    /// Gets a fisob from its ID.
    /// </summary>
    /// <returns>The fisob whose ID is <paramref name="id"/>.</returns>
    public Fisob this[string id] => fisobsByID[id];

    /// <summary>
    /// Gets a fisob from an object type. This is sugar for <see cref="this[string]"/>.
    /// </summary>
    /// <returns>The fisob whose type is <paramref name="type"/>.</returns>
    public Fisob this[ObjType type] => fisobsByID[type.ToString()];

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
