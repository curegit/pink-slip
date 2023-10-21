using UnityEditor;

namespace PinkSlip.EditorOnly
{
	/// <summary>
	/// 車両の足回りのエディタ拡張
	/// </summary>
	[CustomEditor(typeof(Chassis)), CanEditMultipleObjects]
	public class ChassisEditor : SpeedcarEditor
	{
		/// <summary>
		/// 対象コンポーネント
		/// </summary>
		public Chassis Chassis => (Chassis)target;

		/// <summary>
		/// インスペクターを拡張する
		/// </summary>
		public override void OnInspectorGUI()
		{
			// 同期する
			serializedObject.Update();
			// ホイールコライダーについて
			Space();
			Header("Wheel Colliders");
			IncreaseIndent();
			PropertyField("frontLeftWheelCollider");
			PropertyField("frontRightWheelCollider");
			PropertyField("rearLeftWheelCollider");
			PropertyField("rearRightWheelCollider");
			DecreaseIndent();
			// サスペンションについて
			Space();
			Header("Suspension");
			IncreaseIndent();
			PropertyField("frontNaturalFrequency");
			PropertyField("rearNaturalFrequency");
			PropertyField("dampingRatio");
			PropertyField("targetPosition");
			PropertyField("forceShift");
			PropertyField("autoConfigureSuspensionDistance");
			if (!Chassis.AutoConfigureSuspensionDistance)
			{
				PropertyField("frontSuspensionDistance");
				PropertyField("rearSuspensionDistance");
			}
			DecreaseIndent();
			// ステアリングとアライメントについて
			Space();
			Header("Steering & Alignment");
			IncreaseIndent();
			PropertyField("maxSteerAngle");
			PropertyField("ackermannCoefficient");
			PropertyField("frontToeAngle");
			PropertyField("rearToeAngle");
			PropertyField("antiOversteer");
			DecreaseIndent();
			// ブレーキについて
			Space();
			Header("Brakes");
			IncreaseIndent();
			PropertyField("brakeTorque");
			PropertyField("brakeBias");
			PropertyField("handBrakeTorque");
			DecreaseIndent();
			// スタビライザーについて
			Space();
			Header("Stabilizers");
			IncreaseIndent();
			PropertyField("stabilizerCoefficient");
			PropertyField("stabilizerBias");
			DecreaseIndent();
			// タイヤについて
			Space();
			Header("Tires");
			IncreaseIndent();
			PropertyField("frictionCurveSet.forwardExtremumSlip");
			PropertyField("frictionCurveSet.forwardExtremumValue");
			PropertyField("frictionCurveSet.forwardAsymptoteSlip");
			PropertyField("frictionCurveSet.forwardAsymptoteValue");
			PropertyField("frictionCurveSet.sidewaysExtremumSlip");
			PropertyField("frictionCurveSet.sidewaysExtremumValue");
			PropertyField("frictionCurveSet.sidewaysAsymptoteSlip");
			PropertyField("frictionCurveSet.sidewaysAsymptoteValue");
			PropertyField("frictionCurveSet.frontForwardStiffness");
			PropertyField("frictionCurveSet.rearForwardStiffness");
			PropertyField("frictionCurveSet.frontSidewaysStiffness");
			PropertyField("frictionCurveSet.rearSidewaysStiffness");
			PropertyField("maxGripMultiplier");
			PropertyField("maxGripMultiplierSpeed");
			DecreaseIndent();
			// サブステップについて
			Space();
			Header("Substepping");
			IncreaseIndent();
			PropertyField("substepsSpeedThreshold");
			PropertyField("substepsBelowThreshold");
			PropertyField("substepsAboveThreshold");
			DecreaseIndent();
			// 更新する
			serializedObject.ApplyModifiedProperties();
			// プロパティを通して値を検査する
			Chassis.FrontLeftWheelCollider = Chassis.FrontLeftWheelCollider;
			Chassis.FrontRightWheelCollider = Chassis.FrontRightWheelCollider;
			Chassis.RearLeftWheelCollider = Chassis.RearLeftWheelCollider;
			Chassis.RearRightWheelCollider = Chassis.RearRightWheelCollider;
			Chassis.FrontNaturalFrequency = Chassis.FrontNaturalFrequency;
			Chassis.RearNaturalFrequency = Chassis.RearNaturalFrequency;
			Chassis.DampingRatio = Chassis.DampingRatio;
			Chassis.TargetPosition = Chassis.TargetPosition;
			Chassis.ForceShift = Chassis.ForceShift;
			Chassis.AutoConfigureSuspensionDistance = Chassis.AutoConfigureSuspensionDistance;
			Chassis.FrontSuspensionDistance = Chassis.FrontSuspensionDistance;
			Chassis.RearSuspensionDistance = Chassis.RearSuspensionDistance;
			Chassis.MaxSteerAngle = Chassis.MaxSteerAngle;
			Chassis.AckermannCoefficient = Chassis.AckermannCoefficient;
			Chassis.FrontToeAngle = Chassis.FrontToeAngle;
			Chassis.RearToeAngle = Chassis.RearToeAngle;
			Chassis.AntiOversteer = Chassis.AntiOversteer;
			Chassis.BrakeTorque = Chassis.BrakeTorque;
			Chassis.BrakeBias = Chassis.BrakeBias;
			Chassis.HandBrakeTorque = Chassis.HandBrakeTorque;
			Chassis.StabilizerCoefficient = Chassis.StabilizerCoefficient;
			Chassis.StabilizerBias = Chassis.StabilizerBias;
			Chassis.FrictionCurveSet.ForwardExtremumSlip = Chassis.FrictionCurveSet.ForwardExtremumSlip;
			Chassis.FrictionCurveSet.ForwardExtremumValue = Chassis.FrictionCurveSet.ForwardExtremumValue;
			Chassis.FrictionCurveSet.ForwardAsymptoteSlip = Chassis.FrictionCurveSet.ForwardAsymptoteSlip;
			Chassis.FrictionCurveSet.ForwardAsymptoteValue = Chassis.FrictionCurveSet.ForwardAsymptoteValue;
			Chassis.FrictionCurveSet.SidewaysExtremumSlip = Chassis.FrictionCurveSet.SidewaysExtremumSlip;
			Chassis.FrictionCurveSet.SidewaysExtremumValue = Chassis.FrictionCurveSet.SidewaysExtremumValue;
			Chassis.FrictionCurveSet.SidewaysAsymptoteSlip = Chassis.FrictionCurveSet.SidewaysAsymptoteSlip;
			Chassis.FrictionCurveSet.SidewaysAsymptoteValue = Chassis.FrictionCurveSet.SidewaysAsymptoteValue;
			Chassis.FrictionCurveSet.FrontForwardStiffness = Chassis.FrictionCurveSet.FrontForwardStiffness;
			Chassis.FrictionCurveSet.RearForwardStiffness = Chassis.FrictionCurveSet.RearForwardStiffness;
			Chassis.FrictionCurveSet.FrontSidewaysStiffness = Chassis.FrictionCurveSet.FrontSidewaysStiffness;
			Chassis.FrictionCurveSet.RearSidewaysStiffness = Chassis.FrictionCurveSet.RearSidewaysStiffness;
			Chassis.FrictionCurveSet = Chassis.FrictionCurveSet;
			Chassis.MaxGripMultiplier = Chassis.MaxGripMultiplier;
			Chassis.MaxGripMultiplierSpeed = Chassis.MaxGripMultiplierSpeed;
			Chassis.SubstepsSpeedThreshold = Chassis.SubstepsSpeedThreshold;
			Chassis.SubstepsAboveThreshold = Chassis.SubstepsAboveThreshold;
			Chassis.SubstepsBelowThreshold = Chassis.SubstepsBelowThreshold;
		}
	}
}
