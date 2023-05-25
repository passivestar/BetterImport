![betterimport](https://github.com/passivestar/BetterImport/assets/60579014/dadd66fb-51f4-45bb-830e-f2cefa883b08)

# Better Import

⚠️ Early version, not for production use ⚠️

This package makes import to Unity easier and more efficient

## Built-in Postprocessors

Better Import comes with these postprocessors now:

- **Animations**: Automatically creates animator controllers for models that have animations
- **Materials**: Automatically generates Metallic-Smoothness and Albedo-Alpha maps for imported materials

## Built-in Hints

Better Import comes with these built-in hints:

- **GameObject** (hints in objects' names)
    - `-prefab`: Replaces the object with a prefab
    - `-inactive`: Deactivates the object
    - `-static`: Sets the object to static
    - `-lightprobe`: Replaces the object with a light probe
    - `-reflectionprobe`: Replaces the object with a reflection probe
    - `-reverbzone`: Replace the object with a reverb zone
    - `-trigger`: Replaces the object with a trigger
    - `-volume`: Replaces the object with an SRP postprocessing volume
    - `-boxcollider`: Adds a box collider to the object
    - `-nocollider`: Disables the mesh collider
    - `-norender`: Disables the mesh renderer
    - `-rigidbody`: Adds a rigidbody to the object
    - `-bakedlight`: Sets the light mode to baked
    - `-mixedlight`: Sets the light mode to mixed

- **Model** (hints in models' filenames)
    - `-collider`: Generates mesh colliders for the model

- **Animation** (hints in actions' names)
    - `-loop`: Sets the animation to loop

- **Material** (hints in materials' names)
    - `-rough`: Sets the material Smoothness to 0
    - `-smooth`: Sets the material Smoothness to 1

## Custom Hints

You can create your own hints by subclassing `GameObjectHint`, `ModelHint`, `AnimationHint` and `MaterialHint`

Example:

```csharp
using UnityEngine;
using FirstPersonController;

public class PlayerStateVolumeHint : BetterImport.GameObjectHint
{
    public override string Text => "-playerstatevolume";
    public override string ContainerName { get; } = "Player State Volumes";

    // On create is called when the object is created for the first time
    public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
    {
        var collider = derivedObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        derivedObject.AddComponent<PlayerStateVolume>();
    }

    // On update is called when the object has changed on import
    public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
    {
        var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
        var collider = derivedObject.GetComponent<BoxCollider>();
        collider.size = bounds.size;
    }
}
```

See the implementation of built-in hints for more examples!
