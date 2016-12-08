using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FloatingTextController))]
public class AnimatorEditor : Editor
{
	// If there was no existing AnimatorController found on FloatingTextController,
	// the newly generated AnimatorController will be saved at this location.
	public string defaultControllerPath = "Assets/";

	public string defaultControllerName = "FTAnimatorController";
	public string defaultOverrideControllerName = "FTAnimatorOverrideController";

	public AnimatorController latestAnimatorController;

	private FloatingTextController myScript;
	
	/************************************************************************************************/
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		myScript = (FloatingTextController)target;

		if (GUI.changed)
		{
			EditorUtility.SetDirty(myScript);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		// <Update Animator> button will create a new AnimatorController using the
		// given animations in FloatingTextController
		if (GUILayout.Button("Update Animator"))
		{
			// Generate the new AnimatorController
			AnimatorController updatedController = CreateAnimatorController();

			AnimatorOverrideController updatedOverrideController = CreateAnimatorOverrideController(updatedController);

			myScript.animatorOverrideController = updatedOverrideController;

			// Some random thing that ensures editor changes don't reset on Play
			EditorUtility.SetDirty(myScript);
			EditorSceneManager.MarkSceneDirty(myScript.gameObject.scene);
			AssetDatabase.SaveAssets();

		}
	}

	/************************************************************************************************/
	public Object CreateOrReplaceAsset(Object asset, string path)
	{
		Object existingAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
	
		if (existingAsset == null)
		{
			// Create a new object in the specified directory (default: Assets/)
			AssetDatabase.CreateAsset(asset, defaultControllerPath + defaultControllerName + ".controller");
			AssetDatabase.CreateAsset(asset, defaultControllerPath + defaultControllerName + ".overrideController");
			existingAsset = asset;
			Debug.LogError("Animator Override Controller not found. Creating new one in " + defaultControllerPath + " directory.");
		}
		else
		{
			// Replace the existing copy with <asset> data
			EditorUtility.CopySerialized(asset, existingAsset);
			Debug.Log("Overriding existing Animator Override Controller found at " + path + ".");
		}
		AssetDatabase.SaveAssets();
		return existingAsset;
	}

	public AnimatorController CreateAnimatorController()
	{
		// The new controller that will be created based on Manager animations
		AnimatorController newController = new AnimatorController();
		newController.AddLayer("DefaultLayer");

		// Add a parameter that will determine the animation states
		AnimatorControllerParameter animatorParameter = new AnimatorControllerParameter();
		animatorParameter.type = AnimatorControllerParameterType.Int;
		animatorParameter.name = "TextAnimation";
		animatorParameter.defaultInt = 999;
		newController.AddParameter(animatorParameter);

		// Add state machine
		AnimatorStateMachine rootStateMachine = newController.layers[0].stateMachine;
		AnimatorStateMachine stateMachine = rootStateMachine.AddStateMachine("TextAnimationStateMachine");

		// Create a default state to prevent animation auto playing index 0
		AnimatorState waitingState = stateMachine.AddState("Waiting");

		//foreach (AnimationClip clip in DamageNumberManager.instance.animations)
		for (int i = 0; i < myScript.animations.Length; i++)
		{
			AnimationClip clip = myScript.animations[i];

			// Add new state based on the AnimationClip
			AnimatorState state = stateMachine.AddState(clip.name);
			state.motion = clip;

			// Create transition from "Waiting" to the new state
			AnimatorStateTransition transition = waitingState.AddTransition(state, false);
			transition.AddCondition(AnimatorConditionMode.Equals, i, "TextAnimation");
		}
		
		newController.name = defaultControllerName;


		///// SAVE/UPDATE THE NEW ANIMATORCONTROLLER AS A '.controller' ASSET

		// search if there is already an animator controller (and get its path if so)
		string assetPath = null;

		if (myScript.animatorOverrideController != null)
			assetPath = AssetDatabase.GetAssetPath(myScript.animatorOverrideController.runtimeAnimatorController);

		Object existingController = AssetDatabase.LoadAssetAtPath<Object>(assetPath);


		if (existingController == null)
		{
			// Create a new object in the specified directory (default: Assets/)
			AssetDatabase.CreateAsset(newController, defaultControllerPath + defaultControllerName + ".controller");
			existingController = newController;
			Debug.LogError("Animator Controller not found. Creating new one in " + defaultControllerPath + " directory.");
		}
		else
		{
			// Replace the existing copy with latest data
			EditorUtility.CopySerialized(newController, existingController);
			Debug.Log("Overriding existing Animator Controller found at " + assetPath + ".");
		}
		

		AnimatorController updatedController_asset = (AnimatorController)existingController;


		AssetDatabase.SaveAssets();

		latestAnimatorController = updatedController_asset;

		return updatedController_asset;
	}


	// smd

	public AnimatorOverrideController CreateAnimatorOverrideController(AnimatorController n)
	{
		AnimatorOverrideController newOverrideController = new AnimatorOverrideController();
		newOverrideController.runtimeAnimatorController = n;
		newOverrideController.name = "FloatingTextAnimatorController";

		// search if there is already an animator controller (and get its path if so)
		string assetPath = AssetDatabase.GetAssetPath(myScript.animatorOverrideController);

		Object existingOverrideController = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
		
		if (existingOverrideController == null)
		{
			// Create a new object in the specified directory (default: Assets/)
			AssetDatabase.CreateAsset(newOverrideController, defaultControllerPath + defaultOverrideControllerName + ".overrideController");
			existingOverrideController = newOverrideController;
			Debug.LogError("Animator Override Controller not found. Creating new one in " + defaultControllerPath + " directory.");
		}
		else
		{
			// Replace the existing copy with latest data
			EditorUtility.CopySerialized(newOverrideController, existingOverrideController);
			Debug.Log("Overriding existing Animator Override Controller found at " + assetPath + ".");
		}

		AnimatorOverrideController updatedController_asset = (AnimatorOverrideController)existingOverrideController;

		AssetDatabase.SaveAssets();

		return updatedController_asset;
	}
}