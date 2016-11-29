# Unity Floating Text System

### How to use
- Add FloatingTextManager prefab to scene.
- Generate floating text by calling: `FloatingTextManager.instance.CreateFloatingText(...)`

#### Example usage
```C#
if (Input.GetKeyDown(KeyCode.F))
{
    FloatingText instance = FloatingTextManager.instance.CreateFloatingText(transform, "Hello world!", 0, 0);
    instance.SetParent(transform);
    instance.SetScalingMode(ScalingMode.scaleWithDistance, 2.0f);
}
```
This will spawn a FloatingText at the given `transform`. You can also customize some of the details using certain `FloatingText` methods (see below for more details).

---
### Custom FloatingTexts
I suggest duplicating and renaming the existing FloatingText_Default prefab, then tweaking the relevant components in its child object.

After you're satisfied, add your new prefab to the FloatingTextManager's `floatingTextPrefabs` array in the inspector.

### Custom Animations
You can also create new animations for the prefab using the Animation window. Make sure the prefab is in scene and you have its child with the Animator componenent selected.

After you're satisfied, add your new animation to the FloatingTextManager's `animations` array in the inspector.

---
### FloatingTextManager Methods
Call these from anywhere if you want to spawn a new FloatingText.
###### `FloatingText CreateFloatingText(Transform t, string textValue, int prefabIndex, int animIndex)`
- `Transform t` - Prefab will be spawned at this position.
- `string textValue` - Text that will appear.
- `int prefabIndex` - Prefab that will be spawned based on floatingTextPrefabs[].
- `int animIndex` - Animation that will play based on animations[].

---
### FloatingText Methods
Call these methods on the spawned instance returned by `CreateFloatingText(...)`.
###### `void SetScalingMode(ScalingMode newMode, float newScale)`
- `ScalingMode newMode` - Can either be: `constantScale` or `scaleWithDistance`.
- `float newScale` - Pretty self explanatory!

_(By default, these values will be based on settings in FloatingTextManager)_

###### `void SetParent(Transform t)`
- `Transform t` - Transform that the floating text will become a child of. 

_(By default, it will be a child of the FloatingTextManager)_

---
### Credits
- __Martin Glaude__ ([@quill18](https://github.com/quill18)) - [SimplePool.cs](https://gist.github.com/quill18/5a7cfffae68892621267) 
Utilizes object pooling instead of instantiating for better optimization.
