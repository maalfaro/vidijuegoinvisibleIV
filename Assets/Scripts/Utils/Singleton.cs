using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Generic Singleton
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

  /**********************************************************************************************/
  /*  Members                                                                                   */
  /**********************************************************************************************/
  #region Members

  private static T instance;

  public static T Instance
  {
    get
    {
      if (instance == null)
      {
        instance = (T)FindObjectOfType(typeof(T));
        DontDestroyOnLoad(instance);
      }
      return instance;
    }
  }

  #endregion

  /**********************************************************************************************/
  /*  MonoBehaviour Methods                                                                     */
  /**********************************************************************************************/
  #region MonoBehaviour Methods

  protected virtual void Awake()
  {
    if (instance == null)
    {
      InitInstance();
    }
  }

  #endregion

  /**********************************************************************************************/
  /*  Private Methods                                                                           */
  /**********************************************************************************************/
  #region Private methods

  protected virtual void InitInstance()
  {
    instance = this as T;
    DontDestroyOnLoad(instance);
  }

  #endregion


}
