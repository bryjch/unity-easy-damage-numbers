using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

	private Animator animator;

	private Text floatingText;

	private AnimatorClipInfo[] clipInfo;

	/************************************************************************************************/
	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		floatingText = animator.GetComponent<Text>();

		clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	void OnEnable()
	{
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
	}


	private void FixedUpdate()
	{
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
	}

	/************************************************************************************************/
	public void SetText(string text)
	{
		floatingText.text = text;
	}

	public void Initialize(Transform t, string value)
	{
		transform.SetParent(t);
		transform.position = t.position;

		floatingText.text = value;

		if (clipInfo.Length >= 1)
			StartCoroutine(DespawnAfter(clipInfo[0].clip.length));
		else
			StartCoroutine(DespawnAfter(1.0f)); // Fallback despawn timer
	}

	IEnumerator DespawnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(gameObject);
	}
}
