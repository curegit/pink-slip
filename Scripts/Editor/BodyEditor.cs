using UnityEditor;

namespace Speedcar.EditorOnly
{
	/// <summary>
	/// 車体コンポーネントのエディタ拡張
	/// </summary>
	[CustomEditor(typeof(Body)), CanEditMultipleObjects]
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
			// 同期する
			serializedObject.Update();
			// リジッドボディについて
			Space();
			Header("Rigidbody");
			IncreaseIndent();
			PropertyField("centerOfMassConfig");
			if (Body.CenterOfMassConfig == CenterOfMassConfig.Offset) PropertyField("centerOfMassOffset");
			if (Body.CenterOfMassConfig == CenterOfMassConfig.Transform) PropertyField("centerOfMassTransform");
			PropertyField("inertiaTensor", "Moment Of Inertia");
			PropertyField("inertiaTensorRotation", "Principal Axes Of Inertia");
			PropertyField("gravityMultiplier");
			PropertyField("maxAngularSpeed");
			PropertyField("solverIterations");
			PropertyField("solverVelocityIterations");
			DecreaseIndent();
			// 空力について
			Space();
			Header("Aerodynamics");
			IncreaseIndent();
			PropertyField("linearDrag");
			PropertyField("quadraticDrag");
			PropertyField("downforceCoefficient");
			PropertyField("downforceShift");
			PropertyField("maxDownforce");
			DecreaseIndent();
			// 衝突について
			Space();
			Header("Collision");
			IncreaseIndent();
			PropertyField("maxDepenetrationSpeed");
			PropertyField("maxAngularVelocityDeltaOnCollision", "Max Angular Velocity Delta");
			DecreaseIndent();
			// 更新する
			serializedObject.ApplyModifiedProperties();
		}
	}
}
