using UnityEngine;
using UnityEngine.UI;

public class GPSMessageWindow : MonoBehaviour {

	public Text message;

	public void close(){ Destroy (gameObject); }

}
