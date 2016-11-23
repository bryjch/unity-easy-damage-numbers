using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

	public float despawnAfterSecs = 1.0f;

	private Animator animator;

	private Text damageText;

	private AnimatorClipInfo[] clipInfo;

	void OnEnable()
	{
		animator = GetComponentInChildren<Animator>();
		clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		damageText = animator.GetComponent<Text>();

		GetComponent<Canvas>().worldCamera = Camera.main;

		if (clipInfo.Length >= 1)
			StartCoroutine(DespawnAfter(clipInfo[0].clip.length));
		else
			StartCoroutine(DespawnAfter(despawnAfterSecs));
	}

	IEnumerator DespawnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		SimplePool.Despawn(this.gameObject);
	}

	private void FixedUpdate()
	{
		//Vector3 pos = (transform.position + new Vector3(0.0f, transform.lossyScale.y, 0.0f));
		//transform.position = pos;

		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
	}

	public void SetText(string text)
	{
		damageText.text = text;
	}
}
