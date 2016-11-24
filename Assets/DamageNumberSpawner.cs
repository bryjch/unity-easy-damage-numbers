using UnityEngine;
using System.Collections;

public class DamageNumberSpawner : MonoBehaviour {

	public string textValue = "water";

	public string animationName = "FloatingText_DefaultAnim";

	public Vector2 intervalRange = new Vector2(0.2f, 0.4f);

	private bool running = false;

	// Update is called once per frame
	void FixedUpdate () {
		if (!running)
			StartCoroutine(EveryXSecond(Random.Range(intervalRange.x, intervalRange.y)));
	}

	IEnumerator EveryXSecond(float delay)
	{
		running = true;

		yield return new WaitForSeconds(delay);
	
		DamageNumberManager.instance.SpawnDamageNumber(transform, textValue, animationName);
		
		running = false;
	}
}
