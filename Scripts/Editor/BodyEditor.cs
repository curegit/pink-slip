using UnityEngine;
using UnityEditor;

namespace Speedcar
{
	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(Body))]
	public class BodyEditor : Editor
	{
		/// <summary>
		/// 
		/// </summary>
		private Body Body => (Body)target;

		
		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI()
		{
			// 
			Body.InertiaTensor = EditorGUILayout.Vector3Field("Inertia", Body.InertiaTensor);
			Body.InertiaTensorRotation = EditorGUILayout.Vector3Field("Inertia Rotation", Body.InertiaTensorRotation);
			// 
			Body.CenterOfMass = EditorGUILayout.ObjectField("", Body.CenterOfMass, typeof(Transform), true) as Transform;
			// 
			Body.GravityMultiplier = EditorGUILayout.FloatField("", Body.GravityMultiplier);
			// 
			Body.DownforceCoefficient = EditorGUILayout.FloatField("Downforce Coefficient", Body.DownforceCoefficient);
			Body.MaxDownforce = EditorGUILayout.FloatField("Max Downforce", Body.MaxDownforce);

			
			
			


		//Target.testFloat = EditorGUILayout.FloatField(new GUIContent("Test Float", "Here is a tooltip"), Target.testFloat);
		}
		

	}
}
