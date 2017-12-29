using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	[Header("Main")]
	public GameObject _camera;
	public FloorManager floorManager;
	[Header("Time Countdown")]
	public Slider _slider;
	[Header("Text UI")]
	public Text _textFeedback;
	public Text _textStep;
	public Text _textBestStep;
	public Text _textScore;
	public Text _textBestScore;
	[Header("Panel UI")]
	public GameObject panelRetry;
	public GameObject panelFeedback;
	public GameObject panelHighScore;
	public GameObject panelNewHighScore;
	public GameObject exitPanel;
	[Header("Default")]
	public int deadCounter = 0;
	public bool isDead = false;
	public float delayTime = 60.0f;
	public float delayAdd = 0.05f;
	public float _inputDelay = 0.09f;
	[Header("Audio")]
	public AudioSource _audioSource;
	public AudioClip[] _clips;
	private bool _onDelay = false;
	private int _position = 0;
	private int _lastPosition = 0;
	private int _step = 0;
	private int _bestStep = 0;
	private float _delayTime;
	private bool _deadDisplay = false;

	void Start(){
		_delayTime = delayTime;
		if(PlayerPrefs.HasKey("BestScore")){
			_bestStep = PlayerPrefs.GetInt("BestScore", 0);
		}
		_textBestStep.text = "<color=#D7DCDEFF>BEST</color> <b>"+ _bestStep.ToString()+"</b>  ";
		_deadDisplay = false;
	}
	void Update(){

		if(isDead && !_deadDisplay){
			_deadDisplay = true;
			_audioSource.Stop();
			_audioSource.clip = _clips[1];
			_audioSource.Play();
			deadCounter += 1;

			//TODO open panel feedback
			OpenPanelFeedback();

		}else if(!isDead){
			if(panelFeedback.activeSelf){
				panelFeedback.SetActive(false);
			}
			
			if(panelRetry.activeSelf){
				panelNewHighScore.SetActive(false);
				panelRetry.SetActive(false);
			}
		}

		if(_delayTime > 0){
			if(isDead) _delayTime = 0;
			_delayTime = _delayTime - (Time.deltaTime * _step)*0.74f;
			_slider.value = _delayTime/delayTime;
		}

		if(_delayTime <= 0 && !isDead){
			isDead = true;
			_textFeedback.text = "TIME'S UP !!!";
			Debug.Log("TimesUp");
		}

		if(!_onDelay && !isDead){
			
			if(Input.GetButton("Up")){
				Move(0);

			}else if(Input.GetButton("Right")){
				Move(1);

			}else if(Input.GetButton("Down")){
				Move(2);

			}else if(Input.GetButton("Left")){
				Move(3);

			}
		}
	}

	void FixedUpdate(){
		if(_position < 4 && _position > -1){
			Transform _transPlayer = floorManager.floors[_position].transform;
			_camera.transform.position = Vector3.Lerp(_camera.transform.position,new Vector3(_transPlayer.position.x, _transPlayer.position.y-1 ,_camera.transform.position.z), 0.1f);
		}			
	}

	void DelayAndCalcPos(){
		_position = floorManager.getPlayerPosition();
		_lastPosition = _position;
		StartCoroutine(InputDelay(_inputDelay));
		if(!isDead){
			_audioSource.clip = _clips[0];
			_audioSource.Play();
			_step++;
			_textStep.text = _step.ToString();
		}
		_delayTime = delayTime;
	}

	IEnumerator InputDelay(float delay){
		_onDelay = true;
		yield return new WaitForSeconds(delay);
		_onDelay = false;
	}

	private void Move(int _index){
		if(!_onDelay && !isDead){
			DelayAndCalcPos();
			floorManager.floors[_position].hasPlayer = false;
			if(floorManager.floors[_position].neightbours[_index] == null){
				isDead = true;
				_textFeedback.text = "WRONG STEP !!!";
				Debug.Log("Wrong step");
			}else
				floorManager.floors[_position].neightbours[_index].hasPlayer = true;
			
			floorManager.Move(_inputDelay);
			_position = floorManager.getPlayerPosition();
			floorManager.FixFloor();
		}
	}

	public void moveUp(){
		Move(0);
	}
	public void moveRight(){
		Move(1);
	}
	public void moveDown(){
		Move(2);
	}
	public void moveLeft(){
		Move(3);
	}

	public void Restart(){
		foreach(Floor a in floorManager.floors){
			a.hasPlayer = false;
		}
		_audioSource.Stop();
		_audioSource.clip = _clips[2];
		_audioSource.Play();
		_step = 0;
		_textStep.text = _step.ToString();
		_delayTime = delayTime;
		_position = 0;
		bool _successed = false;
		while(!_successed){
			_position = Random.Range(0,3);
			if(floorManager.floors[_position].isEndPoint) _successed = true;
		}
		floorManager.floors[_position].hasPlayer = true;
		isDead = false;
		Start();
	}

	public void OpenExit(){
		exitPanel.SetActive(true);
	}

	public void CloseExit(){
		exitPanel.SetActive(false);
	}

	public void Exit(){
		_audioSource.Stop();
		_audioSource.clip = _clips[3];
		_audioSource.Play();
		Application.Quit();
	}
	IEnumerator TransitionToRetry(float delay){
		yield return new WaitForSeconds(delay);
		if(panelFeedback.activeSelf)
			panelFeedback.SetActive(false);
		
		OpenPanelRetry();
		yield return null;
	}

	public void OpenPanelFeedback(){
		if(!panelFeedback.activeSelf){
			panelFeedback.SetActive(true);
		}

		StartCoroutine(TransitionToRetry(1.0f));
	}
	
	public void OpenPanelRetry(){
		if(!panelRetry.activeSelf){	
			panelRetry.SetActive(true);
		}
		_textScore.text = _step.ToString();

		if(!PlayerPrefs.HasKey("BestScore")){
			PlayerPrefs.SetInt("BestScore", _step);
			_textBestScore.text = _step.ToString();
		}else{
			_bestStep = PlayerPrefs.GetInt("BestScore",0);
			if(_step > _bestStep){
				panelNewHighScore.SetActive(true);
				PlayerPrefs.SetInt("BestScore", _step);
			}
			_textBestScore.text = _bestStep.ToString();
		}
	}
}