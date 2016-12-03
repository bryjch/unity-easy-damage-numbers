using UnityEngine;
using System.Collections;

public class Sweeping : MonoBehaviour {
	
	public float speed = 1.0f;

	public Vector3 direction = new Vector3(0, 0, 0);

	private bool running = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!running)
			StartCoroutine(Sweep());
	}

	IEnumerator Sweep()
	{
		running = true;

		float elapsedTime = 0;
		float perc = 0;

		Vector3 startPos = transform.position;

		while (elapsedTime < 2.0f)
		{
			elapsedTime += Time.deltaTime;

			perc = elapsedTime / 2.0f;
			
			transform.position = Vector3.Slerp(startPos, startPos + direction, perc * speed);

			yield return new WaitForFixedUpdate();
		}

		running = false;
		direction = -direction;
	}
}
