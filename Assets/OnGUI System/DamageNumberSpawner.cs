using UnityEngine;
using System.Collections;

public class DamageNumberSpawner : MonoBehaviour {

	public string textValue = "water";

	public int animationIndex = 0;

	public Vector2 intervalRange = new Vector2(0.2f, 0.4f);

	private bool running = false;

	private Animator personAnimator;

	private void Awake()
	{
		personAnimator = transform.parent.GetComponent<Animator>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!running)
			StartCoroutine(EveryXSecond(Random.Range(intervalRange.x, intervalRange.y)));
	}

	IEnumerator EveryXSecond(float delay)
	{
		running = true;

		yield return new WaitForSeconds(delay);
	
		DamageNumberManager.instance.SpawnDamageNumber(transform, textValue, animationIndex);

		//personAnimator.Play("Person_Speak", 0);
		personAnimator.ResetTrigger("Speak");
		personAnimator.SetTrigger("Speak");
		personAnimator.SetInteger("Test", 69);

		running = false;
	}
}
