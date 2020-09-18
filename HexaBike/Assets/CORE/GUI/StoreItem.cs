using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace mountsix.ui{

	public class StoreItem : MonoBehaviour {

		public Text title;
		public Text price;
		public Text description;
		public Text amountText;
		public Image amountBar;
		public Image icon;
		public Button button;

		public void setUI(PowerUps powerUps, PowerUp pwp){
			this.title.text = pwp.getTitle();
			this.description.text = pwp.getDescription();
			this.icon.sprite = pwp.icon;
			if (pwp.type.Equals (UpgradeType.UPGRADABLE)) {
				int items=PlayerPrefs.GetInt(pwp.pwpID);
				if(items<pwp.maxUpgrade){
					this.price.text = (pwp.price*(items+1)).ToString();
					this.amountBar.fillAmount = this.getValue((float)items,pwp.maxUpgrade);
					this.amountText.enabled=false;
				}else{
					this.amountBar.fillAmount = this.getValue(items,pwp.maxUpgrade);
					this.amountText.enabled=false;
					this.price.enabled=false; this.button.interactable=false;
				}
			} else {
				this.price.text = pwp.price.ToString();
				this.amountText.enabled=true;
				this.amountBar.transform.parent.gameObject.SetActive(false);
				this.amountText.text = PlayerPrefs.GetInt(pwp.pwpID).ToString();
			}
		}

		private float getValue(float currentValue, float maxValue){ return (currentValue / maxValue); }
	}

}