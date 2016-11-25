using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health = 100;

	private bool running = false;

	public void TakeDamage(string amount)
	{
		StartCoroutine(EveryXSecond(amount, 0.4f));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			FloatingTextController.instance.CreateFloatingText("idiot", transform);
			//FloatingTextController.instance.CreateFloatingText("idiot", transform.position + new Vector3(0, 2, 0));
		}

		if (!running)
			this.TakeDamage(Random.Range(0, 100).ToString());
	}

	IEnumerator EveryXSecond(string text, float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);
		//FloatingTextController.instance.CreateFloatingText("idiot", transform.position + new Vector3(0, 1, 0));
		FloatingTextController.instance.CreateFloatingText(text, transform);
		running = false;
	}
}
