using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Floor:MonoBehaviour{
	public int id;
	public bool isDeadFloor;
	public bool hasPlayer;
	public bool isEndPoint{
		get{
			int _count = 0;
			for(int i = 0; i < neightbours.Length; i++){
				if(neightbours[i] != null)_count++;
			}
			return _count == 1;
		}
	}

	public SpriteRenderer spriteRenderer;

	public Floor[] neightbours =  new Floor[4];

	public bool HasNeighbours(){
		for(int i = 0; i < neightbours.Length; i++){
			if(neightbours[i] != null) return true;
		}
		return false;
	}

	void Update(){
		if(hasPlayer){
			spriteRenderer.color = new Color32(192,57,43,255);
		}else{
			spriteRenderer.color = new Color32(236,240,241,255);
		}
	}

	void Initialization(){
		isDeadFloor = false;
		neightbours = new Floor[4];
	}

	public void closeNeighbour(){
		for(int i = 0; i < 4; i++){
			if(neightbours[i] != null){
				neightbours[i].neightbours[(i + 2)%4]= null;
				neightbours[i] = null;
			}
		}
	}

	public void clearNeighbour(){
		for(int i = 0; id < neightbours.Length; i++){
			if(neightbours[i] != null){
				neightbours[i].removeNeightbourById(id);
				neightbours[i] = null;				
			}
		}
	}

	public Floor removeNeightbourById(int id){
		for(int i = 0; i < neightbours.Length; i++){
			if(neightbours[i] != null && neightbours[i].id == id){
				Floor temp = neightbours[i];
				neightbours[i] = null;
				return temp;
			}
		}
		return null;
	}

	public Floor removeNeightbour(Direction direction){
		Floor temp = neightbours[(int)direction];
		neightbours[(int)direction] = null;
		return temp;
	}

	public Floor removeNeightbour(int index){
		Floor temp = neightbours[index];
		neightbours[index] = null;
		return temp;
	}

	public Direction addNeightbour(Floor neightbour){
		int _index = getNeighbourSlot(neightbour);
		switch(_index){
			case 0:
				return Direction.Up;		
			case 1:
				return Direction.Right;
			case 2:
				return Direction.Down;
			case 3:
				return Direction.Left;
		}
		return Direction.Up;
	}

	public int getNeighbourSlot(Floor neightbour){
		bool _canPlace = false;
		int _index = 0;
		while(!_canPlace){
			_index = Random.Range(0, 4);
			if(neightbours[_index]==null) _canPlace = true;
		}
		
		return _index;
	}
}
