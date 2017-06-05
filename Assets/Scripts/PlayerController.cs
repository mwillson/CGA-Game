using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Reflection;

public class PlayerController : MonoBehaviour {

	// Variables for walking/running
	public float runSpeed = 10f;
	public float walkSpeed = 5f;
	public float currentSpeed = 0f;
	public float previousSpeed = 0f;
	bool facingRight = true;
	// The associated animator
	Animator anim;
	//state info for animations
	int runState = Animator.StringToHash ("Base.run");
	int runshootState = Animator.StringToHash ("Base.runandshoot");
	int walkState = Animator.StringToHash ("Base.walk");
	int walkshootState = Animator.StringToHash ("Base.walkandshoot");
	int idleState = Animator.StringToHash ("Base.idle");
	int idleshootState = Animator.StringToHash ("Base.idleshoot");
	int dashState = Animator.StringToHash ("Base.dash");
	int jumpState = Animator.StringToHash ("Base.jumpandfall");
	int pushState = Animator.StringToHash ("Base.hero_push");
	// Jumping/falling stuff
	public float jumpForce = 700f;
	//are we jumping from a dashing state?
	bool jumpingfromdash;
	public float wallPush;
	public bool grounded = false;
	public bool touchingWall = false;
	public Transform groundCheck;
	public Transform wallCheck;
	float groundRadius = 0.2f;
	float wallTouchRadius = 0.2f;
	public LayerMask whatIsGround;
	public LayerMask whatIsWall;

	//shooting
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float nextFire;

	//player stats/ui stuff
	public Player player;
	public GameObject voltDisplay;

	public PlayerMoveState moveState;

	public bool noJump;

	// Use this for initialization
	void Start () {
		player = GameManager.instance.GetPlayer ();
		if (SceneManager.GetActiveScene ().name != "enemytest") {
			//DontDestroyOnLoad (gameObject);
		}
		moveState = PlayerMoveState.Idle;
		noJump = false;
	}

	void OnLevelWasLoaded(){
		player = GameManager.instance.GetPlayer ();
	}

