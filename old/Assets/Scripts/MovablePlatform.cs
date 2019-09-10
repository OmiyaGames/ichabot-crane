using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ConfigurableJoint))]
public class MovablePlatform : MonoBehaviour
{
	public enum Position
	{
		Left,
		Right,
		Middle
	}
	
	public Position startingPosition = Position.Middle;
	
	void Start ()
	{
		if(startingPosition != Position.Middle)
		{
			Vector3 start = transform.localPosition;
			ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
			start.x = joint.linearLimit.limit;
			if(startingPosition == Position.Left)
			{
				start.x *= -1f;
			}
			transform.localPosition = start;
		}
	}
}
