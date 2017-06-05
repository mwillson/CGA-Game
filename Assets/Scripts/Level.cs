using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
//pretty much all we care about saving (i.e. 'Serializing') right now is whether this level is unlocked
//this will change as the game is iterated on
public class Level {

	//the map of the level
	//will be slowly revealed as level is explored
	[System.NonSerialized]
	public GameObject map;
	//list of explored Rooms
	//don't serialize for now, not used currently
	//[System.NonSerialized]
	//List<Room> explored;
	//[System.NonSerialized]
	//public Room spawnroom;
	public List<string> neighbors;
	public string name;
	//public List<NPC> npcs;
	[System.NonSerialized]
	public AudioClip defaultmusic;
	//whether this level has been unlocked by the player
	public bool unlocked;
	//could be a actual level, or a menu, or something else, idk
	//may be useful to have categories for 'story/dialogue' levels and 'action' levels, and 'other'
	public string levelType;
	//this is a keyvalue pair with our data of interest bc it seems that serializing monobehaviours is no good
	//and a cutscene script must be a monobehaviour
	public List<KeyValuePair<string,bool>> cutscenes;

	//constructor
	//n - the name of the unity scene associated with this level object
	public Level(string n) {
		name = n;
		//npcs = new List<NPC> ();
		neighbors = new List<string> ();
		defaultmusic = (AudioClip)Resources.Load ("Audio/" + name);
		cutscenes = new List<KeyValuePair<string,bool>> ();
	}

	/*public NPC GetNPC(string npcname){
		return npcs.Find (x => x.name == npcname);
	}

	public void AddNPC(NPC newnpc){
		npcs.Add (newnpc);
	}
	*/
	public void SetUnlocked (bool unlock){
		unlocked = unlock;
	}

	public bool IsUnlocked(){
		return unlocked;
	}

	public List<string> GetNeighbors(){
		return neighbors;
	}

	public void AddNeighbor(string newNeighbor){
		neighbors.Add (newNeighbor);
	}

	/*public bool HasNPC(string npcName){
		return npcs.Exists (x => x.name == npcName);
	}*/

	public List<KeyValuePair<string,bool>> GetCutscenes(){
		return cutscenes;
	}
}
