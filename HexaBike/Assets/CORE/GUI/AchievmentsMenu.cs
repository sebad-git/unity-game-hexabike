using UnityEngine;
using System.Collections;

namespace mountsix.ui{

	public class AchievmentsMenu : MonoBehaviour {
		
		public AchievmentsConfig achievments;
		public Sprite lockedIcon;
		public Sprite unLockedIcon;
		private AchievmentItem[] items;
		
		void Start () {
			this.items=(AchievmentItem[])gameObject.GetComponentsInChildren<AchievmentItem>();
			this.loadAchievments();
		}
		
		private void loadAchievments(){
			System.Collections.Generic.List<Achievment> achs = this.achievments.achievmentsList;
			for(int i=0; i<items.Length; i++){
				Achievment ach = achs[i]; Sprite icon=this.lockedIcon;
				icon=GameData.achievmentUnlocked(ach.aid) ? this.unLockedIcon : this.lockedIcon;
				this.items[i].setUI(ach,icon);
			}
		}
		
		public void close(){ 
			GameObject.FindGameObjectWithTag (Tags.MENU_SOUND_TAG).GetComponent<AudioSource> ().Play ();
			Destroy (gameObject); 
		}
		
	}
}
