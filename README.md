# Better Import

This package has multiple features for making import to Unity easier and more efficient

## Built-in Postprocessors

These postprocessors are enabled by default:

- Animations: Animations postprocessor automatically creates animator controllers for models that have animations
- Materials: Materials postprocessor automatically generates Metallic-Smoothness maps for imported models
- Lights: Light postprocessor can modify properties of imported lights

## Built-in Hints

Better Import comes with these built-in hints:

- GameObject (hints in objects' names)
    - `-prefab`: Replaces the object with a prefab. Creates a new prefab automatically if it doesn't exist
    - `-inactive`: Deactivates the object
    - `-static`: Sets the object to static
    - `-lightprobe`: Replaces the object with a light probe
    - `-reflectionprobe`: Replaces the object with a reflection probe
    - `-reverbzone`: Replace the object with a reverb zone
    - `-trigger`: Replaces the object with a trigger
    - `-volume`: Replaces the object with an SRP postprocessing volume
    - `-boxcollider`: Adds a box collider to the object
- Model (hints in model filenames)
    - `-collider`: Generates a mesh collider for the model
- Animation (hints in actions' names)
    - `-loop`: Sets the animation to loop
- Material (hints in materials' names)
    - `-rough`: Sets the material Smoothness to 0
    - `-smooth`: Sets the material Smoothness to 1

## Custom Hints

You can create your own hints by subclassing `GameObjectHint`, `ModelHint`, `AnimationHint` and `MaterialHint`

For example:

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