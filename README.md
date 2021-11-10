# fisob-api
A small API that streamlines adding new types of physical objects to Rain World.

Requires EnumExtender.

# Step-by-step guide
To create a physical object:
1. Have a class deriving from `PhysicalObject` and another from `AbstractPhysicalObject`
2. In the `AbstractPhysicalObject` class, override `ToString()` and return `this.SaveAsString("");`
3. Create a class deriving from `Fisob`
4. Create an instance of `FisobRegistry` and call `ApplyHooks` when your mod loads

If you want your APO to have custom data, include it as the parameter in `SaveAsString` and parse it in `Fisob.Parse`.

You're good to go!

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
First, reference EnumExtender in your project.

Then, add the source to your project. There are a few ways.
- Manual: Drop the [source code](https://github.com/Dual-Iron/fisob-api/archive/refs/heads/master.zip) into a new folder in your project.
- Git clone: Clone or fork this repository into a new folder in your project.

Remember to rename the Fisobs namespace. You're good to go!
