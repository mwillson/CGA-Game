using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadPlayerScript : MonoBehaviour {

	public string itemToUse;

	public void Awake(){
		//GameManager.instance = GetComponent<GameManager> ();
		

	}

	// Use this for initialization
	void Start () {
		string sceneName = SceneManager.GetActiveScene ().name;

		//if the scene is not in the master levels list, add it to we can then set current lvl
		if (!(GameManager.masterlevelslist.Exists (x => x.name == sceneName))) {
			GameManager.masterlevelslist.Add (new Level (sceneName));
		}

		GameManager.instance.SetCurrentLevel (sceneName);


		//create the player with the test item equipped
		Player player = new Player();

		/*
		//itemToUse = "disruptor";
		Item item, item2;
		switch (itemToUse) {
		case "disruptor":
			item = new DisruptorMemento ();
			break;
		case "dashboots":
			item = new DashMemento ();
			break;
		default:
			item = new BasicGunMemento ();
			Debug.Log ("Item not Found! Can't give item to player! Defaulting to Basic Gun");
			break;
		}
		item2 = new BasicGunMemento ();

		GameManager.instance.SetPlayer (player);
		player.AddItem (item, item.GetScriptName ());
		player.AddItem (item2, item2.GetScriptName ());
		*/

		//ensure player controller is using THIS up-to-date player
		//UGH
		GameObject.Find ("Player").GetComponent<PlayerController> ().player = GameManager.instance.GetPlayer ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
