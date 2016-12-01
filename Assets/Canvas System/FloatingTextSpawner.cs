using UnityEngine;
using System.Collections;

public class FloatingTextSpawner : MonoBehaviour
{
	[Header("Spawner Settings")]
	public string text = "";
	public bool running = false;
	public int prefabIndex = 0;
	public int animIndex = 0;
	public Vector2 spawnIntervalRange = new Vector2(0.4f, 0.6f);

	[Header("Spawned Text Settings")]
	public float textScale = 1.0f;
	public FloatingText.ScalingMode scalingMode = FloatingText.ScalingMode.constantScale;
	public bool parentToTransform = false;
	public bool alwaysOnTop = false;
	
	private Animator personAnimator;


	/************************************************************************************************/
	private void Start()
	{
		personAnimator = transform.parent.GetComponent<Animator>();
	}

	private void Update()
	{
		if (!running)
		{
			if (text == "")
				StartCoroutine(EveryXSecond(Random.Range(0, 100).ToString(), Random.Range(spawnIntervalRange.x, spawnIntervalRange.y)));
			else
				StartCoroutine(EveryXSecond(text, Random.Range(spawnIntervalRange.x, spawnIntervalRange.y)));

		}
	}

	/************************************************************************************************/

	IEnumerator EveryXSecond(string text, float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);

		FloatingText instance;

		int val;

		// play crit animation if damage is high enough
		if (int.TryParse(text, out val) && val > 70)
			instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, 3);
		else
			instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, animIndex);

		instance.SetScalingMode(scalingMode, textScale);

		if (parentToTransform)
			instance.SetParent(transform);

		if (alwaysOnTop)
			instance.SetAlwaysOnTop(alwaysOnTop);

		personAnimator.ResetTrigger("Speak");
		personAnimator.SetTrigger("Speak");

		running = false;
	}
}
