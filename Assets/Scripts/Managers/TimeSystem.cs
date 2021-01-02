using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Net;
using System.Globalization;

public class TimeSystem : Singleton<TimeSystem>
{

    #region Variables

    private List<Timer> timerList;
	private uint _timerID = 0;
	private bool gamePaused;

    #endregion

    protected override void InitInstance() {
        base.InitInstance();
		InitializeTimers(100);
		//GameController.OnPauseGame += OnPausedGame;
	}

	private void Update() {
		if (!gamePaused) UpdateTimers();
	}

	private void OnPausedGame(bool paused) {
		gamePaused = paused;
	}

	private void OnDisable() {
		//GameController.OnPauseGame -= OnPausedGame;
	}

	#region Init and destroy methods

	public void InitializeTimers(int capacity) {
		timerList = new List<Timer>();
		for (int i = 0; i < capacity; i++) {
			timerList.Add(new Timer());
		}
	}

	///<summary>Devuelve el siguiente timer que no este activo</summary>
	private Timer GetTimer() {
		for (int i = 0; i < timerList.Count; i++) {
			if (!timerList[i].isActive) {
				return timerList[i];
			}
		}

		//Como no hay timers libres añadimos un nuevo timer a la lista
		Timer timer = new Timer();
		timerList.Add(timer);
		return timer;
	}

	///<summary>Devuelve el timer cuyo ID sea igual a timerID</summary>	
	private Timer GetTimerWithID(uint timerID) {
		for (int i = 0; i < timerList.Count; i++) {
			if (timerList[i].GetTimerID().Equals(timerID)) {
				return timerList[i];
			}
		}
		throw new System.Exception("El ID no existe");
	}

	///<summary>Limpiar la lista de timers</summary>	
	public void DestroyAndCleanUp() {
		//Paramos y destruimos todos los timers
		for (int i = 0; i < timerList.Count; i++) {
			Timer timer = timerList[i];
			timer.StopTimer();
			timer.Destroy();
		}

		//Vaciamos la lista
		timerList.Clear();
		_timerID = 0;
	}

	#endregion

	#region Timer methods

	///<summary>Iniciamos un timer para que se ejecute en el tiempo indicado, 
	/// le indicamos el contexto que será la clase que queremos que guarde el timer y
	/// la acción de callback que se ejecutará al cumplir el tiempo.</summary>	
	///<param name="waitValue">Tiempo de espera del timer.</param>
	///<param name="context">Objeto que se va a guardar en el timer.</param>	
	///<param name="callback">Acción que se va a ejecutar a finalizar el tiempo de espera.</param>	
	///<returns>Devuelve el ID del timer que acabamos de iniciar.</returns>
	public uint StartTimerWithID(float waitValue, object context, Action<Timer> callback) {
		Timer timer = GetTimer();
		timer.SetContext(context);
		timer.SetTimerActionCallback(callback);

		_timerID += 1;
		timer.StartTimer(waitValue, true, _timerID);

		return _timerID;
	}


	///<summary>Paramos el timer con el id indicado y ejecutamos el callback.</summary>	
	///<param name="timerID">Id del timer.</param>
	///<param name="callback">Callback a ejecutar tras parar el timer.</param>	
	public void StopTimerWithID(uint timerID, Action<Timer> callback) {
		try {
			Timer timer = GetTimerWithID(timerID);
			timer.StopTimer(callback);
		}
		catch (System.Exception e) {
			Debug.Log(e.Message);
		}
	}

	///<summary>Pausa o reanudad según el parámetro isPaused, el timer que tenga ID igual al timer ID</summary>	
	///<param name="timerID">Id del timer que queremos pausar.</param>
	///<param name="isPaused">Indica si tiene que pausar o reanudar el timer.</param>		
	public void PauseTimerWithID(uint timerID, bool isPaused) {
		try {
			Timer timer = GetTimerWithID(timerID);
			timer.PauseTimer(isPaused);
		}
		catch (System.Exception e) {
			Debug.Log(e.Message);
		}
	}

	public void PauseAllTimers(bool isPaused) {
		for (int i = 0; i < timerList.Count; i++) {
			Timer timer = timerList[i];
			timer.PauseTimer(isPaused);
		}
	}

	public void StopAllTimers() {
		for (int i = 0; i < timerList.Count; i++) {
			Timer timer = timerList[i];
			timer.StopTimer();
		}
	}

	public void UpdateTimers() {
		for (int i = 0; i < timerList.Count; i++) {
			Timer timer = timerList[i];

			if (timer == null) continue;

			if (timer.CannotBePaused) {
				timer.UpdateTimer();
			} else {
				if (timer.isActive) {
					timer.UpdateTimer();
				}
			}
		}
	}

	#endregion



}

public class DateTime_Online
{
    public DateTime GMT;
    public DateTime Local;
    public DateTime Offline;

    public DateTime_Online(DateTime gmt, DateTime local, DateTime offline)
    {
        GMT = gmt;
        Local = local;
        Offline = offline;
    }
    public DateTime_Online(DateTime gmt, DateTime local) : this(gmt, local, DateTime.Now) { }

}
