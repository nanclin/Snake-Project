using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	void OnDrawGizmos()
	{
		DrawArrow.ForGizmo( transform.position, transform.forward * 4, Color.blue, 1 );
	}
}
