using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.Animations;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class FloatingText : MonoBehaviour {

	public enum ScalingMode { constantScale, scaleWithDistance };
	
	public float textScale = 1.0f;
	
	private Vector3 _initialScale;
	private ScalingMode	_scalingMode;

	private Camera		_cam;
	private Animator	_animator;
	private Text		_floatingText;
	private FloatingTextController _textController;

	private void OnValidate()
	{
		// The gameobject this script is attached to MUST have a child gameobject.
		// The child is where the [Text], [Outline] & [Animator] components reside.
		// This is because having the [Animator] component on the parent object causes problems
		// with world transforms during animation.
		// A child will be created if it's not found - then you can customize the details!
		// BUT it's still safer to duplicate the default FloatingText prefab then modify the copy.
		if (transform.childCount < 1 || transform.GetComponentInChildren<Text>() == null)
		{
			Debug.LogError("Missing child on FloatingText object! Creating default child.");
			GameObject child = CreateDefaultChild();
		}
	}

	/************************************************************************************************/
	private void Awake()
	{
		// Ensure essential default values are set (These prevent disgusting pixely text)
		transform.localScale = _initialScale = new Vector3(0.01f, 0.01f, 0.01f);
		GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 10;
		
		_cam = Camera.main;

		if (GetComponent<Canvas>().worldCamera == null)
			GetComponent<Canvas>().worldCamera = _cam;

		_animator = GetComponentInChildren<Animator>();
		_floatingText = _animator.GetComponent<Text>();
		
		_textController = FloatingTextController.instance;

		// Dynamically create an animator controller based on the AnimationClip(s) provided in FloatingTextManager
		_animator.runtimeAnimatorController = _textController.GetAnimatorController();
	}

	void OnEnable()
	{
		textScale = _textController.defaultTextScale;
		_scalingMode = _textController.defaultScalingMode;

		// Billboard facing functionality
		transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
	}


	private void FixedUpdate()
	{
		// Billboard facing functionality
		transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
		
		if (_scalingMode == ScalingMode.constantScale) {
			// Adjust text scale so it's the same size despite distance from camera
			Plane plane = new Plane(_cam.transform.forward, _cam.transform.position);
			float dist = plane.GetDistanceToPoint(transform.position);	// Maybe this can use Vector3.SqrMagnitude instead (less computation?)
			transform.localScale = _initialScale * dist * (textScale / 5);
		}
		else
		{
			transform.localScale = _initialScale * (textScale);
		}
	}

	/************************************************************************************************/

	public void Initialize(Transform t, string textValue, int animIndex)
	{
		transform.position = t.position;

		_floatingText.text = textValue;

		if (_animator.GetCurrentAnimatorClipInfo(0).Length >= 1)
			StartCoroutine(DespawnAfter(_textController.animations[animIndex].length));
		else
			StartCoroutine(DespawnAfter(1.0f)); // Fallback despawn timer if no animations in Manager

		_animator.SetInteger("TextAnimation", animIndex);
	}

	public void InitializeStatic(Transform t, string value)
	{
		transform.SetParent(t);
		transform.position = t.position;

		_floatingText.text = value;
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

	/************************************************************************************************/
	
	// Create a really basic child that has all necessary components
	private GameObject CreateDefaultChild()
	{
		GameObject child = new GameObject();
		child.name = "Text";
		child.transform.SetParent(transform);
		child.AddComponent<RectTransform>();
		child.AddComponent<Text>().alignment = TextAnchor.MiddleCenter;
		child.AddComponent<Outline>();
		child.AddComponent<Animator>();

		return child;
	}

	IEnumerator DespawnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}
}
