using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mountsix.characters{

	public class BPlayer : MonoBehaviour {
		public mountsix.player.BikeConfig bikeconfig;
		public GameObject bike;
		public CharacterModel characterModel;
		public BikeModel bikeModel;
		[HideInInspector]public bool GROUNDED;
		[HideInInspector]public bool FALLEN;
		//PHISYCS
		private Vector3 movement;
		private float AXIS_X;
		[HideInInspector]public float ROTATE_SPEED;
		[HideInInspector]public float MOVE_SPEED;
		//MOBILE
		private bool MOBILE;
		//POWERUPS
		public enum PlayerState{NORMAL=0,BOOST=1,REVIVE=2};
		[HideInInspector] public PlayerState state;
		//RECORD
		[HideInInspector] public List<RTransform> frames;
		private float timer;
		
		void Start () {
			this.state = PlayerState.NORMAL;
			this.movement = Vector3.zero; 
			this.MOBILE=SystemInfo.deviceType==DeviceType.Handheld;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			this.GROUNDED = true;
			this.ROTATE_SPEED = PlayerPrefs.GetFloat(GameData.SENSITIVITY);
			this.MOVE_SPEED = this.bikeconfig.physics.BIKE_SPEED;
			//RECORD.
			this.frames = new List<RTransform>(); this.frames.Add(new RTransform(transform));
			this.playAudio (this.bikeconfig.sounds.moveAudio, true);
			this.bikeconfig.physics.GROUND_LAYER_VALUE = LayerMask.NameToLayer(this.bikeconfig.physics.GROUND_LAYER_NAME);
		}
		
//		void Update () {
//			//Raycast?
//			if(!this.FALLEN && this.GROUNDED || this.state.Equals(PlayerState.BOOST)){
//				this.readInput();
//				float rotationZ = (this.AXIS_X * this.ROTATE_SPEED * Time.deltaTime);
//				transform.Rotate(new Vector3(0,0,rotationZ));
//				this.checkGround();
//				this.movement.z=this.MOVE_SPEED;
//				transform.Translate( this.movement * Time.deltaTime);
//				this.animate(); this.record();
//
//			}else{
//				this.movement=Vector3.zero;
//				if(!this.state.Equals(PlayerState.BOOST)){ this.fall(); }
//			}
//		}

		void Update() {
			//Raycast?
			if(!this.FALLEN && this.GROUNDED || this.state.Equals(PlayerState.BOOST)){
				this.readInput(); this.checkGround();
			}
		}

		void FixedUpdate() {
			//Raycast?
			if(!this.FALLEN && this.GROUNDED || this.state.Equals(PlayerState.BOOST)){
				float rotationZ = (this.AXIS_X * this.ROTATE_SPEED * Time.deltaTime);
				transform.Rotate(new Vector3(0,0,rotationZ));
				this.movement.z=this.MOVE_SPEED;
				transform.Translate( this.movement * Time.deltaTime);
				this.animate(); this.record();
				
			}else{
				this.movement=Vector3.zero;
				if(!this.state.Equals(PlayerState.BOOST)){ this.fall(); }
			}
		}

		private void checkGround(){
			RaycastHit hit;
			Ray ray = new Ray(transform.position, (transform.rotation * Vector3.down));
			this.GROUNDED = Physics.Raycast (ray, out hit, this.bikeconfig.physics.GROUND_DISTANCE);
		}

		private void readInput(){
			if (this.MOBILE) { this.AXIS_X=Input.acceleration.x; } 
			else { this.AXIS_X=Input.GetAxis("Horizontal"); }
		}

		private void animate(){
			if (this.AXIS_X == 0) {
				this.characterModel.animator.SetInteger (this.bikeconfig.animations.STATE, this.bikeconfig.animations.ST_IDLE);
			} 
			else {
				int dir= (this.AXIS_X<0) ? this.bikeconfig.animations.ST_LEFT : this.bikeconfig.animations.ST_RIGHT;
				this.characterModel.animator.SetInteger(this.bikeconfig.animations.STATE, dir);
			}
		}
		
		public void fall(){
			if (this.FALLEN == false) {
				this.GROUNDED=false; this.FALLEN=true;
				this.playAudio (this.bikeconfig.sounds.hittedAudio, false);
				//TRY REVIVE.
				if (mountsix.ui.Game.getGame().canRevive()) {
					mountsix.ui.Game.getGame().activateRevive(this.bikeconfig.physics.FALL_SPEED);
				}
				else{
					//CHARACTER.
					this.characterModel.activateRagdoll(this.bikeconfig.physics.FALL_SPEED);
					//BIKE.
					this.bikeModel.activateRagdoll(this.bikeconfig.physics.FALL_SPEED);
					mountsix.ui.Game.getGame().showGameOver();
				}
			}
		}

		private void record(){
			this.timer+=Time.deltaTime;
			if(this.timer>=this.bikeconfig.RECORD_INTERVAL){
				RaycastHit hit;
				Vector3 forwardPosClose=bike.transform.position; Vector3 forwardPosFar=bike.transform.position;
				forwardPosClose.z+=2; forwardPosFar.z+=10;
				Ray rayClose = new Ray(forwardPosClose, (transform.rotation * Vector3.down));
				Ray rayFar = new Ray(forwardPosFar, (transform.rotation * Vector3.down));
				bool CLOSE_GROUND = Physics.Raycast (rayClose, out hit, this.bikeconfig.physics.GROUND_DISTANCE,this.bikeconfig.physics.GROUND_LAYER_VALUE);
				bool FAR_GROUND = Physics.Raycast (rayFar, out hit, this.bikeconfig.physics.GROUND_DISTANCE,this.bikeconfig.physics.GROUND_LAYER_VALUE);
				if(!this.FALLEN && CLOSE_GROUND && FAR_GROUND){
					if(this.frames.Count>3){ this.frames.Clear(); }
					this.frames.Add(new RTransform(transform)); this.timer=0;
				}
			}
		}
	
		public void playAudio(AudioClip clip,bool loop){
			this.GetComponent<AudioSource>().Stop(); this.GetComponent<AudioSource>().clip = clip; this.GetComponent<AudioSource>().loop=loop; this.GetComponent<AudioSource>().Play();
		}
	}	
}
