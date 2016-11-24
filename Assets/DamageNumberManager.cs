using UnityEngine;
using System.Collections;

public class DamageNumberManager : MonoBehaviour {

	public static DamageNumberManager instance = null;

	public AnimationClip[] animations;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);
	}

	public void SpawnDamageNumber(Transform t, string value, string animationName)
	{
		GameObject instance = SimplePool.Spawn((GameObject)Resources.Load("Prefabs/DamageNumber"), transform.position, Quaternion.identity);
		instance.GetComponent<DamageNumber>().Initialize(t, value, animationName);
	}
}
