using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace mountsix.ui{

	public class AchievmentItem : MonoBehaviour {

		public Text nameLabel;
		public Text descriptionLabel;
		public Image iconImage;

		public void setUI(Achievment achv, Sprite p_icon){
			this.nameLabel.text = achv.getTitle();
			this.descriptionLabel.text = achv.getDescription();
			this.iconImage.sprite = p_icon;
		}
	}

}
