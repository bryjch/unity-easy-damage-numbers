using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {

	public float scaling = 1.0f;

	public float offsetRangeX = 0.1f;
	public float offsetRangeY = 0.1f;

	public static FloatingTextController instance = null;

	public GameObject floatingTextPrefab;
	
	public GameObject canvas;

	public AnimationClip[] animations;
	
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		if (floatingTextPrefab == null) {
			print("[Floating Text] not assigned. Searching for Prefab: \"Resources/Prefabs/FloatingText\".");
			floatingTextPrefab = Resources.Load("Prefabs/FloatingText") as GameObject;
		}

		if (canvas == null)
		{
			print("[Canvas] not assigned. Searching for GameObject: \"Canvas\".");
			canvas = GameObject.Find("Canvas");
		}
	}

	public void CreateFloatingText(string text, Vector3 pos)
	{
		//print("please go to " + pos);

		print("this is happening");	

		GameObject instance = SimplePool.Spawn(floatingTextPrefab, Vector3.zero, Quaternion.identity);
		//instance.GetComponent<FloatingText>().Initialize(pos, "hello world!")

		//FloatingText floatingText = instance.GetComponent<FloatingText>();

		//Vector2 randomizedPos = new Vector2(pos.x + Random.Range(-offsetRangeX, offsetRangeX), pos.y + Random.Range(-offsetRangeY, offsetRangeY));

		//Vector2 screenPosition = Camera.main.WorldToScreenPoint(randomizedPos);

		//floatingText.SetText(text);
	}

	public void CreateFloatingText(string text, Transform t, int animIndex)
	{
		GameObject instance = SimplePool.Spawn(floatingTextPrefab, Vector3.zero, Quaternion.identity);
		instance.GetComponent<FloatingText>().Initialize(t, text, animIndex, scaling);
	}

	public void CreateFloatingTextStatic(string text, Transform t)
	{
		GameObject instance = SimplePool.Spawn(floatingTextPrefab, Vector3.zero, Quaternion.identity);
		instance.GetComponent<FloatingText>().InitializeStatic(t, text);
	}
}
