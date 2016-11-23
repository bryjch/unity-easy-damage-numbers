using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health = 100;

	private bool running = false;

	public void TakeDamage(int amount)
	{
		StartCoroutine(EveryXSecond(0.2f));
		Debug.LogFormat("{0} was dealt {1} damage.", this.name, amount);
	}
	
	void OnMouseOver()
	{
		if (!running)
			this.TakeDamage(Random.Range(0, 100));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			FloatingTextController.instance.CreateFloatingText("idiot", transform.position + new Vector3(0, 2, 0));
		}
	}

	IEnumerator EveryXSecond(float delay)
	{
		running = true;
		yield return new WaitForSeconds(delay);
		FloatingTextController.instance.CreateFloatingText("idiot", transform.position + new Vector3(0, 1, 0));
		running = false;
	}
}
