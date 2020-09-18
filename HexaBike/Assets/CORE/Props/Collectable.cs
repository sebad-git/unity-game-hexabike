using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

	public mountsix.props.collectable.CollectableConfig config;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == Tags.BIKE_TAG) {
			GameObject.FindGameObjectWithTag(Tags.COIN_SOUND_TAG).GetComponent<AudioSource>().Play();
			if(this.config.collectEffect!=null){ Instantiate(config.collectEffect,transform.position,Quaternion.identity); }
			mountsix.ui.Game.getGame().addCoin();
			Destroy(gameObject);
		}
	}
}
