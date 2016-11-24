using UnityEngine;
using System.Collections;

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
	
	private Animation _animation;

	/************************************************************************************************/
	private void Awake()
	{

		_animation = GetComponent<Animation>();
	} 

	void Start()
	{
		leftMod = Random.Range(-randHorizontalOffset, randHorizontalOffset);    // Pick a random offset
		damageStyle = new GUIStyle();
		damageStyleShadow = new GUIStyle();

		damageStyle.alignment = TextAnchor.UpperCenter;
		damageStyleShadow.alignment = TextAnchor.UpperCenter;

		UpdateFontScale();

		// Get array for animation on NumberManager

		foreach (AnimationClip clip in DamageNumberManager.instance.animations)
		{
			clip.legacy = true;
			_animation.AddClip(clip, clip.name);
			print(clip + ">" + clip.name);
		}
	}

	void OnEnable()
	{
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

	public void Initialize(Transform t, string value, string a)
	{
		transform.SetParent(t);

		damage = value;

		currentAnimation = a;

		alpha = 1;
		leftMod = Random.Range(-randHorizontalOffset, randHorizontalOffset);    // Pick a random offset

		print(currentAnimation);
		print(" -- ");

		_animation.Play(currentAnimation);
		StartCoroutine(Despawn(_animation.clip.length));
		UpdateFontScale();
	}

	IEnumerator Despawn(float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}

	private void UpdateFontScale()
	{
		if (distanceScaleFont)
		{
			float distance = Vector3.Distance(Camera.main.transform.position, transform.position);


			resizeFont = Mathf.CeilToInt(fontScale * 10.0f / Mathf.CeilToInt(distance));

			if (damageStyle != null)
			{
				damageStyle.fontSize = resizeFont;
				damageStyleShadow.fontSize = resizeFont;
			}
		}
		else
		{
			damageStyle.fontSize = fontScale;
			damageStyleShadow.fontSize = fontScale;
		}
	}
}