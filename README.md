# fisob-api
An API that streamlines adding new types of physical objects to Rain World.

# Step-by-step
1. Have classes deriving from `PhysicalObject` (PO) and `AbstractPhysicalObject` (APO)
2. Override `AbstractPhysicalObject.ToString()` to return `this.SaveAsString("");`
3. Create a class deriving from `Fisob` for each APO
4. Create an instance of `FisobRegistry` and call `ApplyHooks` on it

If you want your APO to have custom data, include it as the parameter in `SaveAsString` and parse it in `Fisob.Parse`.

You're good to go!

<details>
    <summary>Example</summary>
    
```cs
class CustomFisob : Fisob {
    public CustomFisob : base("custom_fisob") { }

    public override AbstractPhysicalObject? Parse(World world, EntitySaveData saveData) {
        return new CustomAPO(world, saveData.Pos, saveData.ID);
    }
}

class CustomAPO : AbstractPhysicalObject {
    public CustomAPO(World world, WorldCoordinate pos, EntityID ID) : base(world, MyMod.Fisobs["custom_fisob"].Type, null, pos, ID) { }
    
    public override string ToString() => this.SaveAsString("");
    
    public override void Realize() {
        base.Realize();
        realizedObject ??= new CustomPO(...);
    }
}

class CustomPO : PhysicalObject {
    // etc...
    // To spawn a CustomPO in the world, use `new CustomAPO(world, pos, world.game.GetNewID()).Spawn()`.
}

class MyMod {
    public static readonly FisobRegistry Fisobs = GetRegistry();
    
    static FisobRegistry GetRegistry() {
        var ret = new FisobRegistry(new[] { new CustomFisob() });
    }
}
```
</details>

# How to include in your project
There are a few ways.
- Manual
    1. Drop the source code into a new folder in your project
    2. To update, delete the folder and repeat the previous step
- Git
    1. Run `git submodule add https://github.com/Dual-Iron/fisob-api lib/fisob-api` in your working tree
    2. To update, run `git submodule update --remote lib/fisob-api` in your working tree
- Dependency
    1. Download the compiled assembly from [here](https://github.com/Dual-Iron/fisob-api/releases/latest)
    2. Either embed that file or package it with yours
    3. To update, repeat the previous steps
