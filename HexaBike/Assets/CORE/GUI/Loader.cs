using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace mountsix.game{

	public class Loader : MonoBehaviour {

		public float waitTime=2.5f;

		void Start () {
			AdMobBanner.showBanner(true);
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			this.StartCoroutine (this.load ());
		}

		public IEnumerator load(){
			yield return new WaitForSeconds(waitTime);
			Application.LoadLevel ( (Application.loadedLevel + 1) );
		}
	}

}
