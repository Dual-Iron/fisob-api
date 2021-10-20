# fisob-api
A small API that streamlines adding new types of physical objects to Rain World.

Requires EnumExtender.

# Step-by-step guide
1. Have classes deriving from `PhysicalObject` (PO) and `AbstractPhysicalObject` (APO)
2. Override `AbstractPhysicalObject.ToString()` to return `this.SaveAsString("");`
3. Create a class deriving from `Fisob` for each APO
4. Create an instance of `FisobRegistry` and call `ApplyHooks` on it

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
    
    public override string ToString() => this.SaveAsString("");
    
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
There are a few ways.
- Manual
    1. Drop the [source code](https://github.com/Dual-Iron/fisob-api/archive/refs/heads/master.zip) into a new folder in your project
- Git
    1. Run `git submodule add https://github.com/Dual-Iron/fisob-api lib/fisob-api` in your working tree
- Dependency
    1. Download the compiled assembly from [here](https://github.com/Dual-Iron/fisob-api/releases/latest)
    2. Either embed that file or package it with yours

I suggest going with manual or Git integration. It's one less file for end users to download.
