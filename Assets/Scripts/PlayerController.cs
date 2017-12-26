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
	public Text _textStep;
	public Text _textBestStep;
	public Text _textScore;
	public Text _textBestScore;
	[Header("Panel UI")]
	public GameObject panelRetry;
	public GameObject panelHighScore;
	public GameObject panelNewHighScore;
	public GameObject exitPanel;
	[Header("Default")]
	public bool isDead = false;
	public float delayTime = 30.0f;
	public float _inputDelay = 0.09f;
	[Header("Audio")]
	public AudioSource _audioSource;
	public AudioClip[] _clips;
	private bool _onDelay = false;
	private int _position = 0;
	private int _step = 0;
	private int _bestStep = 0;
	private float _delayTime;

	void Start(){
		_delayTime = delayTime;
		if(PlayerPrefs.HasKey("BestScore")){
			_bestStep = PlayerPrefs.GetInt("BestScore", 0);
		}
		_textBestStep.text = "<color=#D7DCDEFF>BEST</color> <b>"+ _bestStep.ToString()+"</b>  ";
	}
	void Update(){

		if(isDead && !panelRetry.activeSelf){
			_audioSource.Stop();
			_audioSource.clip = _clips[1];
			_audioSource.Play();
			panelRetry.SetActive(true);
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

		}else if(!isDead && panelRetry.activeSelf){
			panelNewHighScore.SetActive(false);
			panelRetry.SetActive(false);
		}

		if(_delayTime > 0){
			if(isDead) _delayTime = 0;
			float delta = (Time.deltaTime * _step)/1.34f;
			if(delta>_inputDelay) 
				delta = _inputDelay - 0.0005f;
			_delayTime = _delayTime - delta;
			_slider.value = _delayTime/delayTime;
		}

		if(_delayTime <= 0){
			isDead = true;
		}

		if(!_onDelay && !isDead){
			
			if(Input.GetButton("Up")){
				//Debug.Log("UP" + floorManager.floors[_position].id);
				DelayAndCalcPos();
				floorManager.floors[_position].hasPlayer = false;
				//Debug.Log("UP" + floorManager.floors[_position].id);
				if(floorManager.floors[_position].neightbours[0] == null)
					isDead = true;
				else
				{
					floorManager.floors[_position].neightbours[0].hasPlayer = true;
					//Debug.Log("MAsuk INI");
				}
					
				
				floorManager.move(_inputDelay);
				_position = floorManager.getPlayerPosition();
				Debug.Log("UP pos" + _position);
				

			}else if(Input.GetButton("Right")){
				//Debug.Log("RIGHT");
				DelayAndCalcPos();
				floorManager.floors[_position].hasPlayer = false;
				if(floorManager.floors[_position].neightbours[1] == null)
					isDead = true;
				else	
					floorManager.floors[_position].neightbours[1].hasPlayer = true;
				
				floorManager.move(_inputDelay);
				_position = floorManager.getPlayerPosition();

			}else if(Input.GetButton("Down")){
				//Debug.Log("DOWN");
				DelayAndCalcPos();
				floorManager.floors[_position].hasPlayer = false;
				if(floorManager.floors[_position].neightbours[2] == null)
					isDead = true;
				else
					floorManager.floors[_position].neightbours[2].hasPlayer = true;
				
				floorManager.move(_inputDelay);
				_position = floorManager.getPlayerPosition();

			}else if(Input.GetButton("Left")){
				//Debug.Log("LEFT");
				DelayAndCalcPos();
				floorManager.floors[_position].hasPlayer = false;
					if(floorManager.floors[_position].neightbours[3] == null)
					isDead = true;
				else
					floorManager.floors[_position].neightbours[3].hasPlayer = true;
				
				floorManager.move(_inputDelay);
				_position = floorManager.getPlayerPosition();

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

	private void move(int _index){
		if(!_onDelay && !isDead){
			DelayAndCalcPos();
			floorManager.floors[_position].hasPlayer = false;
			if(floorManager.floors[_position].neightbours[_index] == null)
				isDead = true;
			else
				floorManager.floors[_position].neightbours[_index].hasPlayer = true;
			
			floorManager.move(_inputDelay);
			_position = floorManager.getPlayerPosition();
		}
	}

	public void moveUp(){
		move(0);
	}
	public void moveRight(){
		move(1);
	}
	public void moveDown(){
		move(2);
	}
	public void moveLeft(){
		move(3);
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
		_position = Random.Range(0,3);
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
}
