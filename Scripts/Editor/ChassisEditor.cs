using UnityEngine;
using UnityEditor;

namespace Speedcar
{
	[CustomEditor(typeof(Chassis))]
	public class ChassisEditor : Editor
	{
		public Chassis Chassis => (Chassis)target;

		/*
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			Suspension.FrontForwardFriction = new WheelFrictionCurve
			{
				asymptoteSlip = EditorGUILayout.DelayedFloatField("Asy", Suspension.FrontForwardFriction.asymptoteSlip),
			};
		}
		*/
	}
}
