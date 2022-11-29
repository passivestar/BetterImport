![betterimport](https://user-images.githubusercontent.com/60579014/204586011-c40e7390-216b-4bbd-8851-258bea93a7d4.jpg)

# Better Import

⚠️ Early version, not for production use ⚠️

This package makes import to Unity easier and more efficient

## Built-in Postprocessors

Better Import comes with these postprocessors now:

- **Animations**: Automatically creates animator controllers for models that have animations
- **Materials**: Automatically generates Metallic-Smoothness maps for imported models
- **Lights**: Can modify properties of imported lights

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
