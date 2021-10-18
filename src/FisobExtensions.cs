using UnityEngine;

namespace Fisobs;

/// <summary>
/// Provides extension methods for POs and APOs.
/// </summary>
public static class FisobExtensions
{
    /// <summary>
    /// Realizes an APO at a position with a specified velocity.
    /// </summary>
    /// <param name="obj">The abstract physical object.</param>
    /// <param name="pos">The position of the object in the room.</param>
    /// <param name="vel">The velocity of the object's body chunks.</param>
    public static void Spawn(this AbstractPhysicalObject obj, Vector2 pos, Vector2 vel)
    {
        if (obj.realizedObject != null) {
            Debug.Log("TRYING TO REALIZE TWICE! " + obj);
            return;
        }

        obj.Room.AddEntity(obj);
        obj.RealizeInRoom();

        if (obj.realizedObject is PhysicalObject o) {
            foreach (var chunk in o.bodyChunks) {
                chunk.HardSetPosition(pos);
                chunk.vel = vel;
            }
        }
    }

    /// <summary>
    /// Realizes an APO at a position with a speed of zero.
    /// </summary>
    /// <param name="obj">The abstract physical object.</param>
    /// <param name="pos">The position of the object in the room.</param>
    public static void Spawn(this AbstractPhysicalObject obj, Vector2 pos)
    {
        Spawn(obj, pos, Vector2.zero);
    }

    /// <summary>
    /// Realizes an APO wherever it may be with a speed of zero.
    /// </summary>
    /// <param name="obj">The abstract physical object.</param>
    public static void Spawn(this AbstractPhysicalObject obj)
    {
        if (obj.Room.realizedRoom != null)
            Spawn(obj, obj.Room.realizedRoom.MiddleOfTile(obj.pos.Tile), Vector2.zero);
        else
            Debug.Log("TRYING TO REALIZE IN NON REALIZED ROOM! " + obj);
    }
}