	void Awake() {
		anim = GetComponent<Animator> ();
		jumpingfromdash = false;
	}
		

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "shot") {
			//player.AddHealth (-1*other.gameObject.GetComponent<ShotMover>().dmg);
			Destroy (other.gameObject);
			GameObject.Find ("Health").GetComponent<HealthBarScript> ().UpdateHealth (player.getHealth ());

			//StartCoroutine (PlayerHitScript.ExecuteHurtAnimation (GetComponent<BoxCollider2D>()));
		}
	}

	void FixedUpdate () {

		// Are we on the ground?
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);


		//Are we on a wall?
		touchingWall = Physics2D.OverlapCircle (wallCheck.position, wallTouchRadius, whatIsWall);
		anim.SetBool ("Ground", grounded);
		anim.SetBool ("Walled", touchingWall);

		// vertical speed
		anim.SetFloat ("vSpeed", GetComponent<Rigidbody2D> ().velocity.y);

		// Are we moving?
		float move = Input.GetAxis ("Horizontal");

		if (move == 0) {
			moveState = PlayerMoveState.Idle;
		}

		AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo (0);
		//AnimatorTransitionInfo currentTransition = anim.GetAnimatorTransitionInfo (0);
		anim.SetFloat ("Speed", Mathf.Abs (move));
		//change the current speed if we need to
		if (currentState.nameHash == dashState) {
			//currentSpeed = GetComponent<DashScript>().dashSpeed;


			/// CHANGE TO TOGGLE ACTIVATE?
		} else if (currentState.nameHash == runState || currentState.nameHash == runshootState) {
			//if we are running
			//change physics speed appropriately
			currentSpeed = runSpeed;


		} else if (currentState.nameHash == walkState || currentState.nameHash == walkshootState) {
			//if we are walking
			currentSpeed = walkSpeed;
		} else if (currentState.nameHash == idleState || currentState.nameHash == idleshootState) {
			//if we are idle
			currentSpeed = 0f;
		}else if (currentState.nameHash == jumpState) {
			if ((anim.GetBool ("Dashing") == true) || jumpingfromdash) {
				currentSpeed = 10f;
				//we need to keep track of whether we came out of a dashing state to a jumping state
				jumpingfromdash = true;
			} else if(move != 0) {
				currentSpeed = 8;
			}else {
				currentSpeed = 0;
			}
		}

		if(moveState == PlayerMoveState.Push && currentState.nameHash != pushState){
			anim.SetBool ("Pushing", true);
			currentSpeed = 1f;
		}

		if (jumpingfromdash && currentState.nameHash != jumpState) {
			jumpingfromdash = false;
		}

		//for if we are idle(not 'inputting' movement), but want to dash
		if(facingRight && move == 0){
			//rigidbody2D.velocity = new Vector2 (1.0f * currentSpeed, rigidbody2D.velocity.y);
			move = 1.0f;
		}else if(!facingRight && move == 0){
			//rigidbody2D.velocity = new Vector2 (-1.0f * currentSpeed, rigidbody2D.velocity.y);
			move = -1.0f;
		}
		//just use the current speed(shouldn't change if we are 'in the air')
		GetComponent<Rigidbody2D>().velocity = new Vector2 (move * currentSpeed, GetComponent<Rigidbody2D>().velocity.y);
	
		//flip sprite when turning around
		if (move > 0 && !facingRight) {
			Flip ();
		} else if (move < 0 && facingRight) {
			Flip ();
		}

	}

	// Update is called once per frame
	//GOAL: Have update resolve jumping and the two currently equipped items

	void Update() {
	


		anim.SetBool("Running", Input.GetButton("Speed"));
			
		//if you're on the ground, and space is hit, jump
		if (grounded && Input.GetButtonDown ("Jump_Confirm") && !noJump) {
			anim.SetBool("Ground", false);
			Jump (false);
		}
			

		//if your not grounded, but on a wall, and hit space, do a wall jump
		//TO DO : move this functionality to an item!
		//this shouldn't get called if the above jump call was done
		else if (!grounded && touchingWall && Input.GetButtonDown ("Jump_Confirm") && !noJump) {
			
			anim.SetBool("Ground", false);
			Jump (true);
		}


		//NEED TO Dynamically get type of each item's monobehavior script
		//use that type to cast and call attemptresolve on it


		//resolve use of item in slot1
		//Item1 currently mapped to 'f' key
		/*
		if (Input.GetButton ("Item1")) {
			//Debug.Log ("has item in slot 1? " + player.HasItemInSlot (1));
		}
		if ( !(Input.GetButton ("ItemSelect")) && Input.GetButton ("Item1") && player.HasItemInSlot(1)) {
			//Debug.Log (player.GetEquippedItem (1).GetScriptName () );
			//Debug.Log ("thingyyyyy: " + player.GetEquippedItem (1).GetScriptName ());
			Type itemobjecttype = GetComponent (player.GetEquippedItem (1).GetScriptName ()).GetType ();
			MethodInfo method = itemobjecttype.GetMethod ("AttemptResolve");

			//player.GetEquippedItem(1).DisplayStats();
			//player.GetEquippedItem(1).AttemptResolve();
			method.Invoke(GetComponent(player.GetEquippedItem(1).GetScriptName()), new object[0]);
		}
		//resolve use of item in slot2
		if (!(Input.GetButton ("ItemSelect")) && Input.GetButton ("Item2") && player.HasItemInSlot(2)) {
			Type itemobjecttype = GetComponent(player.GetEquippedItem (2).GetScriptName ()).GetType();
			MethodInfo method = itemobjecttype.GetMethod ("AttemptResolve");

			player.GetEquippedItem(2).DisplayStats();
			method.Invoke(GetComponent(player.GetEquippedItem(2).GetScriptName()), new object[0]);
		}*/
	
		/*if (Input.GetKeyDown (KeyCode.M)) {
			GameManager.playerpos = transform.position;
			statScreenGO = (GameObject)Instantiate (statScreenPrefab, transform.position, transform.rotation);
			//SceneManager.LoadScene ("stats_screen");
		}*/
	}

	void OnLevelWasLoaded(int levelnum){
		transform.position = GameManager.playerpos;
	}

	//do a jump
	//special stuff if on a wall
	void Jump(bool walled){
		if (walled && !grounded) {
			if(facingRight) {
				wallPush = -2500f;
			}else {
				wallPush = 2500f;
			}
			GetComponent<Rigidbody2D>().AddForce(new Vector2(wallPush, 0f));
			GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, 0.0f);
			Debug.Log ("wall jump!");
		}
		GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		/*Quaternion shotRotation = shotSpawn.localRotation;
		if (theScale.x < 0) {
			shotSpawn.localRotation.Set(0, 180, 0, shotSpawn.localRotation.w);
		} else {
			shotSpawn.localRotation.y = 0;
		}*/
		shotSpawn.Rotate (new Vector3 (0, 180, 0));
	}

	public bool FacingRight(){
		return facingRight;
	}

	public void DisableAnimator(){
		anim.enabled = false;
	}
		
	public void EnableAnimator(){
		anim.enabled = true;
	}


}

public enum PlayerMoveState {
	Dash,
	Idle,
	Walk,
	Run,
	Jump,
	Push
}