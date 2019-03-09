using UnityEngine;
using UnityEditor;

namespace Speedcar
{
	[CustomEditor(typeof(Suspension))]
	public class SuspensionEditor : Editor
	{
		public Suspension Suspension => (Suspension)target;

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
