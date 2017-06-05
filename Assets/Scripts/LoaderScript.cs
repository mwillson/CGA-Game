using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using M8.Animator;
/// <summary>
/// This script should load save data from a file about a player. It should send that player data
/// to the GameManager.
/// </summary>
/// 


public class LoaderScript : MonoBehaviour {
	
	public Transform gamemanagerprefab;

	//private BasicGun gun;
	//private DashScript dash;
	//private Disruptor dis;
	public string sceneName, npcFileName;

	// Use this for initialization
	// Called before any Start functions
	// Currently only creates player via player constructor. Needs to load save data from file!!!
	void Awake () {
		//SaveLoad.LoadDataFromFile ();
		//SaveLoad.LoadLevelsInfo()

		Instantiate (gamemanagerprefab, new Vector3 (0, 0, 0), new Quaternion ());


		//PlayerData pdata = SaveLoad.CreatePlayerData(GameManager.instance.GetPlayer());
	

		Debug.Log ("datapath: " + Application.persistentDataPath);
		//load all the savegame data from the savedGames folder
		//SaveLoad.LoadAllData ();

		//add item script components to player gameobject
		//this might need to always be done, so its good to have it execute on scene load
		//probably should take current inventory and add appropriate scripts based on that
		//for now, hardcoded is fine
		/*dis = (Disruptor)GameObject.FindGameObjectWithTag("Player").AddComponent<Disruptor>();
		gun = (BasicGun)GameObject.FindGameObjectWithTag("Player").AddComponent<BasicGun>();
		dash = (DashScript)GameObject.FindGameObjectWithTag ("Player").AddComponent<DashScript>();*/

		//currently adds npc's by finding all objects in scene with a dialogue controller attached, maybe this will change
		sceneName = SceneManager.GetActiveScene ().name;
		//if we are on the 'title' screen still, change the name of the scene to menu1 for gamemanager.currentlevel purposes
		//this is a ONE TIME EXCEPTION due to the way menu1 is additively loaded from title
		if (sceneName == "title")
			sceneName = "menu1";
		if (sceneName != "worldmap") {
			Debug.Log ("scene: " + sceneName);
			GameManager.instance.SetCurrentLevel (sceneName);
			Debug.Log (GameManager.instance.GetCurrentLevel ());
		}



		//currently adds new npc's
		//SHOULD actually get file data if there is any
		/*if(){
		//this needs to be moved into mapmovescript so it can be calculated as level is loaded asynchronously i think
		//also because i dont want to have this loader script requirement in every freakin scene
		}else {*/
			
		//}

		/*DialogueController[] npcdialogues = FindObjectsOfType<DialogueController> ();
		foreach (DialogueController dc in npcdialogues) {
			NPC dude = new NPC (dc.gameObject.name);
			Debug.Log("spanpos?" + dude.currentspawnpos.V3);
			GameObject.Find (dude.name).transform.localPosition = dude.currentspawnpos.V3;
			Debug.Log(dude.name + "'s newpos?" + GameObject.Find (dude.name).transform.localPosition);

			//add npc to level object if it's not part of it already
			if (!GameManager.instance.GetCurrentLevel ().HasNPC (dc.gameObject.name)) {
				GameManager.instance.GetCurrentLevel ().AddNPC (dude);
			}
		}*/
	
	}

	/*public void ResolveCondition(DialogueController dc){
		bool retVal = false;
		MethodInfo method;
		Type actorType;
		switch (dc.conditionClass) {
		case "Player"://do something
			actorType = Type.GetType ("Player");
			method = actorType.GetMethod (dc.conditionMethod);
			retVal = (bool)(method.Invoke (GameManager.instance.GetPlayer (), dc.conditionMethodParams));
			break;
		default:
			break;
		}

		//this is terrible
		if (retVal == true) {
			npcFileName = dc.filenames[0];
		} else {
			npcFileName = dc.filenames[1];
		}
	}*/

