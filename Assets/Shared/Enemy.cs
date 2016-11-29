using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health = 100;

	public int prefabIndex = 0;
	public int animIndex = 0;

	private bool running = false;

	public void TakeDamage(string amount)
	{
		StartCoroutine(EveryXSecond(amount, 0.4f));
	}

	private void Update()
	{
		if (!running)
			this.TakeDamage(Random.Range(0, 100).ToString());
	}

	IEnumerator EveryXSecond(string text, float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);

		FloatingText instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, animIndex);
		//instance.SetScalingMode(FloatingText.ScalingMode.constantScale, 1.0f);
		//instance.SetParent(transform);

		running = false;
	}
}
