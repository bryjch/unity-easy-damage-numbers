using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class DamageNumber : MonoBehaviour
{
	public string currentAnimation;

	[Tooltip("Default font scale. Play around with this!")]
	public int fontScale = 20;

	[Tooltip("Toggle whether font maintains size relative to distance.")]
	public bool distanceScaleFont = true;

	[Tooltip("Add random horizontal offset for spawning (Useful to reduce overlapping).")]
	public float randHorizontalOffset = 10.0f;

	[SerializeField]
	private float alpha = 1;        // Animate this using Animation system

	private string damage = "Ouch!";
	private float left;
	private float top;

	private Vector3 textPos;    // The world position the text will 'spawn' at

	private float leftMod;      // How off-center horizontally it will spawn - cached variable

	private int resizeFont = 20;
	
	GUIStyle damageStyle;
	GUIStyle damageStyleShadow;

	//private AnimatorClipInfo[] clipInfo;

	private float animationDuration;
	
	//private Animation _animation;

	/************************************************************************************************/
	void Start()
	{
		leftMod = Random.Range(-randHorizontalOffset, randHorizontalOffset);    // Pick a random offset
		damageStyle = new GUIStyle();
		damageStyleShadow = new GUIStyle();

		damageStyle.alignment = TextAnchor.UpperCenter;
		damageStyleShadow.alignment = TextAnchor.UpperCenter;

		UpdateFontScale();

		UpdateAnimatorController();
	}

	/************************************************************************************************/
	void FixedUpdate()
	{

		// Convert world space to make it screen relative; keep text font scale
		Vector3 pos = (transform.position + new Vector3(0.0f, transform.lossyScale.y, 0.0f));

		if (Camera.main != null)
			textPos = Camera.main.WorldToScreenPoint(pos);
		else
			textPos = new Vector3(0.0f, -1000.0f, 0.0f);


		left = textPos.x + leftMod;
		top = Screen.height - (textPos.y);

		Color color = new Color(1, 1, 1, alpha);
		damageStyle.normal.textColor = color;

		Color bgcolor = new Color(0, 0, 0, alpha);
		damageStyleShadow.normal.textColor = bgcolor;

	}

	/************************************************************************************************/
	void OnGUI()
	{
		if (textPos.z > 0)
		{
			GUI.Label(new Rect(left - 1 - 25, (top - 1), 50, 25), damage, damageStyleShadow);
			GUI.Label(new Rect(left - 1 - 25, (top + 1), 50, 25), damage, damageStyleShadow);
			GUI.Label(new Rect(left + 1 - 25, (top - 1), 50, 25), damage, damageStyleShadow);
			GUI.Label(new Rect(left + 1 - 25, (top + 1), 50, 25), damage, damageStyleShadow);
			GUI.Label(new Rect(left + 0 - 25, (top), 50, 25), damage, damageStyle);
		}
	}

	public void Initialize(Transform t, string value, int animationIndex = 0, GUIStyle fontStyle = null)
	{
		transform.SetParent(t);

		damage = value;
		alpha = 1;
		leftMod = Random.Range(-randHorizontalOffset, randHorizontalOffset);    // Pick a random offset
		
		UpdateFontScale(t);

		GetComponent<Animator>().SetInteger("TextAnimation", animationIndex);
		
		StartCoroutine(Despawn(1.0f));
	}

	IEnumerator Despawn(float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}

	private void UpdateFontScale(Transform t = null)
	{
		if (t == null)
			t = transform;

		if (distanceScaleFont)
		{
			float distance = Vector3.Distance(Camera.main.transform.position, t.position);

			//print(transform.name + " at " + transform.position);
			
			resizeFont = Mathf.CeilToInt(fontScale * 10.0f / Mathf.CeilToInt(distance));

			if (damageStyle != null)
			{
				damageStyle.fontSize = resizeFont;
				damageStyleShadow.fontSize = resizeFont;
			}
			damage = Mathf.Ceil(distance).ToString();
		}
		else
		{
			damageStyle.fontSize = fontScale;
			damageStyleShadow.fontSize = fontScale;
		}
	}

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
		for (int i = 0; i < DamageNumberManager.instance.animations.Length; i++)
		{
			AnimationClip clip = DamageNumberManager.instance.animations[i];

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

		GetComponent<Animator>().runtimeAnimatorController = overrideController;
	}
}