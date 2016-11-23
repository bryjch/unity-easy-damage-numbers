using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour {

	public float offsetRangeX = 0.1f;
	public float offsetRangeY = 0.1f;

	public static FloatingTextController instance = null;

	public GameObject floatingTextPrefab;
	
	public GameObject canvas;

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
		print("please go to " + pos);

		GameObject instance = SimplePool.Spawn(floatingTextPrefab, pos, Quaternion.identity);

		FloatingText floatingText = instance.GetComponent<FloatingText>();

		Vector2 randomizedPos = new Vector2(pos.x + Random.Range(-offsetRangeX, offsetRangeX), pos.y + Random.Range(-offsetRangeY, offsetRangeY));

		Vector2 screenPosition = Camera.main.WorldToScreenPoint(randomizedPos);

		floatingText.SetText(text);
	}

	public void CreateFloatingText(string text, Transform location)
	{
		GameObject instance = SimplePool.Spawn(floatingTextPrefab, Vector3.zero, Quaternion.identity);

		FloatingText floatingText = instance.GetComponent<FloatingText>();
		
		Vector2 randomizedPos = new Vector2(location.position.x + Random.Range(-offsetRangeX, offsetRangeX), location.position.y + Random.Range(-offsetRangeY, offsetRangeY));

		Vector2 screenPosition = Camera.main.WorldToScreenPoint(randomizedPos);
		
		floatingText.SetText(text);

		//floatingText.transform.SetParent(canvas.transform, false);

		//floatingText.transform.SetParent(location, false);

		//floatingText.transform.position = location.position;

		//floatingText.transform.position = screenPosition;	
	}

}
