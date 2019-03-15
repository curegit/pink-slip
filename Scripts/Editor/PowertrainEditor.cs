using UnityEditor;

namespace Speedcar.EditorOnly
{
	/// <summary>
	/// 動力系コンポーネントのエディタ拡張
	/// </summary>
	[CustomEditor(typeof(Powertrain)), CanEditMultipleObjects]
	public class PowertrainEditor : SpeedcarEditor
	{
		/// <summary>
		/// 対象コンポーネント
		/// </summary>
		private Powertrain Powertrain => (Powertrain)target;

		/// <summary>
		/// インスペクターを拡張する
		/// </summary>
		public override void OnInspectorGUI()
		{
			// 同期する
			serializedObject.Update();
			// エンジンについて
			Space();
			Header("Engine");
			IncreaseIndent();
			Header("Torque");
			IncreaseIndent();
			PropertyField("torqueCurveModel");
			if (Powertrain.TorqueCurveModel == TorqueCurveModel.AnimationCurve)
			{
				PropertyField("torqueAnimationCurve");
			}
			PropertyField("idlingRPM");
			if (Powertrain.TorqueCurveModel == TorqueCurveModel.Parametric)
			{
				PropertyField("parametricTorqueCurve.idlingTorque");
				PropertyField("parametricTorqueCurve.maxTorqueLowerRPM");
				PropertyField("parametricTorqueCurve.maxTorqueUpperRPM");
				PropertyField("parametricTorqueCurve.maxTorque");
				PropertyField("parametricTorqueCurve.redline");
				PropertyField("parametricTorqueCurve.redlineTorque");
			}
			PropertyField("revLimit");
			PropertyField("maxRPM");
			PropertyField("torqueMultiplier");
			DecreaseIndent();
			PropertyField("engineInertia");
			PropertyField("engineBrakingCoefficient", "Engine Braking");
			PropertyField("wheelDampingRate");
			PropertyField("minPressure");
			PropertyField("maxPlainPressure");
			PropertyField("hasForcedInduction", "Forced Induction");
			if (Powertrain.HasForcedInduction)
			{
				PropertyField("forcedInductionDevice", "Device");
				PropertyField("maxForcedInductionEfficiency", "Efficiency");
				PropertyField("maxAdditionalForcedPressure", "Additional Pressure");
				PropertyField("forcedPressureMaxDeltaRate", "Pressure Max Delta Rate");
			}
			PropertyField("hasNitrous", "Nitrous");
			if (Powertrain.HasNitrous)
			{
				PropertyField("nitrousEfficiency");
			}
			DecreaseIndent();
			// 変速機について
			Space();
			Header("Transmission");
			IncreaseIndent();
			PropertyField("gearRatios");
			PropertyField("reverseGearRatio");
			PropertyField("finalDriveRatio");
			DecreaseIndent();
			// 伝達系について（変速機以外）
			Space();
			Header("Driveline");
			IncreaseIndent();
			PropertyField("drivetrain");
			PropertyField("frontDifferentialLocking");
			PropertyField("rearDifferentialLocking");
			if (Powertrain.Drivetrain == Drivetrain.AllWheelDrive)
			{
				PropertyField("centerDifferentialLocking");
				if (Powertrain.CenterDifferentialLocking > 0f)
				{
					PropertyField("centerDifferentialBalance");
				}
			}
			PropertyField("clutchTime");
			DecreaseIndent();
			// 更新する
			serializedObject.ApplyModifiedProperties();
		}
	}
}
