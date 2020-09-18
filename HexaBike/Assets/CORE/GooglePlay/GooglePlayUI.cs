using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayUI : MonoBehaviour {

	public GooglePlayConfig config;
	public static GooglePlayUI instance;
	private string errorConnectMessage;
	private string successConnectMessage;

	//Delegates.
	public delegate void OnConnect();
	//Events.
	public static event OnConnect OnConnectEvent;

	void Start () {
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
		string ln=System.Environment.NewLine;
		errorConnectMessage = "Error connecting to Google Play Services." + ln + "Try again later.";
		instance = this;
	}

	public void showMessage(string p_message){ 
		GPSMessageWindow mWindow = (GPSMessageWindow)GameObject.Instantiate(config.messageWindow);
		mWindow.message.text=p_message;
	}

	public bool isConnected(){ return Social.localUser.authenticated; }

	public void connect(){
		if (!Social.localUser.authenticated){
			GameObject window = (GameObject)GameObject.Instantiate(config.autoConnectWindow);
			Social.localUser.Authenticate((bool success)=>{
				if(success){ Destroy(window); OnConnectEvent(); }
				else{ Destroy(window); this.showMessage(errorConnectMessage); }
			});
		}
	}

	public void ShowAchievments(){
		if (Social.localUser.authenticated){ Social.ShowAchievementsUI(); }
		else{ this.connect(); }
	}

	public void showScores(){
		if (Social.localUser.authenticated){ Social.ShowLeaderboardUI(); }
		else{ this.connect(); }
	}

	public long loadScore(string scoreId){
		long score = 0;
		PlayGamesPlatform.Instance.LoadScores( scoreId, LeaderboardStart.PlayerCentered, 100, 
		                                      LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime,
			(data) => {
			score = data.PlayerScore.value;
		});
		return score;
	}

	public void showScores(string scoreId){
		if (Social.localUser.authenticated){ PlayGamesPlatform.Instance.ShowLeaderboardUI(scoreId); }
		else{ this.connect(); }
	}

	public void unlockAchievement(string achId){
		if (Social.localUser.authenticated){
			Social.ReportProgress(achId, 100.0f, (bool success) => { });
		}
	}

	public void uploadScore(string scoreID, long score){
		if (Social.localUser.authenticated){
			Social.ReportScore(score, scoreID, (bool success) => { });
		}
	}
}
