using UnityEditor;

namespace Speedcar.EditorOnly
{
	/// <summary>
	/// 車両の入力インターフェイスのエディタ拡張
	/// </summary>
	[CustomEditor(typeof(CarController)), CanEditMultipleObjects]
	public class CarControllerEditor : SpeedcarEditor
	{
		/// <summary>
		/// 対象コンポーネント
		/// </summary>
		public CarController CarController => (CarController)target;

		/// <summary>
		/// インスペクターを拡張する
		/// </summary>
		public override void OnInspectorGUI()
		{
			// 同期する
			serializedObject.Update();
			// ABSについて
			PropertyField("useAntiLockBrake");
			if (CarController.UseAntiLockBrake)
			{
				PropertyField("antiLockBrakeEpsilon");
				PropertyField("antiLockBrakeSlipMargin");
				PropertyField("antiLockBrakeMaxStepDelta");
			}
			// TCSについて
			PropertyField("useTractionControl");
			if (CarController.UseTractionControl)
			{
				PropertyField("tractionControlEpsilon");
				PropertyField("tractionControlSlipMargin");
				PropertyField("tractionControlMaxGasDelta");
			}
			// ステアリングについて
			PropertyField("limitSteerRate");
			if (CarController.LimitSteerRate)
			{
				PropertyField("limitedMaxSteerRate");
				PropertyField("limitedSteerRateSpeed");
			}
			// 更新する
			serializedObject.ApplyModifiedProperties();
			// プロパティを通して値を検査する
			CarController.UseAntiLockBrake = CarController.UseAntiLockBrake;
			CarController.AntiLockBrakeEpsilon = CarController.AntiLockBrakeEpsilon;
			CarController.AntiLockBrakeSlipMargin = CarController.AntiLockBrakeSlipMargin;
			CarController.AntiLockBrakeMaxStepDelta = CarController.AntiLockBrakeMaxStepDelta;
			CarController.UseTractionControl = CarController.UseTractionControl;
			CarController.TractionControlEpsilon = CarController.TractionControlEpsilon;
			CarController.TractionControlSlipMargin = CarController.TractionControlSlipMargin;
			CarController.TractionControlMaxGasDelta = CarController.TractionControlMaxGasDelta;
			CarController.LimitSteerRate = CarController.LimitSteerRate;
			CarController.LimitedMaxSteerRate = CarController.LimitedMaxSteerRate;
			CarController.LimitedSteerRateSpeed = CarController.LimitedSteerRateSpeed;
		}
	}
}
