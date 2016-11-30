using UnityEngine;
using System.Collections;

public class FloatingTextSpawner : MonoBehaviour
{
	[Header("Spawner Settings")]
	public bool running = false;
	public int prefabIndex = 0;
	public int animIndex = 0;
	public Vector2 spawnIntervalRange = new Vector2(0.4f, 0.6f);

	[Header("Spawned Text Settings")]
	public float textScale = 1.0f;
	public FloatingText.ScalingMode scalingMode = FloatingText.ScalingMode.constantScale;
	public bool parentToTransform = false;
	
	private Animator personAnimator;


	/************************************************************************************************/
	private void Start()
	{
		personAnimator = transform.parent.GetComponent<Animator>();
	}

	private void Update()
	{
		if (!running)
			this.TakeDamage(Random.Range(0, 100).ToString());
	}

	/************************************************************************************************/
	public void TakeDamage(string amount)
	{
		StartCoroutine(EveryXSecond(amount, Random.Range(spawnIntervalRange.x, spawnIntervalRange.y)));
	}

	IEnumerator EveryXSecond(string text, float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);

		FloatingText instance;
		if (int.Parse(text) > 70)
			instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, 3);
		else
			instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, animIndex);

		instance.SetScalingMode(scalingMode, textScale);

		if (parentToTransform)
			instance.SetParent(transform);

		personAnimator.ResetTrigger("Speak");
		personAnimator.SetTrigger("Speak");

		running = false;
	}
}
