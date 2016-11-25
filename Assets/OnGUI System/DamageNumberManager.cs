using UnityEngine;
using System.Collections;

public class DamageNumberManager : MonoBehaviour {

	public static DamageNumberManager instance = null;

	public GameObject damageNumber;

	public AnimationClip[] animations;

	public GUIStyle testStyle;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != null)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		//GameObject test = (GameObject)Resources.Load("Prefabs/DamageNumber");
	}
	
	public void SpawnDamageNumber(Transform t, string value, int animationIndex)
	{
		GameObject instance = SimplePool.Spawn(damageNumber, Vector3.zero, Quaternion.identity);
		instance.GetComponent<DamageNumber>().Initialize(t, value, animationIndex);
	}

	public void SpawnDamageNumber(Transform t, string value, string animationName)
	{
		// do something here that checks animationName then converts it to animationIndex based on the PREFAB gameobject's Animator
	}
}
