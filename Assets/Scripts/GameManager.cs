using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static List<Player> savedgames;
	public static GameObject pointsawardedobj;
	public static Vector3 playerpos;

	public static List<Level> masterlevelslist;
	//events to be processed, usually at the sart of a scene
	//information is passed through this queue across scenes
	public static Queue<string> levelsToUnlock;

	public static GameManager instance {
		get {
			if(_instance == null){
				_instance = GameObject.FindObjectOfType<GameManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
		set{
			_instance = value;
		}
	}

	private int level;
	private Player player;
	private int saveSlot;
	private LevelGraph openlevels;
	public PlayerData playerData;
	private LevelGraph levelGraph;

	public Level currentlevel;
	public GameObject newLevelPrefab, newLevelScene;
	public Object[] icons;

	//place to return player to if they go into an interior area?
	//will there only be 1 level deep of interiors?
	//string is scenename, Vector3 is position
	public KeyValuePair<string, Vector3> returnpoint;

	//constructor
	public GameManager() {
	}

	// Use this for initialization
	void Awake () {
		if (_instance == null) {
			Debug.Log ("gm instance null awake");
			_instance = this;
			DontDestroyOnLoad (gameObject);
			_instance.SetPlayer (new Player ());
			_instance.GetPlayer ().init ();
			//createplayerdata() needs a levelgraph, otherwise a null one will be propogated through the chain initially
			//this code is quite loopy, needs to be better!!!
			//the problem is that this gets called before GameManager.instance is fully formed!!!
			playerData = SaveLoad.CreatePlayerData (GetPlayer (), "home");

			//we are starting up the application

			saveSlot = 0;
			levelsToUnlock = new Queue<string> ();

			masterlevelslist = new List<Level> ();


			//these wont show up in world map, they should not be loaded into levels graph, so
			//they are set as "locked"
			masterlevelslist.Add (new Level ("menu1"));
			masterlevelslist.Find (x => x.name == "menu1").SetUnlocked (false);

			masterlevelslist.Add (new Level ("worldmap"));
			masterlevelslist.Find (x => x.name == "worldmap").SetUnlocked (false);

			//populate initial (three) save slots with player data
			for (int i = 0; i < 3; i++) {
				//PlayerData newData = SaveLoad.CreatePlayerData (GetPlayer());
				SaveLoad.AddLevelsData (playerData, new LevelGraph (), masterlevelslist, "home"); 
				SaveLoad.savedgames.Add (playerData);
				Debug.Log ("master list? " + masterlevelslist);
				Debug.Log ("is this a new game? " + SaveLoad.IsNewGame (i));
			}

			levelGraph = new LevelGraph ();
			icons = Resources.LoadAll ("Items/Icons/items");
		} else if (_instance != this) {
			Debug.Log ("gm instance not null awake");

			Destroy (gameObject);
		} /*else {
			_instance = this;
			//playerData = SaveLoad.CreatePlayerData (GetPlayer (), "home");
			masterlevelslist = new List<Level> ();
			//levelGraph = new LevelGraph ();
			icons = Resources.LoadAll ("Items/Icons/items");
			Debug.Log ("gm awake");
		}*/


	}
	
	// Update is called once per frame
	void Update () {
	    
	}

	void OnLevelWasLoaded(int level){
		/*Debug.Log ("loaded level, now set current level");
		//initialize player health to correct value
		string sceneName = SceneManager.GetActiveScene ().name;
		//change this if necessary to accomodate image-based representation of health
		//if (sceneName != "menu1" && sceneName != "worldmap" && sceneName != "title" && sceneName != "stats_screen") {
		//	GameObject.Find ("Health").GetComponent<Text> ().text = "Health: " + _instance.GetPlayer ().getHealth ();
		//}

		//if levels have been unlocked on world map scene load, show them being unlocked
		if ( sceneName == "worldmap") {
			//don't us mapmovescript data here bc it may not be set up yet, its Start() is intertwined with this callback
			//set player object position


			if (levelsToUnlock.Count > 0) {

				Debug.Log("most recent level: " + GetPlayerData().mostRecentLevel);
				LevelNode currentlyOn = GameManager.instance.GetLevelGraph ().FindNodeByName (GetPlayerData ().mostRecentLevel);
				GameObject.Find ("player").transform.position = new Vector3(currentlyOn.position.x, currentlyOn.position.y, -1.0f);

				Debug.Log ("levels to unlock: " + levelsToUnlock.Peek ());
				foreach (string levelname in levelsToUnlock) {
					Debug.Log ("Adding " + levelname + " to world map.");


					// DONT GET NEIGHBOR, GET NODE BY NAME, YOU MAY HAVE UNLOCKED NOT A NEIGHBOR!!!
					LevelNode nodeToAdd = instance.GetLevelGraph().FindNodeByName(levelname);


					//by this point in the game loop, the Node of the level in question has been added to our graph, but it's neighbors might not exist in it's instance,
					//so we create them and do that here
					//for this new level, instantiate its neighbor nodes and add them to the graph, but dont unlock them!
					foreach (string neighborName in GetLevel(levelname).GetNeighbors()) {
						LevelNode neighborNode;
						//if the neighbor node exists, that's the one to add, otherwise, create a new node
						if (instance.GetLevelGraph ().NodesList ().Exists (x => x.GetName () == neighborName)) {
							neighborNode = instance.GetLevelGraph ().FindNodeByName (neighborName);
						} else {
							neighborNode = new LevelNode (masterlevelslist.Find (x => x.name == neighborName));
							//when we create a levelnode thats going to be added to the graph, make sure it has its actual positional info 
							neighborNode.SetPosition (new Vector3 (GameObject.Find (neighborName).transform.position.x,
																   GameObject.Find (neighborName).transform.position.y,
																   0));
						}

						//if there is a path from the neighbor, then the path TO the neighbor is just it's reverse
						if (neighborNode.HasPathTo (levelname)) {
							//unfortunately, we have to create a copy after reversing the neighbor's original path array, and then reverse it back to its original state
							//if we can come up with a way to do this algorithm without useing Array.Reverse, that would be nice
							Vector3[] reversepath = neighborNode.GetNeighbors ().Find (x => x.key.GetName () == levelname).value.GetPath ();
							System.Array.Reverse (reversepath);

						} else {
						//otherwise we must create a path to this neighbor

						}

						levelGraph.AddNeighborToNode (nodeToAdd, neighborNode);

					}
					//now that its neighbors have been added, make sure the up-to-date node is in the graph
					levelGraph.AddNode (nodeToAdd);
					//add the newly created neighbors for the levelnode of the unlocked level to the level graph
					foreach (SerializableKeyValue<LevelNode, MapPath> neighbor in nodeToAdd.GetNeighbors()) {
						GameManager.instance.GetLevelGraph ().AddNode (neighbor.key);
					}

				
					Debug.Log ("node position? " + instance.GetLevelGraph().FindNodeByName(levelname).GetPosition ());

					//after node has been added, save the level data
					Debug.Log("Is the most recent here??? " + SaveLoad.savedgames [saveSlot].mostRecentLevel);
					SaveLoad.SaveLevelData(SaveLoad.savedgames [saveSlot].mostRecentLevel);
					SaveLoad.AssignToSaveSlot (GameManager.instance.GetPlayer (), GameManager.instance.GetSaveSlot (), SaveLoad.savedgames [saveSlot].mostRecentLevel);
					SaveLoad.WriteData (GameManager.instance.GetSaveSlot ());

					ShowLevelUnlock (levelname);
					levelsToUnlock.Dequeue ();
					if (levelsToUnlock.Count < 1)
						break;
				}
			}
		}
		
		*/
	}

	public void SetPlayer(Player p){
		player = p;
	}

	public Player GetPlayer(){
		return player;
	}

	public PlayerData GetPlayerData(){
		return SaveLoad.savedgames [saveSlot];
	}

	public void DeletePlayerData(int slot){
		SaveLoad.DeleteData (slot);
	}

	public LevelGraph GetLevelGraph(){
		return levelGraph;
	}

	public void SetLevelGraph(LevelGraph lg) {
		levelGraph = lg;
	}
		
	public void SetSaveSlot (int newSlot){
		saveSlot = newSlot;
	}

	public int GetSaveSlot() {
		return saveSlot;
	}

	public void SetMostRecentLevel(string levelName){
		SaveLoad.savedgames [saveSlot].mostRecentLevel = levelName;
	}

	public void SetCurrentLevel(string lvlname) {
		Debug.Log ("settin current lvl");
		currentlevel = GameManager.masterlevelslist.Find (x => x.name == lvlname);

		//this creates a new level based on the current scene we are in, just in case we don't find the level we are looking for
		//this usually happens when we are testing a level, not playing an actual build
		//just kidding - it happens with interior 'levels'
		if (currentlevel == null) {
			Debug.Log ("setting current level to new one bc not found in master list");
			currentlevel = new Level(SceneManager.GetActiveScene ().name);
			GameManager.masterlevelslist.Add (currentlevel);
		}
	}
		
	public Level GetCurrentLevel() {
		return currentlevel;
	}

	public Level GetLevel(string name){
		return masterlevelslist.Find (x => x.name == name);					
	}

	//execute the 'adding a level to the map' cutscene
	public void ShowLevelUnlock(string lvlname){
		//NEED TO REDO FOR MATE ANIMATOR PLUGIN

		/*
		newLevelScene = (GameObject) GameObject.Find("NewLevelScene");
		NewLevelScene sceneScript = newLevelScene.GetComponent<NewLevelScene> ();
		sceneScript.SetLevelName (lvlname);
		//sceneScript.Start ();
		sceneScript.Execute ();*/

	}

	public static Vector3 ScreenCenter(){
		return new Vector3 (Screen.width / 2, Screen.height / 2, 0);
	}


		
}
