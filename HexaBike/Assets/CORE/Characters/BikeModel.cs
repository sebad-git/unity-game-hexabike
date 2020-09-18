using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BikeModel : Recordable {

	public Renderer render;
	public Material rewindMaterial;
	public GameObject bikeEffectPrefab;
	private List<RTransform> frames;
	private GameObject bikeEffect;
	private Material defaultMaterial;

	void Start () { this.frames = new List<RTransform>(); this.defaultMaterial=this.render.material; }

	public void activateRagdoll(Vector3 velocity){
		this.render.enabled = false;
		if(this.bikeEffect==null){
			this.bikeEffect = (GameObject)Instantiate (this.bikeEffectPrefab, transform.position, 
			                                           this.bikeEffectPrefab.transform.rotation);
			this.bikeEffect.GetComponent<Rigidbody>().velocity=velocity;
			if(this.state.Equals(RecordState.RECORDING)){
				Renderer cRender=this.bikeEffect.GetComponent<Renderer>();
				cRender.material=this.rewindMaterial;
			}
		}
	}

	public override void record(){
		this.frames.Add(new RTransform(bikeEffect.transform));
	}

	public override void rewind(){
		if (this.frames.Count > 0) {
			RTransform rt= this.frames[(this.frames.Count-1)];
			rt.setFrame(bikeEffect.transform);
			this.frames.RemoveAt((this.frames.Count-1));
		}
		else {
			Destroy(this.bikeEffect); this.bikeEffect=null;
			this.render.material = this.rewindMaterial; 
			this.render.enabled = true; this.state=RecordState.NORMAL;
		}
	}

	public void resetMaterial(){
		this.render.material = this.defaultMaterial;
	}
}
