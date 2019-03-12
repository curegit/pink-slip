using UnityEngine;
using UnityEditor;

namespace Speedcar.EditorOnly
{
	/// <summary>
	/// 車体コンポーネントのエディタ拡張
	/// </summary>
	[CustomEditor(typeof(Body))]
	public class BodyEditor : SpeedcarEditor
	{
		/// <summary>
		/// 対象コンポーネント
		/// </summary>
		private Body Body => (Body)target;
		
		/// <summary>
		/// インスペクターを拡張する
		/// </summary>
		public override void OnInspectorGUI()
		{
			// リジッドボディについて
			Space();
			Header("Rigidbody");
			Body.CenterOfMass = EditorGUILayout.ObjectField("Center of Mass", Body.CenterOfMass, typeof(Transform), true) as Transform;
			Body.InertiaTensor = EditorGUILayout.Vector3Field("Moment of Inertia", Body.InertiaTensor);
			Body.InertiaTensorRotation = EditorGUILayout.Vector3Field("Inertia Rotation", Body.InertiaTensorRotation);
			Body.GravityMultiplier = EditorGUILayout.Slider("Gravity Multiplier", Body.GravityMultiplier, 0.5f, 3f);
			Body.MaxAngularSpeed = EditorGUILayout.Slider("Max Angular Speed", Body.MaxAngularSpeed, 0.5f, 6.2f);
			Body.SolverIterations = EditorGUILayout.IntSlider("Solver Iterations", Body.SolverIterations, 1, 32);
			Body.SolverVelocityIterations = EditorGUILayout.IntSlider("Solver Velocity Iterations", Body.SolverVelocityIterations, 1, 32);
			// 空力について
			Header("Aerodynamics");
			Body.LinearDrag = EditorGUILayout.Vector3Field("Linear Drag", Body.LinearDrag);
			Body.QuadraticDrag = EditorGUILayout.Vector3Field("Quadratic Drag", Body.QuadraticDrag);
			Body.DownforceCoefficient = EditorGUILayout.FloatField("Downforce Coefficient", Body.DownforceCoefficient);
			Body.DownforceShift = EditorGUILayout.FloatField("Downforce Shift", Body.DownforceShift);
			Body.MaxDownforce = EditorGUILayout.FloatField("Max Downforce", Body.MaxDownforce);
			// 衝突について
			Header("Collision");
			Body.MaxDepenetrationSpeed = EditorGUILayout.Slider("Max Depenetration Speed", Body.MaxDepenetrationSpeed, 10f, 50f);
			Body.MaxAngularVelocityDeltaOnCollision = EditorGUILayout.Slider("Max Angular Velocity Delta", Body.MaxAngularVelocityDeltaOnCollision, 0f, 10f);
		}
	}
}
