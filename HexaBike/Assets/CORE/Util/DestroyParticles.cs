using UnityEngine;
using System.Collections;

namespace mountsix.util{

	public class DestroyParticles : MonoBehaviour {

		void Start () { Destroy(transform.root.gameObject,GetComponent<ParticleSystem>().duration); }

	}

}
