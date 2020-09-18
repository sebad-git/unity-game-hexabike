using UnityEngine;
using System.Collections.Generic;

public class PowerUps : ScriptableObject {
	
	public List<PowerUp> powerUps;

	public PowerUp getPowerUp(string p_pwpID){
		System.Predicate<PowerUp> powerFinder = (PowerUp pwp) => { return pwp.pwpID == p_pwpID; };
		PowerUp fPwp=this.powerUps.Find(powerFinder);
		return fPwp;
	}

	public PowerUp getPowerUpByCategory(UpgradeCategory category){
		System.Predicate<PowerUp> powerFinder = (PowerUp pwp) => { return pwp.category.Equals(category); };
		PowerUp fPwp=this.powerUps.Find(powerFinder);
		return fPwp;
	}

	public int getPowerUpAmount(UpgradeCategory category){
		PowerUp power= getPowerUpByCategory(category);
		if (power != null) { int amount = PlayerPrefs.GetInt (power.pwpID); return amount;}
		return 0;
	}
	
}
