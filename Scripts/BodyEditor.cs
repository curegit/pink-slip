using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Speedcar
{
	[CustomEditor(typeof(Body))]
	public class BodyEditor : Editor
	{
		private Body Body => (Body)target;

		/*
		public override void OnInspectorGUI()
		{
			Body.CenterOfMass = EditorGUILayout.ObjectField("Center of Mass", Body.CenterOfMass, typeof(Transform), true) as Transform;

			Body.InertiaTensor = EditorGUILayout.Vector3Field("Inertia", Body.InertiaTensor);

			Body.InertiaTensorRotation = EditorGUILayout.Vector3Field("Inertia Rotation", Body.InertiaTensorRotation);

			Body.DownforceCoefficient = EditorGUILayout.FloatField("Downforce Coefficient", Body.DownforceCoefficient);

		Target.testFloat = EditorGUILayout.FloatField(new GUIContent("Test Float", "Here is a tooltip"), Target.testFloat);
		}
		*/

	}
}