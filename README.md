# fisob-api
A small API that streamlines adding new types of physical objects to Rain World.

Requires EnumExtender.

# Setup
To define a new physical object, Rain World expects a `PhysicalObject` and `AbstractPhysicalObject` class:
1. Make a class derived from `PhysicalObject`. This represents a loaded, or "realized", object. Define the object's behavior here.
1. Make a class derived from `AbstractPhysicalObject` that overrides `ToString()` to return `this.SaveAsString()`. This represents an unloaded, or "abstract", object. Define the object's saved data here.
1. Make a class derived from `Fisob`. This represents the object's properties, like how valuable the item is to scavengers.
1. When your mod loads, construct a `FisobRegistry` and call `ApplyHooks` on it.

You're good to go! For specific help, see the examples below.

# Examples

The entire [CentiShields](https://github.com/Dual-Iron/centipede-shields) mod is a glorified example of fisob-api. Steal some code from there.

<details>
    <summary>Click here for a more concise example.</summary>
    
```cs
class CustomFisob : Fisob {
    public static readonly CustomFisob Instance = new CustomFisob();
    
    private CustomFisob() : base("custom_fisob") { }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData) {
        return new CustomAPO(world, saveData.Pos, saveData.ID);
    }
}

class CustomAPO : AbstractPhysicalObject {
    public CustomAPO(World world, WorldCoordinate pos, EntityID ID) : base(world, CustomFisob.Instance.Type, null, pos, ID) { }
    
    public override string ToString() => this.SaveToString("");
    
    public override void Realize() {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new CustomPO(...);
    }
}

class CustomPO : PhysicalObject {
    // etc...
    // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.
}

class MyMod {
    void OnEnable() {
        new FisobRegistry(new[] { CustomFisob.Instance }).ApplyHooks();
    }
}
```
</details>

# How to integrate into your project
1. Reference EnumExtender in your project.
2. Drop [the fisobs source code](https://github.com/Dual-Iron/fisob-api/archive/refs/heads/master.zip) into a new folder in your project.

Remember to rename the Fisobs namespace. You're good to go!
