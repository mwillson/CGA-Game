using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// A LevelNode represents a transform on the worldmap, which can be the entrance for a game level
[System.Serializable]
public class LevelNode {

	//this would be the scene name
	public string nameOfLevel;
	//maybe dont need a reference to the level? name alone may be fine
	[System.NonSerialized]
	public Level theLevel;
	//each neighbor is a key value pair: a node and it's direction from this node
	//private List<SerializableKeyValue<LevelNode, MapPath>> neighbors;
	private List<string> neighborNames;
	//the position of this node in the world map, in global space coordinates
	public FileVector3 position;
	//icon for use on the world map
	[System.NonSerialized]
	private Sprite icon;

	//probs not necessary
	private bool activated;

	public LevelNode(Level lvl){
		//neighbors = new List<SerializableKeyValue<LevelNode, MapPath>> ();
		theLevel = lvl;
		Debug.Log ("level: " + lvl.name );
		neighborNames = lvl.GetNeighbors();
		nameOfLevel = lvl.name;
	}

	public LevelNode(Level lvl, List<string> neighs){
		
		//neighbors = new List<SerializableKeyValue<LevelNode, MapPath>> ();
		theLevel = lvl;
		neighborNames = neighs;
		nameOfLevel = lvl.name;
	}

	public string GetName() {
		return nameOfLevel;
	}

	/*public List<SerializableKeyValue<LevelNode, MapPath>> GetNeighbors(){
		return neighbors;
	}*/

	//a neighbor should NEVER be added without a mappath
	//otherwise this breaks this.HasPathTo(), so you'd have to change that too
	/*public void AddNeighbor(LevelNode newNeighbor, MapPath neighborPath){
		SerializableKeyValue<LevelNode, MapPath> kvpair = new SerializableKeyValue<LevelNode, MapPath>();
		kvpair.key = newNeighbor;
		Debug.Log ("adding neighbor with key: " + newNeighbor.GetName () + " and value: " + neighborPath.GetPath());
		kvpair.value = neighborPath;
		neighbors.Add (kvpair);
	}*/

	public bool IsActivated() {
		return activated;
	}

	public void SetPosition(Vector3 pos){
		position.Fill(pos);
	}

	public Vector3 GetPosition(){
		return position.V3;
	}

	public bool IsUnlocked(){
		return GameManager.masterlevelslist.Find(x => x.name == nameOfLevel).IsUnlocked();
	}

	//find the neighbor in a certain direction, if there is one.
	//this is the levelnode key where the value is a string representing the direction
	/*public LevelNode GetNeighborInDirection(string direction){
		return neighbors.Find (x => x.value.GetDirection() == direction).key;
	}

	//find the neighbor, given it's name. useful when unlocking a level and adding it to world map
	public LevelNode GetNeighborByName(string name) {
		Debug.Log ("neighbor name to find: " + name);
		//Debug.Log ("neighbors: " + neighbors [0]);
		return neighbors.Find (x => x.key.GetName () == name).key;
	}

	//Figure out direction of a neighbor node
	//this should actually map to an input direction: up down left or right
	public string GetNeighborDirection(string name){
		//needs to be based on direction of first point on path between this node and neighbor in question
		return neighbors.Find(x => x.key.GetName() == name).value.GetDirection();
	}

	//does it have a neighbor in the specified direction?
	public bool HasNeighbor(string direction){
		//Debug.Log ("which node?: " + nameOfLevel + "which way? " + direction);
		foreach (SerializableKeyValue<LevelNode, MapPath> neighb in neighbors) {
			//Debug.Log (neighb.value);
			foreach(Vector3 vect in neighb.value.GetPath()){
			//Debug.Log (vect);
			}
		}
		//Debug.Log ("result of neighbors.exists: " + neighbors.Exists (x => x.value.GetDirection () == direction));
		return neighbors.Exists (x => x.value.GetDirection() == direction);
	}
		
	//does this node have a path to the named node?
	//yes if it has a neighbor with that name, no if not
	public bool HasPathTo (string nodename) {
		return neighbors.Exists (x => x.key.nameOfLevel == nodename);
	}
	
	public MapPath CreatePath(string neighborName){
		
		List<Transform> subnodes = new List<Transform>();
		foreach (Transform subnode in GameObject.Find(nameOfLevel).transform) {
			//add the child transform if it is named according to the convention
			//the convention: its name must contain the name of the neighbor it goes to followed by an 'n' aand the number of that node on the path
			Regex regx = new Regex(@"" + neighborName + @"n\d+");
			if (regx.IsMatch(subnode.gameObject.name)) {
				Debug.Log ("for this node: " + nameOfLevel + " there is a gameobject subnode for neighbor " + neighborName + ": " + subnode.gameObject.name);
				subnodes.Add (subnode);
			}
		}

		//the array of transforms for a map path is the amount of children gameobjects this node's gameobject has which fit subnode criteria
		//plus one for this node's object
		//and one for the destination node object
		Vector3[] theArray = new Vector3[subnodes.Count + 2];
		theArray [0] = position.V3;
		int i = 1;

		//add children 'sub nodes' to the path
		//if there are no children this code should not be called at all
		foreach (Transform subnode in subnodes) {
			//add the subnode to the path
			theArray [i] = subnode.position;
			i++;
		}
		Debug.Log ("Array size is " + theArray.Length);
		Debug.Log ("Putting item in at position " + i);

		//i has been incremented one last time, giving us the last position in the array
		theArray[i] = GameObject.Find(neighborName).transform.position;
		Debug.Log ("neighbor: " + neighborName + "at position " + i +" in the theArray ");
		foreach (Vector3 pathnode in theArray) {
			Debug.Log ("Added " + pathnode + " to the map path array for " + nameOfLevel);
		}
		return new MapPath(theArray);
	}
	*/

}
