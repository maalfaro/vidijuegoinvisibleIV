using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Timer{

	private uint idTimer;
	private float _waitTime = float.MaxValue;
	private float _currentTime;

	public bool CannotBePaused {get; private set;}
	public bool isActive=false;
	public Action<Timer> callback;
	public object Context {get; set;}

	public void SetContext(object aContext){
		Context=aContext;
	}

	public void SetTimerActionCallback(Action<Timer> newCallback){
		callback= newCallback;
	}

	public float GetCurrentTime(){
		return _currentTime;
	}

	public uint GetTimerID(){
		return idTimer;
	}

	public float GetWaittingTimer(){
		return _waitTime;
	}

	public void Destroy(){
		SetContext(null);
		SetTimerActionCallback(null);
	}

	#region Funcionalidad del timer

	public void StartTimer(float waitValue, bool _isActive, uint timerID, bool cannotBePaused = false) {
		_waitTime=waitValue;
		_currentTime=0.0f;
		isActive=_isActive;
		idTimer = timerID;
		CannotBePaused =cannotBePaused;
	}

	public void StopTimer(Action<Timer> callback=null){
		isActive=false;
		CannotBePaused=false;
		_waitTime = float.MaxValue;
		_currentTime=0.0f;
		idTimer=0;
		SetContext(null);
		SetTimerActionCallback(null);
		callback?.Invoke(this);
	}

	public void PauseTimer(bool isPaused){
		if(!CannotBePaused){
			if(isActive && isPaused){
				isActive=false;
			}else if(!isActive && !isPaused && _waitTime!=float.MaxValue){
				isActive=true;
			}
		}
	}
	public void UpdateTimer(){
		if(isActive){
			_currentTime+=Time.deltaTime;
			
			//Si hemos superado el tiempo de espera
			//lanzamos la callback y paramos el timer
			if(_currentTime>=_waitTime){
				StopTimer(callback);
			}
		}
	}
#endregion
}
