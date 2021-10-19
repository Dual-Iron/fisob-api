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

<details>
    <summary>Example</summary>
    
```cs
class CustomFisob : Fisob {
    public static readonly CustomFisob Instance = new CustomFisob();
    
    private CustomFisob : base("custom_fisob") { }

    public override AbstractPhysicalObject? Parse(World world, EntitySaveData saveData) {
        return new CustomAPO(world, saveData.Pos, saveData.ID);
    }
}

class CustomAPO : AbstractPhysicalObject {
    public CustomAPO(World world, WorldCoordinate pos, EntityID ID) : base(world, CustomFisob.Instance.Type, null, pos, ID) { }
    
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

I suggest going with manual or Git integration. You'll have to compile the source yourself, but it's one less file to download for end users.

<details>
    <summary>I can't compile the source code</summary>
    
It's probably because your C# version is outdated. 
    
To fix that, double-click your project in Visual Studio and [set the `LangVersion` property to `latest`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version#edit-the-project-file).

<details>
    <summary>I don't see LangVersion / I can't double-click my project in Visual Studio</summary>

Then your project configuration is _also_ outdated. To fix that:
1. Close Visual Studio
2. Replace your .csproj file's contents with [this](https://gist.githubusercontent.com/Dual-Iron/f4cdf5bd8f9f5d5222d76e7c6e5e37d4/raw/267dfe7f9b0d01ac233c4e3f1717044ce803b632/SampleProject.csproj) using a simple text editor
3. Open the .csproj file with Visual Studio again

Make sure to change the `References` property in the new .csproj to the path containing all your reference assemblies.
</details>
</details>
