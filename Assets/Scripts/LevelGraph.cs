using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelGraph {

	//a list of the nodes representing world map areas
	private List<LevelNode> nodelist;
	//number of exits found
	private int numExits;


	public LevelGraph(){
		nodelist = new List<LevelNode> ();
		numExits = 0;
	}

	//Find a Level Node via its name
	//By convention, gameobjects in the world map scene are named the same as
	//their corresponding nodes in the graph, and should be named the same as their corresponding Level objects 
	public LevelNode FindNodeByName(string nodename) {
		foreach (LevelNode item in nodelist) {
			if(item.GetName() == nodename){
				//Debug.Log ("Found the Node! IT has position: " + item.GetPosition());
				return item;
			}
		}
		Debug.Log ("Didn't find the node!!! " + nodename);
		//if nothing found, return null
		return default(LevelNode);
	}

	//add node if it is not already in the list
	public void AddNode( LevelNode newNode){
		//if a node with this name doesn't exist in the list already, add it
		if (!nodelist.Exists (x => x.GetName ().Equals (newNode.GetName ()))) {
			Debug.Log ("adding a new Node to levelgraph! " + newNode.GetName ());
			nodelist.Add (newNode);
		} 
		//but if it does exist already, replace it bc it might be different in some other way
		else {
			Debug.Log ("replacing an existing node in the levelgraph!");
			nodelist.Remove (nodelist.Find (x => x.GetName ().Equals (newNode.GetName ())));
			nodelist.Add (newNode);
		}

		//Debug.Log ("same node? " + (nodelist.Find (x => x.GetName ().Equals (newNode.GetName ())) == newNode) );
		//Debug.Log ("node position immediately afetr adding: " + nodelist.Find (x => x.GetName ().Equals (newNode.GetName ())).position);
	}

	public int NumberOfNodes(){
		return nodelist.Count;
	}

	public void AddNeighborToNode(LevelNode node, LevelNode neighbor){
		//node.AddNeighbor (neighbor);
	}

	public List<LevelNode> NodesList(){
		return nodelist;
	}
}
