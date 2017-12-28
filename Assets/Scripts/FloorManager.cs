using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour {
	public GameObject[] floorsGO = new GameObject[3];
	public Floor[] floors = new Floor[3];
	private bool _moving = false;
	// array ujung;
	private int[] _endPoints = new int[2];
	// Use this for initialization
	void Start () {
		_moving = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int getPlayerPosition(){
		for(int i = 0; i < 3; i++){
			if(floors[i].hasPlayer) return i;
		}
		return -1;
	}

	public void Move(float delay){
		if(!_moving){
			StartCoroutine(MoveFloor(delay));
		}
	}

	public void Move(float delay, int id){
		if(!_moving){
			//Debug.Log("Corotine Move");
			StartCoroutine(MoveFloor(delay, id));
		}
	}

	private void Moving(){
		bool _successed = false;
		while(!_successed){
			int _index = 0;
			for(int i = 0; i < 3; i++){
				if(floors[i].isEndPoint){
					_endPoints[_index] = i;
					_index++;
				}
			}

			int _randIndex = _endPoints[Random.Range(0,2)];
			if(!floors[_randIndex].hasPlayer)
				_successed = true;
			else
				continue;

			bool _canPlace = false;
			int _randTarget = 0;
			while(!_canPlace){
				_randTarget = Random.Range(0,3);
				if(_randIndex != _randTarget) _canPlace = true;
			}

			int _slot = floors[_randTarget].getNeighbourSlot(floors[_randIndex]);
			floors[_randIndex].closeNeighbour();
			floors[_randTarget].neightbours[_slot] = floors[_randIndex];
			switch(_slot){
				case (int)Direction.Up:
					floorsGO[_randIndex].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x,floorsGO[_randTarget].transform.position.y+0.64f);
					break;
				case (int)Direction.Right:
					floorsGO[_randIndex].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x+0.64f,floorsGO[_randTarget].transform.position.y);
					break;
				case (int)Direction.Down:
					floorsGO[_randIndex].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x,floorsGO[_randTarget].transform.position.y-0.64f);
					break;
				case (int)Direction.Left:
					floorsGO[_randIndex].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x-0.64f,floorsGO[_randTarget].transform.position.y);
					break;
			}
			_slot = (_slot + 2)%4;
			floors[_randIndex].neightbours[_slot] = floors[_randTarget];
		}
	}

	private void Moving(int id){
		int _index = 0;
		for(int i = 0; i < 3; i++){
			if(floors[i].id == id){
				_index = i;
			}
		}
		//Debug.Log("move id "+_index);

		bool _canPlace = false;
		int _randTarget = 0;
		while(!_canPlace){
			_randTarget = Random.Range(0,3);
			if(_index != _randTarget) _canPlace = true;
		}

		int _slot = floors[_randTarget].getNeighbourSlot(floors[_index]);
		floors[_index].closeNeighbour();
		floors[_randTarget].neightbours[_slot] = floors[_index];
		switch(_slot){
			case (int)Direction.Up:
				floorsGO[_index].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x,floorsGO[_randTarget].transform.position.y+0.64f);
				break;
			case (int)Direction.Right:
				floorsGO[_index].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x+0.64f,floorsGO[_randTarget].transform.position.y);
				break;
			case (int)Direction.Down:
				floorsGO[_index].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x,floorsGO[_randTarget].transform.position.y-0.64f);
				break;
			case (int)Direction.Left:
				floorsGO[_index].transform.position = new Vector2(floorsGO[_randTarget].transform.position.x-0.64f,floorsGO[_randTarget].transform.position.y);
				break;
		}
		_slot = (_slot + 2)%4;
		floors[_index].neightbours[_slot] = floors[_randTarget];
	}

	public void FixFloor(){
		for(int i = 0 ; i < floors.Length; i++){
				if(!floors[i].HasNeighbours()){
					Moving(floors[i].id);
				}
			}
	}

	IEnumerator MoveFloor(float delay){
		_moving = true;
		Moving();
		yield return new WaitForSeconds(delay);
		_moving = false;
		yield return null;
	}

	IEnumerator MoveFloor(float delay, int id){
		_moving = true;
		Moving(id);
		yield return new WaitForSeconds(delay);
		_moving = false;
		yield return null;
	}
}
