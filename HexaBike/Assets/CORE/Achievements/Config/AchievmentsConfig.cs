using UnityEngine;
using System.Collections.Generic;

public class AchievmentsConfig : ScriptableObject {

	public List<Achievment> achievmentsList;

	public Achievment findById(string achId){
		System.Predicate<Achievment> achFinder = (Achievment ach) => { return ach.aid.Trim().ToLower() == achId.Trim().ToLower(); };
		Achievment fAchv=this.achievmentsList.Find(achFinder);
		return fAchv;
	}


}
