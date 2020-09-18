using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using mountsix.characters;

namespace mountsix.ui{

	public class Game : MonoBehaviour {
		public enum Framerate{MOBILE=30, PC=60};
		public Framerate framerate=Framerate.MOBILE;
		public PowerUps powerUps;
		public Text coinsLabel;
		public Text kmsLabel;
		public GameOverMenu gameOver;
		public float gameOverWait=1.5f;
		public PauseMenu pauseMenu;
		//POWERUPS
		public Image reviveImage; private int reviveAmount;
		//PRIVATE
		private static Game game;
		private int coins; private float kms;
		private BPlayer player;
		private float originalPos;
		//AUDIO
		private AudioSource coinSound;
		private AudioSource music;

		void Awake(){
			Application.targetFrameRate = (int)this.framerate; SoundManager.updateSound();
		}

		void Start () {
			AdMobBanner.showBanner(true);
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Time.timeScale = 1; this.coins = 0; this.kms = 0;
			if(this.coinsLabel!=null){ this.coinsLabel.text = this.coins.ToString(); }
			if(this.kmsLabel!=null){ this.kmsLabel.text = this.kms.ToString(); }
			this.player=GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).GetComponent<BPlayer>();
			this.originalPos=this.player.transform.position.z;
			this.coinSound = GameObject.FindGameObjectWithTag (Tags.COIN_SOUND_TAG).GetComponent<AudioSource>();
			this.music = GameObject.FindGameObjectWithTag (Tags.MUSIC_TAG).GetComponent<AudioSource>();
			this.reviveAmount=this.powerUps.getPowerUpAmount(UpgradeCategory.CATEGORY_3);
			this.reviveImage.enabled = (this.reviveAmount>0)? true : false;
		}

		public void showGameOver() {
			this.music.volume = 0;
			GameData.updateCoins(this.coins); GameData.updateMiles(this.kms);
			this.StartCoroutine (this.createGameOver() );
		}

		private IEnumerator createGameOver(){
			yield return new WaitForSeconds(this.gameOverWait);
			Instantiate(this.gameOver);
		}

		public void pause() { Instantiate(this.pauseMenu); }

		public static Game getGame(){
			if (game == null) { game = GameObject.FindObjectOfType<Game>(); }
			return game;
		}

		public void addCoin(){ this.coinSound.Play(); this.coins += 1; }

		void FixedUpdate () {
			if(this.coinsLabel!=null){ this.coinsLabel.text = this.coins.ToString(); }
			this.kms = (float)System.Math.Round((this.player.transform.position.z-this.originalPos),1);
			if(this.kmsLabel!=null){ this.kmsLabel.text = this.kms.ToString()+" m"; }
		}

// ****************************** POWERUPS ******************************

		public void activateTurbo(){
			if ( this.player.GROUNDED) {
				PowerUp power=this.powerUps.getPowerUpByCategory(UpgradeCategory.CATEGORY_1);
				float timer=power.createEffect();
				this.StartCoroutine( this.turbo(timer) );
			}
		}

		public void activateCoinMagnet(){
			if (this.player.GROUNDED) {
				PowerUp power = this.powerUps.getPowerUpByCategory(UpgradeCategory.CATEGORY_2);
				power.createEffect();
			}
		}

		public void activateRevive(Vector3 fallVelocity){
			PowerUp power=this.powerUps.getPowerUpByCategory(UpgradeCategory.CATEGORY_3);
			this.reviveAmount --;
			PlayerPrefs.SetInt(power.pwpID, this.reviveAmount);
			GameData.unlockAchievment(AchievementsHandler.achievements.findById(power.upgradeAchievment));
			this.reviveImage.enabled = (this.reviveAmount>0)? true : false;
			this.StartCoroutine ( this.revive(fallVelocity,power) );
		}

		private IEnumerator turbo(float timer){
			this.player.state= BPlayer.PlayerState.BOOST;
			float curSpeed = this.player.bikeconfig.physics.BIKE_SPEED;
			this.player.MOVE_SPEED = this.player.bikeconfig.physics.BOOST_SPEED;
			yield return new WaitForSeconds(timer);
			while (this.player.MOVE_SPEED>curSpeed) {
				this.player.MOVE_SPEED-=this.player.bikeconfig.physics.BOOST_BREAK_SPEED; yield return null;
			}
			this.player.MOVE_SPEED = curSpeed; 
			this.player.state = BPlayer.PlayerState.NORMAL;
		}

		public bool canRevive(){
			return (this.powerUps.getPowerUpAmount(UpgradeCategory.CATEGORY_3) > 0);
		}

		public IEnumerator revive(Vector3 fallSpeed,PowerUp power){
			this.player.FALLEN=true; this.player.GROUNDED=false;
			//FALL.
			float timer = mountsix.ui.Game.getGame ().gameOverWait;
			this.player.bikeModel.state = Recordable.RecordState.RECORDING;
			this.player.characterModel.state = Recordable.RecordState.RECORDING;
			this.player.bikeModel.activateRagdoll(fallSpeed); this.player.characterModel.activateRagdoll(fallSpeed);
			//RECORD.
			while (timer>0) {
				this.player.characterModel.record(); this.player.bikeModel.record(); timer-=Recordable.FRAMERATE; 
				yield return null;
			}
			mountsix.util.DestroyAfter effect = power.createEffect(this.player.characterModel.transform).GetComponent<mountsix.util.DestroyAfter>();
			//REWIND.
			this.player.bikeModel.state = Recordable.RecordState.REWIND;
			this.player.characterModel.state = Recordable.RecordState.REWIND;
			while (this.player.bikeModel.isReWinding() || this.player.characterModel.isReWinding() ) {
				this.player.bikeModel.rewind(); this.player.characterModel.rewind(); yield return null; 
			}
			//REWIND END.
			this.player.characterModel.animator.SetInteger (this.player.bikeconfig.animations.STATE, this.player.bikeconfig.animations.ST_IDLE);
			yield return new WaitForSeconds(0.5f);
			//BACKTRACK.
			RTransform lastSave = this.player.frames[0];
			float distance = 100f;
			Transform pTransform = this.player.transform;
			while(distance > 0.3f){
				pTransform.position=Vector3.Lerp(pTransform.position,lastSave.rPosition,(1.2f*Time.deltaTime));
				pTransform.rotation=Quaternion.Lerp(pTransform.rotation,lastSave.rRotation,(1.2f*Time.deltaTime));
				distance=Mathf.Abs(Vector3.Distance(pTransform.position,lastSave.rPosition));
				yield return null;
			}
			lastSave.setFrame(pTransform);
			effect.destroyNow();
			this.player.bikeModel.resetMaterial(); this.player.characterModel.resetMaterial();
			//READY.
			this.player.playAudio(this.player.bikeconfig.sounds.moveAudio, true);
			this.player.GROUNDED = true; this.player.FALLEN=false;
		}

	}
}
