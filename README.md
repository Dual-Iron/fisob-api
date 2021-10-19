# fisob-api
An API that streamlines adding new types of physical objects to Rain World.

# Step-by-step
1. Have classes deriving from `PhysicalObject` (PO) and `AbstractPhysicalObject` (APO)
2. Override `AbstractPhysicalObject.ToString()` to return `this.SaveAsString("");`
3. Create a class deriving from `Fisob` for each APO
4. Create an instance of `FisobRegistry` and call `ApplyHooks` on it

After that, you're good to go. If you want your APO to have custom data, include it as the parameter in `SaveAsString` and parse it in `Fisob.Parse`.

# Example
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
