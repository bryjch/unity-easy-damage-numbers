using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health = 100;

	public int prefabIndex = 0;
	public int animIndex = 0;

	private bool running = false;

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
		StartCoroutine(EveryXSecond(amount, Random.Range(0.4f, 0.7f)));
	}

	IEnumerator EveryXSecond(string text, float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);

		if (int.Parse(text) > 70)
		{
			FloatingText instance = FloatingTextController.instance.CreateFloatingText(text, transform, 0, 2);
		}
		else
		{
			FloatingText instance = FloatingTextController.instance.CreateFloatingText(text, transform, prefabIndex, animIndex);
		}

		
		//instance.SetScalingMode(FloatingText.ScalingMode.constantScale, 1.0f);
		//instance.SetParent(transform);

		personAnimator.ResetTrigger("Speak");
		personAnimator.SetTrigger("Speak");

		running = false;
	}
}
