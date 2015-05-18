using UnityEngine;
using System.Collections;

public class Button : State {

	public Color colorOn;
	public Color colorOff;

	private int contactCount = 0;

	void Awake()
	{
		Init();
	}

	override public void Init()
	{
		TurnOff();
	}

	void OnTriggerEnter( Collider other )
	{
		if( other.tag == "Player" || other.tag == "Snake Cell" )
		{
			if( contactCount == 0 )
			{
				TurnOn();
			}
			contactCount++;
		}
	}
	void OnTriggerExit( Collider other )
	{
		contactCount--;
		if( contactCount == 0 && ( other.tag == "Player" || other.tag == "Snake Cell"  ) )
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		// GetComponent<Renderer>().material.color = colorOn;
		StateTrue();

	}
	public void TurnOff()
	{
		// GetComponent<Renderer>().material.color = colorOff;
		StateFalse();
	}
}
