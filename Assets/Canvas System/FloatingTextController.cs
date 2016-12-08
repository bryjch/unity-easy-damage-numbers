using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {

	public static FloatingTextController instance = null;
	
	[Header("Text Settings")]
	public float defaultTextScale = 1.0f;
	public FloatingText.ScalingMode defaultScalingMode = FloatingText.ScalingMode.constantScale;    // This won't update if you change it in the inspector after runtime
	public bool defaultAlwaysOnTop = false;

	[Header("do this")]
	public Material defaultFontMaterial;
	public Material overlayFontMaterial;

	[Header("Instantiable Prefabs & Animations")]
	[Tooltip("These prefabs can be instantiated based on their index by calling: CreateFloatingText(...)")]
	public GameObject[] floatingTextPrefabs;
	[Tooltip("Animations are also selected when calling: CreateFloatingText(...)")]
	public AnimationClip[] animations;

	[Header("Update this after adding/removing animations")]
	public AnimatorOverrideController animatorOverrideController;
	
	[HideInInspector]
	public Quaternion lookRotation;

	/************************************************************************************************/
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
		
		if (defaultFontMaterial == null)
			defaultFontMaterial = (Material)Resources.Load("Materials/DefaultFontMaterial");

		if (overlayFontMaterial == null)
			overlayFontMaterial = (Material)Resources.Load("Materials/OverlayFontMaterial");
	}

	private void FixedUpdate()
	{
		lookRotation = Quaternion.LookRotation(Camera.main.transform.forward);
	}

	/************************************************************************************************/
	public FloatingText CreateFloatingText(string textValue, Transform t, int prefabIndex, int animIndex)
	{
		// Check parameter validity
		if (prefabIndex >= floatingTextPrefabs.Length) prefabIndex = 0;
		if (animIndex >= animations.Length) animIndex = 0;

		// Spawn new instance from pool. By default, it will be a parent of the controller gameobject.
		FloatingText instance = SimplePool.Spawn(floatingTextPrefabs[prefabIndex], Vector3.zero, Quaternion.identity).GetComponent<FloatingText>();
		instance.transform.SetParent(transform);
		
		instance.Initialize(t, textValue, animIndex);

		return instance;
	}

	// Method if you want to create a text object that shouldn't animate and despawn
	public FloatingText CreateFloatingTextStatic(string textValue, Transform t, int prefabIndex)
	{
		// Check parameter validity
		if (prefabIndex < floatingTextPrefabs.Length) prefabIndex = 0;

		// Spawn new instance from pool. By default, it will be a parent of the controller gameobject.
		FloatingText instance = SimplePool.Spawn(floatingTextPrefabs[prefabIndex], Vector3.zero, Quaternion.identity).GetComponent<FloatingText>();
		instance.transform.SetParent(transform); 

		instance.InitializeStatic(t, textValue);
		
		return instance;
	}
}
