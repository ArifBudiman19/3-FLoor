using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class MyAds : MonoBehaviour {
	void Start(){
		Advertisement.Initialize("1651092");
	}

	public void ShowAds(){
		if(Advertisement.IsReady()){
			Advertisement.Show("video", new ShowOptions(){resultCallback = HandleAdsResult});
		}
	}

	public void HandleAdsResult(ShowResult result){
		switch(result){
			case ShowResult.Failed:
				Debug.Log("No internet connection");
				break;
			case ShowResult.Skipped:
				Debug.Log("Good boyyyyy");
				break;
			case ShowResult.Finished:
				Debug.Log("Good boyyyyy");
				break;
		}
	}


}
