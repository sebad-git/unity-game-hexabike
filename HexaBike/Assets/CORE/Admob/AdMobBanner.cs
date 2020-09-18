using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdMobBanner {

	public static string AD_UNIT_ID="ca-app-pub-2790886384819689/7407951854";

	public static BannerView banner;

	private static void createBanner(){
		AdMobBanner.banner = new BannerView(AD_UNIT_ID, AdSize.Banner, AdPosition.Top);
		AdRequest request = new AdRequest.Builder().Build();
		AdMobBanner.banner.LoadAd(request);
		AdMobBanner.banner.Hide();
	}

	public static void showBanner(bool show){
		if(banner==null){ AdMobBanner.createBanner(); }
		if (show==true) { banner.Show(); } else{ banner.Hide(); } 
	}

}
