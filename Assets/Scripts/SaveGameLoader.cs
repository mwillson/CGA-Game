using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveGameLoader : MonoBehaviour {

	AsyncOperation async, async1, async2, async3, oneToUse;
	public int saveSlot;
	public GameObject infoPrefab, itemIconPrefab;
	GameObject infoObj;
	public string whichFile;
	public List<AsyncOperation> gameLoads;
	public int whichLoad;
	public XmlDocument levelsDoc;
	public string sceneToLoad;

	// Use this for initialization
	void Awake () {
		//DontDestroyOnLoad (gameObject);
		//StartCoroutine(Load());
	}

	void Start() {
		infoPrefab = (GameObject)Resources.Load ("Prefabs/SaveGameInfo");
		itemIconPrefab = (GameObject)Resources.Load ("Prefabs/Image");
		TextAsset d = (TextAsset)Resources.Load ("LevelData/masterlevelslist");
		levelsDoc = new XmlDocument();
		levelsDoc.LoadXml(d.text);
		oneToUse = null;
		gameLoads = new List<AsyncOperation> ();
		whichLoad = 0;
	}

	// Update is called once per frame
	void Update () {
		
	}
		
	//This Loads A 'Save Game'
	//
	//IMPORTANT  NOTE: A 'Save Game' might be a New Game with basic preloaded info
	public void LoadSaveGame(string whichone){
		Debug.Log ("Loading Game From Talk Event.");
		//possibly necessary if this event gets fired multiple times simultaneously
		//keep just in case, should be taken care of though vis a vis the event being removed from its delegate in dialogue controller
		//after it gets fired off
		//if (this != null) {
		GameObject.Find("screenFader").GetComponent<Fading>().BeginFade(1);
		GameObject.Find ("Loader").GetComponent<Fading> ().BeginFade (1);
			//StartCoroutine (LoadGameAsync (whichone));
		LoadGameInfo(whichone);
		SceneManager.LoadScene (sceneToLoad);
		//}
	}

	//this gets called once upon game load
	//so we should create our player and world data for this game instance here
	public IEnumerator LoadGameAsync(string which){
		whichFile = which;

		//create new asynchronous operation each time this is called?
		gameLoads.Add (new AsyncOperation ());

		//let game manager know we are using this save slot
		GameManager.instance.SetSaveSlot (this.saveSlot);

		//create an async operation to load up the worldmap scene depending on which file chosen
		gameLoads [whichLoad] = SceneManager.LoadSceneAsync ("worldmap", LoadSceneMode.Additive);
		gameLoads [whichLoad].allowSceneActivation = false;
		yield return gameLoads [whichLoad];
		/*	switch (which) {
		case "File1":
			Debug.Log ("Loading file1");
			if (async1 == null) {
				async1 = SceneManager.LoadSceneAsync ("worldmap", LoadSceneMode.Additive);
			}
			async1.allowSceneActivation = false;
			oneToUse = async1;
			yield return async1;
			break;
		case "File2":
			Debug.Log ("Loading File2");
			oneToUse = async2;
			if (async2 == null) {
				async2 = SceneManager.LoadSceneAsync ("worldmap", LoadSceneMode.Additive);
			}
			async2.allowSceneActivation = false;
			yield return async2;
				break;
			default:
				break;
			}
		*/
			//always load world map first when starting up a saved game
			//async = SceneManager.LoadSceneAsync ("worldmap", LoadSceneMode.Additive);
			//async.allowSceneActivation = false;
			//need to yield return the AsyncOperation bc it has to do something before i can get info from the scene it loads
			//yield return async;
			//setting up player instance from save file
			//createplayer also builds the level graph from saved data

	
			//if the data is not that of a new game
		if (!SaveLoad.IsNewGame (GameManager.instance.GetSaveSlot ())) {
			Debug.Log ("not a new game, loading file data");
			// put the data from the file into the save slot we are using
			SaveLoad.LoadDataFromFile (GameManager.instance.GetSaveSlot ());

			Debug.Log ("most recent level? " + SaveLoad.GetMostRecentLevelName ());

			//update the master level list with this save game's level data
			GameManager.masterlevelslist = GameManager.instance.GetPlayerData ().levellist;
		
			//update the player
			GameManager.instance.SetPlayer (SaveLoad.CreatePlayer (GameManager.instance.GetPlayerData ()));
			//for every item, add the script to the player gameobject so it can be used
			//jk,this gets done elsewhere
			//foreach (Item it in GameManager.instance.GetPlayer().GetInventory().GetItemsList().Values) {
			//	GameManager.instance.GetPlayer ().AddItemScriptToPlayer (it);
			//}
			//assign slots to equipped items
			/*if (GameManager.instance.GetPlayer ().GetEquippedItem (1) != null) {
				ItemMenu.PutItemInSlot (GameManager.instance.GetPlayer ().GetEquippedItem (1), 1);
			}
			if (GameManager.instance.GetPlayer ().GetEquippedItem (2) != null) {
				ItemMenu.PutItemInSlot (GameManager.instance.GetPlayer ().GetEquippedItem (2), 2);
			}
			//update the current level to the most recently unlocked
			GameManager.instance.SetCurrentLevel (SaveLoad.GetMostRecentLevelName ());
			GameManager.instance.SetLevelGraph (GameManager.instance.GetPlayerData ().levels);
			foreach (LevelNode lnode in GameManager.instance.GetLevelGraph().NodesList()) {
				Debug.Log ("node: " + lnode.GetName () + lnode.position + lnode.HasNeighbor("Down"));
				foreach (SerializableKeyValue<LevelNode, MapPath> neigh in lnode.GetNeighbors()) {
					Debug.Log ("with neighbor: " + neigh.key.GetName());
				}
			}*/
			sceneToLoad = "worldmap";
				
		} else {
			//is a new game, make sure first level is most recent and save immediately
			Debug.Log("IS a NEW GAME!!!");

			//set up initial stages
			AddLevelsToMasterList();
			//unlock home village and temple at beginning
			GameManager.masterlevelslist.Find (x => x.name == "home").SetUnlocked (true);
			GameManager.masterlevelslist.Find (x => x.name == "village1").SetUnlocked (true);
			GameManager.masterlevelslist.Find (x => x.name == "temple1").SetUnlocked (true);

			GameManager.instance.SetMostRecentLevel("home");
			LoadLevelGraph ();
			SaveLoad.WriteData (GameManager.instance.GetSaveSlot ());
			//start home scene right away, not worldmap on first loadup
			sceneToLoad = "home";
		}

			//populate the world map graph while the scene is loading asynchronously
			//LoadLevelGraph ();
		yield return null;
		/*switch (which) {
		case "File1":
			Debug.Log ("Loaded Graph for file1");
			yield return async1;
			break;
		case "File2":
			Debug.Log ("Loaded Graph for file2");
			yield return async2;
			break;
		default:
			yield return null;
			break;
		}*/

	}

	public void LoadGameInfo(string which){
		if (!SaveLoad.IsNewGame (GameManager.instance.GetSaveSlot ())) {
			Debug.Log ("not a new game, loading file data");
			// put the data from the file into the save slot we are using
			SaveLoad.LoadDataFromFile (GameManager.instance.GetSaveSlot ());

			Debug.Log ("most recent level? " + SaveLoad.GetMostRecentLevelName ());

			//update the master level list with this save game's level data
			GameManager.masterlevelslist = GameManager.instance.GetPlayerData ().levellist;

			//update the player
			GameManager.instance.SetPlayer (SaveLoad.CreatePlayer (GameManager.instance.GetPlayerData ()));
			//for every item, add the script to the player gameobject so it can be used
			//foreach (Item it in GameManager.instance.GetPlayer().GetInventory().GetItemsList().Values) {
			//	GameManager.instance.GetPlayer ().AddItemScriptToPlayer (it);
			//}
			//add items to player, including item script components, and assign slots to equipped items
			SetUpItems();
			//update the current level to the most recently unlocked
			GameManager.instance.SetCurrentLevel (SaveLoad.GetMostRecentLevelName ());
			GameManager.instance.SetLevelGraph (GameManager.instance.GetPlayerData ().levels);
			/*foreach (LevelNode lnode in GameManager.instance.GetLevelGraph().NodesList()) {
				Debug.Log ("node: " + lnode.GetName () + lnode.position + lnode.HasNeighbor("Down"));
				foreach (SerializableKeyValue<LevelNode, MapPath> neigh in lnode.GetNeighbors()) {
					Debug.Log ("with neighbor: " + neigh.key.GetName());
				}
			}*/
			sceneToLoad = "worldmap";

		} else {
			//is a new game, make sure first level is most recent and save immediately
			Debug.Log("IS a NEW GAME!!!");

			//set up initial stages
			AddLevelsToMasterList();
			//only one unlocked should be village1 (for now)
			/*GameManager.masterlevelslist.Find (x => x.name == "home").SetUnlocked (true);
			GameManager.masterlevelslist.Find (x => x.name == "village1").SetUnlocked (true);
			GameManager.masterlevelslist.Find (x => x.name == "temple1").SetUnlocked (true);

			GameManager.instance.SetMostRecentLevel("home");
			*/
			//LoadLevelGraph ();
			SaveLoad.WriteData (GameManager.instance.GetSaveSlot ());
			//start home scene right away, not worldmap on first loadup
			sceneToLoad = "home";
		}
	}

	public void SetUpItems(){
		/*Player player = GameManager.instance.GetPlayer ();
		if (player.HasItems ()) {
			//add items? maybe not
			foreach (Item item in player.GetInventory().GetItemsList()) {
				//player.AddItem (item, item.GetScriptName ());
				item.Init();
			}

			//put equipped items back into their slots
			if (player.GetEquippedItem (1) != null) {
				Debug.Log ("setting item 1 in slot!");
				Debug.Log ("icon: " + player.GetEquippedItem (1).GetIcon ());
				ItemMenu.PutItemInSlot (player.GetEquippedItem (1), 1);
			}
			if (player.GetEquippedItem (2) != null) {
				Debug.Log ("Setting item2 in slot");
				ItemMenu.PutItemInSlot (player.GetEquippedItem (2), 2);

			}
		}*/
	}

	public void AddLevelsToMasterList(){
		foreach (XmlNode level in levelsDoc.DocumentElement.ChildNodes) {
			string levelName = level.ChildNodes [0].InnerText;
			Debug.Log ("level name added: " + levelName);
			GameManager.masterlevelslist.Add (new Level (levelName));
			GameManager.masterlevelslist.Find (x => x.name == levelName).SetUnlocked (false);
			foreach (XmlNode neighborLevel in level.ChildNodes[1].ChildNodes) { 
				GameManager.masterlevelslist.Find (x => x.name == levelName).AddNeighbor (neighborLevel.InnerText);
			}
		}
	}

	//Loads up the graph of levels the world map uses
	//This should reload the graph from scratch, no artifacts leftover from any potentially previous loads
	//from other save datas
	public void LoadLevelGraph() {
		/*
		//GameManager.instance.SetLevelGraph(SaveLoad.savedgames[GameManager.instance.GetSaveSlot()].levels);

		GameObject[] mapSceneObjects = SceneManager.GetSceneByName ("worldmap").GetRootGameObjects ();
		GameObject levelsObj = new GameObject();
		string neighborDirection;
		Transform newNodeTransform, neighborTransform;
		LevelNode nodeToAdd, neighborToAdd;
		for (int i = 0; i < mapSceneObjects.Length; i++) {
			if (mapSceneObjects [i].name == "levels")
				levelsObj = mapSceneObjects [i];
		}

		//for each level that is unlocked
		foreach (Level lvl in GameManager.masterlevelslist) {

			//add nodes for each unlocked level and also for the neighbors of these unlocked levels
			if (lvl.IsUnlocked ()) {
				Debug.Log ("Adding Unlocked Level from save game: " + lvl.name);
				//first create the node we'd like to add
				nodeToAdd = new LevelNode (lvl, lvl.GetNeighbors());
				newNodeTransform = levelsObj.transform.FindChild(nodeToAdd.GetName ());
				nodeToAdd.SetPosition (newNodeTransform.position);
				//Vector3 newNodePos = lvl.MapPosition();
				//GameManager.instance.GetLevelGraph().AddNode (nodeToAdd);

				//then add nodes for each of its neighbors
				foreach (string neighborName in lvl.GetNeighbors()) {
					neighborTransform = levelsObj.transform.FindChild (neighborName);
					//Vector3 neighborPos  = GameManager.masterlevelslist.Find(x=> x.name == neighborName).MapPosition();
					MapPath pathToNeighbor = nodeToAdd.CreatePath (neighborName);
					//if the levelnode has already been created, just use it
					if (GameManager.instance.GetLevelGraph ().NodesList ().Exists (x => x.GetName () == neighborName)) {
						neighborToAdd = GameManager.instance.GetLevelGraph ().NodesList ().Find (x => x.GetName () == neighborName);
					} else {
						//otherwise create a new levelnode
						neighborToAdd = new LevelNode (GameManager.masterlevelslist.Find (x => x.name == neighborName));
					}
					neighborToAdd.SetPosition (neighborTransform.position);
					nodeToAdd.AddNeighbor (neighborToAdd, pathToNeighbor);
					//Debug.Log (nodeToAdd.GetName() + "'s neighborrrs: " + nodeToAdd.GetNeighbors().Count);

				}
				//add the node to the game's level graph
				GameManager.instance.GetLevelGraph ().AddNode (nodeToAdd);
				Debug.Log (nodeToAdd.GetName() + "'s neighborrrs: " + GameManager.instance.GetLevelGraph ().FindNodeByName (nodeToAdd.GetName()).GetNeighbors().Count);

				//and then its neighbors
				foreach (SerializableKeyValue<LevelNode, MapPath> neighbor in nodeToAdd.GetNeighbors()) {
					GameManager.instance.GetLevelGraph().AddNode(neighbor.key);
				}

			}
		}
		Debug.Log ("worldmap info loaded!!!!!!");
		Debug.Log ("a thinggygygygy: " + GameManager.instance.GetLevelGraph ().FindNodeByName ("home").GetNeighbors().Count);
		*/
	}

	public void Unload(){
		//stop whatever loading coroutine WAS running
		StopCoroutine (LoadGameAsync (whichFile));
		//increment which load we are on, since a new one will need to be started
		Debug.Log("canceling load op. incrementing index for new loadop.");
		whichLoad += 1;
		SceneManager.UnloadScene ("worldmap");
	}

	public void ActivateScene(){
		/*switch (whichFile) {
		case "File1":
			async1.allowSceneActivation = true;
			break;
		case "File2":
			async2.allowSceneActivation = true;
			break;
		}*/
		gameLoads[whichLoad].allowSceneActivation = true;
		SceneManager.LoadScene("worldmap");

	}



	//instantiate an object which shows the player's info for this saved game
	public void ShowSaveInfo(){
		/*
		infoObj = Instantiate (infoPrefab) as GameObject;
		infoObj.transform.SetParent (GameObject.Find("Canvas").transform);
		//change size of info object so it takes up 1/3 of screen width and 1/4 screen height
		infoObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width*.9f, Screen.height/2);
		//move it according to how big it is
		//should be 40 px from bottom and left screen edges
		float movex =  (Screen.width/3);
		float movey =  (Screen.height/4);
		infoObj.GetComponent<RectTransform> ().anchoredPosition =  new Vector2 (Screen.width*.05f, 25);

		//if save data exists, populate the ui object with it, if not do nothing; the prefab is good
		if (SaveLoad.FileExists(saveSlot)) {
			PlayerData pdata = SaveLoad.GetDataFromFile(saveSlot);

			//show a healthbar that represents player's saved health
			RectTransform healthbar = infoObj.transform.GetChild (0).gameObject.GetComponentInChildren<Image> ().rectTransform;
			healthbar.offsetMax = new Vector2(pdata.health, healthbar.offsetMax.y);
			Debug.Log ("bout to show items");
			//show icons for each item attained
			Transform itemsView = infoObj.transform.GetChild (1);
			if (pdata.items != null) {
				int itemnum = 0;
				foreach (SerializableKeyValue<string, Item> item in pdata.items) {
					//make sure icon is showable
					item.value.Init ();
					Debug.Log ("Showing item: " + item.value.GetName () + " in save info");
					GameObject itemIcon = (GameObject)Instantiate (itemIconPrefab, itemsView.position + new Vector3(itemnum*30,0f,0f), itemsView.rotation); 
					itemIcon.transform.SetParent(itemsView);
					Debug.Log ("has an icon? " + item.value.GetIcon ());
					itemIcon.gameObject.GetComponent<Image>().sprite = item.value.GetIcon ();
					itemnum++;
				}
			}

		}
		*/
	}

	public void HideSaveInfo(){
		if(infoObj != null)Destroy (infoObj);
	}
}
