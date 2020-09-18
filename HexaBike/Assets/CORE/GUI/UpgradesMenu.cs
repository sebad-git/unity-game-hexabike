using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace mountsix.ui{

	public class UpgradesMenu : MonoBehaviour {

		public Text cashLabel;
		public PowerUps powerUpList;
		private StoreItem[] items;
		private int coins;

		void Start () { 
			this.coins = PlayerPrefs.GetInt(GameData.TOTAL_SCORE);
			this.cashLabel.text = this.coins.ToString();
			this.items=(StoreItem[])gameObject.GetComponentsInChildren<StoreItem>();
			this.createPowerUps();
		}

		public void buyItem(string iId){
			PowerUp pwp = this.powerUpList.getPowerUp(iId);
			if(pwp!=null && this.coins >= pwp.price){
				int amount=(PlayerPrefs.GetInt(pwp.pwpID)+1);
				PlayerPrefs.SetInt(pwp.pwpID,amount);
				this.coins-=pwp.price;
				PlayerPrefs.SetInt(GameData.TOTAL_SCORE,this.coins);
				this.cashLabel.text = this.coins.ToString();
				if(amount>=pwp.maxUpgrade){
					GameData.unlockAchievment(AchievementsHandler.achievements.findById(pwp.upgradeAchievment));
				}
				this.updatePowerUps();
			}
		}
		
		private void createPowerUps(){
			List<PowerUp> powerUps = this.powerUpList.powerUps;
			for(int i=0; i<items.Length; i++){ 
				PowerUp pwp = powerUps[i]; this.items[i].setUI(this.powerUpList,pwp);
				this.items[i].button.interactable=(this.coins<pwp.price)? false : true;
				this.items[i].button.onClick.AddListener( delegate{ this.buyItem(pwp.pwpID); } );
			}
		}

		private void updatePowerUps(){
			List<PowerUp> powerUps = this.powerUpList.powerUps;
			for(int i=0; i<items.Length; i++){
				PowerUp pwp = powerUps[i];
				this.items[i].button.interactable=(this.coins<pwp.price)? false : true;
				this.items[i].setUI(this.powerUpList,pwp);
			}
		}
		
		public void close(){
			GameObject.FindGameObjectWithTag (Tags.MENU_SOUND_TAG).GetComponent<AudioSource> ().Play ();
			Destroy (gameObject); 
		}
	}
}
