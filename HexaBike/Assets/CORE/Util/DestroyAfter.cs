using UnityEngine;
using System.Collections;

namespace mountsix.util{
	public class DestroyAfter : MonoBehaviour {

		[Range(0f,30f)]public float DESTROY_TIME=1.2f;

		void Start () { Destroy(gameObject,DESTROY_TIME); }
		public void destroyNow() { Destroy(gameObject); }

	}
}