	void Start(){
		
		/*DialogueController[] npcdialogues = FindObjectsOfType<DialogueController> ();
		foreach (DialogueController dc in npcdialogues) {
			NPC dude = new NPC (dc.gameObject.name);
			Debug.Log (dude.name);
			GameManager.instance.GetCurrentLevel ().AddNPC (dude);
		}*/
		//GameManager.instance.SetCurrentLevel (sceneName);

		if (sceneName != "title" && sceneName != "menu1" && sceneName != "worldmap") {
			//GameManager.instance.SetCurrentLevel (sceneName);
			//if we are on a level
			//Debug.Log("stting up player instance from save file" + SceneManager.GetActiveScene ().name);
			//GameManager.instance.SetPlayer (SaveLoad.CreatePlayer (GameManager.instance.GetPlayerData()));
			SetUpHUD (GameManager.instance.GetPlayer ());

			//update most recent level to this one and then save the level data
			SaveLoad.SaveLevelData(GameManager.instance.GetCurrentLevel().name);


			//AnimatorTimeline.Play ("Enter");
			//load up cutscenes
			if (GameObject.Find ("Cutscenes") != null) {
				//get all scene cutscenes that have been saved so far
				//if there are none, this bit does nothing
				List<KeyValuePair<string,bool>> cutsceneslist = GameManager.instance.GetCurrentLevel().cutscenes;
				foreach (KeyValuePair<string,bool> cutsceneinfo in cutsceneslist) {
					//basically just say whether it has played already or not
					Debug.Log("object? "+ cutsceneinfo.Key);
					//key is the relevant gameobject's name in the scene
					//value is whether it's played or not
					GameObject.Find(cutsceneinfo.Key).GetComponent<Cutscene> ().hasPlayed = cutsceneinfo.Value;
				}

				Cutscene[] cutscenes = GameObject.Find ("Cutscenes").GetComponentsInChildren<Cutscene> ();
				//if there is one to play on scene start and it hasn't yet played, play it
				foreach (Cutscene cutscene in cutscenes) {
					if (cutscene.playOnStart && !cutscene.hasPlayed) {
						//execute will play the cutscene and add it to this level's cutscene list
						cutscene.Execute();
					}
				}

			}
			//END CUTSCENE STUFF
		} 

		/*DialogueController[] npcdialogues = FindObjectsOfType<DialogueController> ();
		foreach (DialogueController dc in npcdialogues) {
			NPC dude = new NPC (dc.gameObject.name);
			Debug.Log(dude.name + "'s start() pos?" + GameObject.Find (dude.name).transform.localPosition);

		}*/

			//disabling item menu controller for now since we are loading a blank game with a fresh, empty player
			/*GameManager.objectcontroller.AddControl (GameObject.Find ("EventManagement"), GameObject.FindGameObjectWithTag ("Player"));
			GameManager.objectcontroller.DisableComponent (
				GameObject.Find ("EventManagement"),
				GameObject.FindGameObjectWithTag ("Player"),
				"ItemMenuController");*/
			Vector3 startpos;
				startpos = GameObject.Find ("PlayerSpawn").transform.position;


			//if we have a 'return-to' point, and we are in it's scene, start there
			KeyValuePair<string, Vector3> retpoint = GameManager.instance.returnpoint;
			if (retpoint.Value != Vector3.zero && retpoint.Key == SceneManager.GetActiveScene().name) {
				Debug.Log ("Changing start position!");
				startpos = GameManager.instance.returnpoint.Value;
			}
			GameObject.Find ("Main Camera").transform.position = new Vector3 (startpos.x, startpos.y + 2f, GameObject.Find ("Main Camera").transform.position.z);
			
			//NEED TO REDO FOR MATE ANIMATOR PLUGIN
			//CutSceneManager.instance.currentCutScene = GameObject.Find ("PlayerSpawnScene").AddComponent<IntroCutscene> ();
			//CutSceneManager.instance.currentCutScene.SetCutSceneObject (GameObject.Find ("PlayerSpawnScene"));
			//Debug.Log (CutSceneManager.instance.currentCutScene.cutSceneObject);

			//GameObject.Find ("PlayerSpawnScene").GetComponent<QuickCutsceneController> ().Start ();
			//CutSceneManager.instance.currentCutScene.Start ();
			//makes sure player is not visible till cutscene starts?
			GameObject.Find ("Player").transform.position = startpos;
			GameManager.instance.GetPlayer ().SetHealth (48);
	}
		

	//Sets up all UI elements based on player information
	//this may need to be called each time a level is loaded if hud is not persistent across scenes
	void SetUpHUD(Player player) {

		/*if (player.HasItems ()) {
			//add item scripts
			foreach (Item item in player.GetInventory().GetItemsList()) {
				Debug.Log ("adding item script from loader");
				player.AddItemScriptToPlayer(item);
			}

			if (player.GetEquippedItem (1) != null) {
				Debug.Log ("setting hud image for item 1!");
				Debug.Log ("icon: " + player.GetEquippedItem (1).GetIcon ());
				ItemMenu.PutItemInSlot (player.GetEquippedItem (1), 1);
				ItemMenu.UpdateEquippedItemIcon (player.GetEquippedItem (1), 1); 
			}
			if (player.GetEquippedItem (2) != null) {
				ItemMenu.PutItemInSlot (player.GetEquippedItem (2), 2);

				ItemMenu.UpdateEquippedItemIcon (player.GetEquippedItem (2), 2);
			}
		}
		//GameObject.FindGameObjectWithTag ("hud").transform.Find ("Health").gameObject.GetComponent<Text> ().text = "Health: " + player.getHealth ();
		*/
	}
		
	void Update(){
		//DialogueController[] npcdialogues = FindObjectsOfType<DialogueController> ();
		//bad!!
		//foreach (DialogueController dc in npcdialogues) {
		//	NPC dude = GameManager.instance.GetCurrentLevel ().GetNPC(dc.gameObject.name);
		//	Debug.Log(dude.name + "'s  pos?" + GameObject.Find (dude.name).transform.localPosition);
			//GameObject.Find (dude.name).transform.localPosition = dude.currentspawnpos.V3;

		//}
	}

}
