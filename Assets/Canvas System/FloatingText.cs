using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class FloatingText : MonoBehaviour {

	public enum ScalingMode { constantScale, scaleWithDistance };
	
	public float textScale = 1.0f;
	
	private Vector3 _initialScale;
	private ScalingMode	_scalingMode;
	private bool _alwaysOnTop;

	private Camera		_cam;
	private Canvas		_canvas;
	private Animator	_animator;
	private Text		_floatingText;
	private Image		_floatingImage;
	private FloatingTextController _textController;

	private void OnValidate()
	{
		// The gameobject this script is attached to MUST have a child gameobject.
		// The child is where the [Text], [Outline] & [Animator] components reside.
		// This is because having the [Animator] component on the parent object causes problems
		// with world transforms during animation.
		// A child will be created if it's not found - then you can customize the details!
		// BUT it's still safer to duplicate the default FloatingText prefab then modify the copy.
		
		// Ensure essential default values are set (These prevent disgusting pixely text)
		transform.localScale = _initialScale = new Vector3(0.01f, 0.01f, 0.01f);
		GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;

		if (transform.childCount < 1 || !transform.GetComponentInChildren<Animator>())
		{
			Debug.LogError("Missing child on FloatingText object! Creating default child.");
			GameObject child = CreateComponentsChild();
		}
	}

	/************************************************************************************************/
	private void Awake()
	{
		// Ensure essential default values are set (These prevent disgusting pixely text)
		transform.localScale = _initialScale = new Vector3(0.01f, 0.01f, 0.01f);
		GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;

		_canvas = GetComponent<Canvas>();
		_animator = GetComponentInChildren<Animator>();
		_floatingText = GetComponentInChildren<Text>(true);
		_floatingImage = GetComponentInChildren<Image>(true);

		_cam = Camera.main;

		if (_canvas.worldCamera == null)
			_canvas.worldCamera = _cam;
		
		_textController = FloatingTextController.instance;

		// Dynamically create an animator controller based on the AnimationClip(s) provided in FloatingTextManager
		_animator.runtimeAnimatorController = _textController.animatorOverrideController;
	}

	void OnEnable()
	{
		// Disable temporarily and update values before enabling
		_canvas.enabled = false;

		textScale = _textController.defaultTextScale;
		_scalingMode = _textController.defaultScalingMode;
		_alwaysOnTop = _textController.defaultAlwaysOnTop;
	}


	private void FixedUpdate()
	{
		UpdateRotationAndScale();

		if (!_canvas.enabled)
			_canvas.enabled = true;
	}

	/************************************************************************************************/

	public void Initialize(Transform t, string textValue, int animIndex)
	{
		transform.position = t.position;
		_floatingText.text = textValue;

		SetAlwaysOnTop(_alwaysOnTop);

		// Check duration of animation then despawn after that duration
		if (_animator.GetCurrentAnimatorClipInfo(0).Length >= 1)
			StartCoroutine(DespawnAfter(_textController.animations[animIndex].length));
		else
			StartCoroutine(DespawnAfter(1.0f)); // Fallback despawn timer if no animations in Manager

		_animator.SetInteger("TextAnimation", animIndex);
	}

	public void InitializeStatic(Transform t, string value)
	{
		transform.position = t.position;
		_floatingText.text = value;

		SetAlwaysOnTop(_alwaysOnTop);
	}

	public void SetScalingMode(ScalingMode newMode, float newScale)
	{
		_scalingMode = newMode;
		textScale = newScale;
	}

	public void SetParent(Transform t)
	{
		transform.SetParent(t);
	}

	public void SetAlwaysOnTop(bool state)
	{
		Material newMaterial;

		if (state)
			newMaterial = FloatingTextController.instance.overlayFontMaterial;
		else
			newMaterial = FloatingTextController.instance.defaultFontMaterial;
		
		_floatingText.material = newMaterial;
		_floatingImage.material = newMaterial;
	}

	/************************************************************************************************/
	private void UpdateRotationAndScale()
	{
		//transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
		transform.rotation = FloatingTextController.instance.lookRotation;
		
		if (_scalingMode == ScalingMode.constantScale)
		{
			// Adjust text scale so it's the same size despite distance from camera
			Plane plane = new Plane(_cam.transform.forward, _cam.transform.position);
			float dist = plane.GetDistanceToPoint(transform.position);  // Maybe this can use Vector3.SqrMagnitude instead (less computation?)
			transform.localScale = _initialScale * dist * (textScale / 5);
		}
		else
		{
			transform.localScale = _initialScale * (textScale);
		}
	}
	
	// Create a really basic child that has all necessary components
	private GameObject CreateComponentsChild()
	{
		GameObject components = new GameObject("Components", typeof(Animator));
		components.transform.SetParent(transform);
		
		GameObject text = new GameObject("Text", typeof(Text), typeof(Outline));
		text.transform.SetParent(components.transform);
		text.transform.localPosition = Vector3.zero;
		text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

		GameObject image = new GameObject("Image", typeof(Image));
		image.transform.SetParent(components.transform);
		image.transform.localPosition = Vector3.zero;
		image.SetActive(false);

		components.transform.localScale = new Vector3(1, 1, 1);

		return components;
	}

	IEnumerator DespawnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}
}
