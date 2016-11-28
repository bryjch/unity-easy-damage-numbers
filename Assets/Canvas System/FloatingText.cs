using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.Animations;

public class FloatingText : MonoBehaviour {

	public enum ScalingMode { constantScale, scaleWithDistance };
	
	public float objectScale = 1.0f;

	private ScalingMode scalingMode = ScalingMode.constantScale;

	private Text floatingText;

	private AnimatorClipInfo[] clipInfo;

	private Vector3 initialScale;

	
	private bool fixedScale = true;

	private Animator animator;
	private Camera cam;
	private FloatingTextController textController;


	/************************************************************************************************/
	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		floatingText = animator.GetComponent<Text>();

		clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		GetComponent<Canvas>().worldCamera = Camera.main;

		initialScale = transform.localScale;
		cam = Camera.main;

		if (textController == null)
			textController = FloatingTextController.instance;

		UpdateAnimatorController();
	}

	void OnEnable()
	{
		// Billboard facing functionality
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
	}


	private void FixedUpdate()
	{
		// Billboard facing functionality
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
		
		if (scalingMode == ScalingMode.constantScale) {
			// Adjust text scale so it's the same size despite distance from camera
			Plane plane = new Plane(cam.transform.forward, cam.transform.position);
			float dist = plane.GetDistanceToPoint(transform.position);	// Maybe this can use Vector3.SqrMagnitude instead (less computation?)
			transform.localScale = initialScale * dist * (objectScale / 5);
		}
	}

	/************************************************************************************************/

	public void Initialize(Transform t, string value, int animIndex, float fontScale = 1.0f)
	{
		transform.SetParent(t);
		transform.position = t.position;

		floatingText.text = value;

		if (clipInfo.Length >= 1)
			StartCoroutine(DespawnAfter(textController.animations[animIndex].length));
		else
			StartCoroutine(DespawnAfter(1.0f)); // Fallback despawn timer if no animations in Manager

		objectScale = fontScale;

		GetComponentInChildren<Animator>().SetInteger("TextAnimation", animIndex);
	}

	public void InitializeStatic(Transform t, string value)
	{
		transform.SetParent(t);
		transform.position = t.position;

		floatingText.text = value;
	}

	public void SetScalingMode(ScalingMode newMode)
	{
		scalingMode = newMode;
	}

	/************************************************************************************************/


	private void UpdateAnimatorController()
	{
		// The new controller that will be created based on Manager animations
		AnimatorController newController = new AnimatorController();
		newController.AddLayer("DefaultLayer");

		// Add a parameter that will determine the animation states
		AnimatorControllerParameter animatorParameter = new AnimatorControllerParameter();
		animatorParameter.type = AnimatorControllerParameterType.Int;
		animatorParameter.name = "TextAnimation";
		animatorParameter.defaultInt = 999;
		newController.AddParameter(animatorParameter);

		// Add state machine
		AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
		AnimatorStateMachine stateMachine = rootStateMachine.AddStateMachine("TextAnimationStateMachine");

		// Create a default state to prevent animation auto playing index 0
		AnimatorState waitingState = stateMachine.AddState("Waiting");

		//foreach (AnimationClip clip in DamageNumberManager.instance.animations)
		for (int i = 0; i < FloatingTextController.instance.animations.Length; i++)
		{
			AnimationClip clip = FloatingTextController.instance.animations[i];

			// Add new state based on the AnimationClip
			AnimatorState state = stateMachine.AddState(clip.name);
			state.motion = clip;

			// Create transition from "Waiting" to the new state
			AnimatorStateTransition transition = waitingState.AddTransition(state, false);
			transition.AddCondition(AnimatorConditionMode.Equals, i, "TextAnimation");
		}

		// Override the existing Animator Controller
		AnimatorOverrideController overrideController = new AnimatorOverrideController();
		overrideController.runtimeAnimatorController = newController;

		GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
	}
	IEnumerator DespawnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}
}
