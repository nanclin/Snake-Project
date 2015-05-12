using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Initializer : MonoBehaviour {

	public static List<Initializer> initializersList = new List<Initializer>();

// UNITY MEHTODS ///////////////////////////////////////////////////////////////
	void OnEnable()
	{
		initializersList.Add( this );
	}

	void OnDisable()
	{
		initializersList.Remove( this );
	}

	void Awake()
	{
		Init();
	}
//////////////////////////////////////////////////////////// EO UNITY MEHTODS //

// INITIALIZATION ///////////////////////////////////////////////////////////////
	public virtual void Init()
	{
		Debug.LogWarning( "Override me!", this );
	}
//////////////////////////////////////////////////////////// EO INITIALIZATION //

// STATIC METHODS ///////////////////////////////////////////////////////////////
	public static void ResetAll()
	{
		foreach( Initializer initializer in initializersList )
			initializer.Init();
	}
//////////////////////////////////////////////////////////// EO STATIC METHODS //
}
