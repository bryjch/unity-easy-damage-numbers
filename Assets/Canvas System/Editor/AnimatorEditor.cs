using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

// This script generates a new AnimatorController (AC) and AnimatorOverrideController (AOC)
// based on what animations are in FloatingTextController's animations[]. The intention is 
// to remove the need to manually set up controllers after adding new animations.

// AOC is used in FloatingTextController instead because AC requires UnityEditor.Animations. 
// This causes a lot of problems with non-Editor scripts (i.e. FloatingTextController) during Build. 
// AOC can be referenced without UnityEditor, so FloatingTextController uses that instead.

// The reason there needs to be both a AnimatorController AND AnimatorOverrideController
// is because AOC requires an AC reference as an asset. Trust me, I would've much preferred
// that this script only generated an AOC, but that had issues.

[CustomEditor(typeof(FloatingTextController))]
public class AnimatorEditor : Editor
{
	// If there was no existing AnimatorOverrideController found on FloatingTextController,
	// the newly generated AC & AOC will be saved at this location with these names.
	private string DEFAULT_CONTROLLER_PATH = "Assets/";
	private string DEFAULT_CONTROLLER_NAME = "FTAnimatorController";
	private string DEFAULT_OVERRIDE_CONTROLLER_NAME = "FTAnimatorOverrideController";

	// Reference to "this" FloatingTextController script
	private FloatingTextController myScript;
	
	/************************************************************************************************/
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		myScript = (FloatingTextController)target;

		// <Update Animator> button will create a new AnimatorController using the
		// given animations in FloatingTextController
		if (GUILayout.Button("Update Animator Controller"))
		{
			// Generate the new AnimatorController & AnimatorOverrideController
			AnimatorController updatedController = CreateAnimatorController();
			AnimatorOverrideController updatedOverrideController = CreateAnimatorOverrideController(updatedController);

			myScript.animatorOverrideController = updatedOverrideController;

			// Some random things that ensures editor changes don't reset on Play
			EditorUtility.SetDirty(myScript);
			EditorSceneManager.MarkSceneDirty(myScript.gameObject.scene);
			AssetDatabase.SaveAssets();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(myScript);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
	}

	/************************************************************************************************/
	public AnimatorController CreateAnimatorController()
	{
		// The new controller that will be created based on Manager animations
		AnimatorController newController = new AnimatorController();
		newController.name = DEFAULT_CONTROLLER_NAME;
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
		
		//// Save OR update the new AnimatorController as an asset (.controller)

		// Search if there is already an AnimatorController (and get its path if so)
		string assetPath = null;

		if (myScript.animatorOverrideController != null)
			assetPath = AssetDatabase.GetAssetPath(myScript.animatorOverrideController.runtimeAnimatorController);

		Object existingController = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

		if (existingController == null)
		{
			// Create the new AnimatorController in the specified default directory
			AssetDatabase.CreateAsset(newController, DEFAULT_CONTROLLER_PATH + DEFAULT_CONTROLLER_NAME + ".controller");
			existingController = newController;
			Debug.LogError("AnimatorController not found. Creating new one in " + DEFAULT_CONTROLLER_PATH + " directory.");
		}
		else
		{
			// Update the existing AnimatorController copy with latest data
			EditorUtility.CopySerialized(newController, existingController);
			Debug.Log("Updated existing AnimatorController found at " + assetPath + ".");
		}

		// Make sure the returned controller refers to the generated 'asset', not the controller 'within this scope'
		AnimatorController updatedController_asset = (AnimatorController)existingController;
		
		AssetDatabase.SaveAssets();

		return updatedController_asset;
	}


	public AnimatorOverrideController CreateAnimatorOverrideController(AnimatorController ac)
	{
		AnimatorOverrideController newOverrideController = new AnimatorOverrideController();
		newOverrideController.runtimeAnimatorController = ac;
		newOverrideController.name = DEFAULT_OVERRIDE_CONTROLLER_NAME;

		// search if there is already an animator controller (and get its path if so)
		string assetPath = AssetDatabase.GetAssetPath(myScript.animatorOverrideController);

		Object existingOverrideController = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
		
		if (existingOverrideController == null)
		{
			// Create a new AnimatorOverrideController in the specified default directory
			AssetDatabase.CreateAsset(newOverrideController, DEFAULT_CONTROLLER_PATH + DEFAULT_OVERRIDE_CONTROLLER_NAME + ".overrideController");
			existingOverrideController = newOverrideController;
			Debug.LogError("AnimatorOverrideController not found. Creating new one in " + DEFAULT_CONTROLLER_PATH + " directory.");
		}
		else
		{
			// Update the existing AnimatorOverrideController copy with latest data
			EditorUtility.CopySerialized(newOverrideController, existingOverrideController);
			Debug.Log("Updated existing AnimatorOverrideController found at " + assetPath + ".");
		}

		// Make sure the returned controller refers to the generated 'asset', not the controller 'within this scope'
		AnimatorOverrideController updatedController_asset = (AnimatorOverrideController)existingOverrideController;

		AssetDatabase.SaveAssets();

		return updatedController_asset;
	}
}