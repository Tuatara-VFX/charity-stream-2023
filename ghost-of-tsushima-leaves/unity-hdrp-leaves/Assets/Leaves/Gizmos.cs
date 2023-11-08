using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmos : MonoBehaviour
{
	public Color Color;
	public Color WireColor;

	private void OnDrawGizmos()
	{
		if (enabled)
		{
			UnityEngine.Gizmos.color = Color;
			UnityEngine.Gizmos.DrawSphere(transform.position, Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f);
			UnityEngine.Gizmos.color = WireColor;
			UnityEngine.Gizmos.DrawWireSphere(transform.position, Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z) * 0.5f);
		}
	}
}
