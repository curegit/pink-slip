using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 車両の動力系と伝動機構
	/// </summary>
	[RequireComponent(typeof(Suspension)), DisallowMultipleComponent]
	public class Powertrain : MonoBehaviour
	{
		/// <summary>
		/// トルク曲線の表現方法
		/// </summary>
		[SerializeField]
		private TorqueCurveModel torqueCurveModel = TorqueCurveModel.Parametric;

		/// <summary>
		/// パラメトリックなトルク曲線
		/// </summary>
		[SerializeField]
		private ParametricTorqueCurve parametricTorqueCurve = new ParametricTorqueCurve();

		/// <summary>
		/// トルク曲線のアニメーションカーブ
		/// </summary>
		[SerializeField]
		public AnimationCurve torqueAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1000, 10), new Keyframe(5000, 600), new Keyframe(7800, 800), new Keyframe(8000, 0));

		/// <summary>
		/// トルクを定数倍するための乗数
		/// </summary>
		[SerializeField]
		private float torqueMultiplier = 1f;

		/// <summary>
		/// アイドル時の回転数
		/// </summary>
		[SerializeField]
		private float idlingRPM = 1000f;

		/// <summary>
		/// レブリミット
		/// </summary>
		[SerializeField]
		private float revLimit = 8000f;

		[SerializeField]
		private float maxRPM = 12000f;

		[SerializeField]
		private float engineInertia = 0.01f;

		[SerializeField]
		private float engineBrakingCoefficient = 10.5f;

		[SerializeField]
		private float wheelDampingRate = 0.15f;

		[SerializeField]
		private float minPressure = 10000f;

		[SerializeField]
		private float maxPlainPressure = 101300f;

		[SerializeField]
		private bool hasForcedInduction;

		[SerializeField]
		private ForcedInduction forcedInductionDevice = ForcedInduction.Turbocharger;

		[SerializeField]
		private float maxForcedInductionEfficiency = 1.45f;

		[SerializeField]
		private float maxAdditionalForcedPressure;

		[SerializeField]
		private float forcedPressureMaxDeltaRate = 1.2f;



		[SerializeField]
		private bool hasNitrous;

		[SerializeField]
		private float nitrousEfficiency = 1.6f;




		[SerializeField, Range(0.1f, 5.0f)]
		private float[] gearRatios = { 3.2f, 1.9f, 1.3f, 1.0f, 0.7f, 0.5f };

		[SerializeField, Range(0.1f, 5.0f)]
		private float reverseGearRatio = 2.9f;

		[SerializeField, Range(0.1f, 5.0f)]
		private float finalDriveRatio = 3.7f;


		[SerializeField]
		private Drivetrain drivetrain = Drivetrain.AllWheelDrive;

		[SerializeField]
		private float frontDifferentialLocking = 0.5f;

		[SerializeField]
		private float rearDifferentialLocking = 0.5f;

		[SerializeField]
		private float centerDifferentialLocking = 1f;

		[SerializeField]
		private float centerDifferentialBalance = 0.6f;

		[SerializeField]
		private float clutchTime = 0.6f;

		private float throttle;

		private int gear = 1;

		/// <summary>
		/// トルク曲線の表現方法
		/// </summary>
		public TorqueCurveModel TorqueCurveModel
		{
			get
			{
				return torqueCurveModel;
			}
			set
			{
				torqueCurveModel = value;
			}
		}

		/// <summary>
		/// パラメトリックなトルク曲線
		/// </summary>
		public ParametricTorqueCurve ParametricTorqueCurve
		{
			get
			{
				return parametricTorqueCurve;
			}
			set
			{
				parametricTorqueCurve = value;
			}
		}

		/// <summary>
		/// トルク曲線のアニメーションカーブ
		/// </summary>
		public AnimationCurve TorqueAnimationCurve
		{
			get
			{
				return torqueAnimationCurve;
			}
			set
			{
				torqueAnimationCurve = value;
			}
		}

		/// <summary>
		/// トルクを定数倍するための乗数
		/// </summary>
		public float TorqueMultiplier
		{
			get
			{
				return torqueMultiplier;
			}
			set
			{
				torqueMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// アイドル時の回転数
		/// </summary>
		public float IdlingRPM
		{
			get
			{
				return idlingRPM;
			}
			set
			{
				idlingRPM = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 回転数の上限値
		/// </summary>
		public float RevLimit
		{
			get
			{
				return revLimit;
			}
			set
			{
				revLimit = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 値として取りうる最大の回転数
		/// </summary>
		public float MaxRPM
		{
			get
			{
				return maxRPM;
			}
			set
			{
				maxRPM = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// エンジンの慣性モーメント
		/// </summary>
		public float EngineInertia
		{
			get
			{
				return engineInertia;
			}
			set
			{
				engineInertia = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// エンジンブレーキ係数
		/// </summary>
		public float EngineBrakingCoefficient
		{
			get
			{
				return engineBrakingCoefficient;
			}
			set
			{
				engineBrakingCoefficient = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 車輪の減衰率
		/// </summary>
		public float WheelDampingRate
		{
			get
			{
				return wheelDampingRate;
			}
			set
			{
				wheelDampingRate = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// エンジンの最小圧力（絶対圧 Pa）
		/// </summary>
		public float MinPressure
		{
			get
			{
				return minPressure;
			}
			set
			{
				minPressure = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 過給器なしの場合のエンジンの最大圧力（絶対圧 Pa）
		/// </summary>
		public float MaxPlainPressure
		{
			get
			{
				return maxPlainPressure;
			}
			set
			{
				maxPlainPressure = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float MaxPressure
		{
			get
			{
				if (HasForcedInduction)
				{
					return MaxPlainPressure + MaxAdditionalForcedPressure;
				}
				else
				{
					return MaxPlainPressure;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasForcedInduction
		{
			get
			{
				return hasForcedInduction;
			}
			set
			{
				hasForcedInduction = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ForcedInduction ForcedInductionDevice
		{
			get
			{
				return forcedInductionDevice;
			}
			set
			{
				forcedInductionDevice = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float MaxForcedInductionEfficiency
		{
			get
			{
				return maxForcedInductionEfficiency;
			}
			set
			{
				maxForcedInductionEfficiency = Mathf.Max(value, 1f);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float MaxAdditionalForcedPressure
		{
			get
			{
				return maxAdditionalForcedPressure;
			}
			set
			{
				maxAdditionalForcedPressure = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float ForcedPressureMaxDeltaRate
		{
			get
			{
				return forcedPressureMaxDeltaRate;
			}
			set
			{
				forcedPressureMaxDeltaRate = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 亜酸化窒素噴射システムを搭載しているか
		/// </summary>
		private bool HasNitrous
		{
			get
			{
				return hasNitrous;
			}
			set
			{
				hasNitrous = value;
			}
		}

		/// <summary>
		/// 亜酸化窒素噴射時のトルク増幅
		/// </summary>
		public float NitrousEfficiency
		{
			get
			{
				return nitrousEfficiency;
			}
			set
			{
				nitrousEfficiency = Mathf.Max(value, 1f);
			}
		}

		/// <summary>
		/// 前進用の変速比のコレクション
		/// </summary>
		public IEnumerable<float> GearRatios
		{
			get
			{
				foreach (var gearRatio in gearRatios)
				{
					yield return gearRatio;
				}
			}
			set
			{
				gearRatios = value.Select(r => Mathf.Max(r, 0f)).ToArray();
			}
		}

		/// <summary>
		/// 後退用の変速比（正の値）
		/// </summary>
		public float ReverseGearRatio
		{
			get
			{
				return reverseGearRatio;
			}
			set
			{
				reverseGearRatio = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 減速比
		/// </summary>
		public float FinalDriveRatio
		{
			get
			{
				return finalDriveRatio;
			}
			set
			{
				finalDriveRatio = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 現在のギアの変速比
		/// </summary>
		public float CurrentGearRatio
		{
			get
			{
				return GearRatio(Gear);
			}
		}

		/// <summary>
		/// 最高ギアの番号
		/// </summary>
		public int TopGear => GearRatios.Count();

		/// <summary>
		/// 駆動方式
		/// </summary>
		public Drivetrain Drivetrain
		{
			get
			{
				return drivetrain;
			}
			set
			{
				drivetrain = value;
			}
		}

		/// <summary>
		/// 前輪の差動の固定化（0=開放 1=固定）
		/// </summary>
		public float FrontDifferentialLocking
		{
			get
			{
				return frontDifferentialLocking;
			}
			set
			{
				frontDifferentialLocking = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 後輪の差動の固定化（0=開放 1=固定）
		/// </summary>
		public float RearDifferentialLocking
		{
			get
			{
				return rearDifferentialLocking;
			}
			set
			{
				rearDifferentialLocking = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// トルクの前後分配の固定化（0=開放 1=固定 四輪駆動のみ）
		/// </summary>
		public float CenterDifferentialLocking
		{
			get
			{
				return centerDifferentialLocking;
			}
			set
			{
				centerDifferentialLocking = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// トルクの前後分配の固定時の配分（0=すべて前 1=すべて後ろ 四輪駆動のみ）
		/// </summary>
		public float CenterDifferentialBalance
		{
			get
			{
				return centerDifferentialBalance;
			}
			set
			{
				centerDifferentialBalance = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// クラッチをつなぐ時間
		/// </summary>
		public float ClutchTime
		{
			get
			{
				return clutchTime;
			}
			set
			{
				clutchTime = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// スロットル開度（0=全閉 1=全開）
		/// </summary>
		public float Throttle
		{
			get
			{
				return throttle;
			}
			set
			{
				throttle = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 現在のギア
		/// </summary>
		public int Gear
		{
			get
			{
				return gear;
			}
			set
			{
				// 有効な範囲に丸める
				int g = Mathf.Clamp(value, -1, TopGear);
				// クラッチを切る
				if (g != gear)
				{
					Clutch = 1f;
				}
				// 更新する
				gear = g;
			}
		}

		/// <summary>
		/// 亜酸化窒素を噴射しているか
		/// </summary>
		public bool Nitrous { get; set; }

		/// <summary>
		/// エンジン回転数
		/// </summary>
		public float RPM { get; private set; }

		/// <summary>
		/// 現在のエンジンの出力トルク（Nm）
		/// </summary>
		public float Torque { get; private set; }

		/// <summary>
		/// エンジンの内圧（絶対圧 Pa）
		/// </summary>
		public float Pressure { get; private set; }

		/// <summary>
		/// 亜酸化窒素残量（秒）
		/// </summary>
		public float RemainingNitrous { get; set; } = 3f;

		/// <summary>
		/// 亜酸化窒素によるトルク増幅
		/// </summary>
		private float NitrousCurrentEfficiency { get; set; }

		/// <summary>
		/// クラッチの状態（0=接続 1=断絶）
		/// </summary>
		private float Clutch { get; set; }

		/// <summary>
		/// 足回りのコンポーネント
		/// </summary>
		private Suspension Suspension { get; set; }

		/// <summary>
		/// 初期化時に呼ばれるメソッド
		/// </summary>
		private void Start()
		{
			Suspension = GetComponent<Suspension>();
		}

		/// <summary>
		/// 毎物理フレームに呼ばれるメソッド
		/// </summary>
		private void FixedUpdate()
		{
			UpdateClutch();
			UpdatePressure();
			UpdateRPM();
			UpdateNitrous();
			UpdateTorque();
			ApplyTorque();
			ApplyWheelDamping();
		}

		/// <summary>
		/// クラッチを更新する
		/// </summary>
		private void UpdateClutch()
		{
			Clutch = Mathf.MoveTowards(Clutch, 0f, Time.fixedDeltaTime / ClutchTime);
		}

		/// <summary>
		/// エンジンの内圧を更新する
		/// </summary>
		private void UpdatePressure()
		{
			// 過給器ありの場合
			if (HasForcedInduction)
			{
				// 目標過給圧を求める
				float target = Mathf.Lerp(MinPressure, MaxPressure, Throttle);
				// フレームごとの最大の加圧を求める
				float maxDelta = Time.fixedDeltaTime * ForcedPressureMaxDeltaRate * (MaxPressure - MinPressure);
				// ターボチャージャーの場合
				if (ForcedInductionDevice == ForcedInduction.Turbocharger)
				{
					// 圧力の上昇速度は排気量に比例
					if (target > Pressure)
					{
						Pressure = Mathf.MoveTowards(Pressure, target, Mathf.Lerp(0f, maxDelta, Mathf.InverseLerp(0f, RevLimit, RPM * Throttle)));
					}
					// 圧力の減少は直ちに従う
					else
					{
						Pressure = target;
					}
				}
				// スーパーチャージャーの場合
				else if (ForcedInductionDevice == ForcedInduction.Supercharger)
				{
					// 圧力の上昇速度はスロットル開度に比例
					if (target > Pressure)
					{
						Pressure = Mathf.MoveTowards(Pressure, target, Mathf.Lerp(0f, maxDelta, Throttle));
					}
					// 圧力の減少は直ちに従う
					else
					{
						Pressure = target;
					}
				}
			}
			// 自然吸気エンジンの場合
			else
			{
				Pressure = Mathf.Lerp(MinPressure, MaxPressure, Throttle);
			}
		}

		/// <summary>
		/// エンジン回転数を更新する
		/// </summary>
		private void UpdateRPM()
		{
			// Nレンジ以外の場合
			if (Gear != 0)
			{
				// 駆動輪のRPMを計算する
				float wheelRPM = 0f;
				switch (Drivetrain)
				{
					// 前輪駆動の場合
					case Drivetrain.FrontWheelDrive:
						wheelRPM = Suspension.FrontWheelColliders.Average(x => Mathf.Abs(x.rpm));
						break;
					// 後輪駆動の場合
					case Drivetrain.RearWheelDrive:
						wheelRPM = Suspension.RearWheelColliders.Average(x => Mathf.Abs(x.rpm));
						break;
					// 四輪駆動の場合
					case Drivetrain.AllWheelDrive:
						float frontWheelRPM = Suspension.FrontWheelColliders.Average(x => Mathf.Abs(x.rpm));
						float rearWheelRPM = Suspension.RearWheelColliders.Average(x => Mathf.Abs(x.rpm));
						wheelRPM = (frontWheelRPM + rearWheelRPM) / 2f;
						break;
				}
				// エンジン回転数に変換
				float rpm = Transmission(wheelRPM) + IdlingRPM;
				// クラッチ代わりの近似を行う
				rpm = Mathf.Lerp(rpm, RPM, Clutch);
				// 有効な範囲に丸めて更新
				RPM = Mathf.Clamp(rpm, IdlingRPM, MaxRPM);
			}
			// Nレンジの場合
			else
			{
				// 角加速度から回転数を更新
				float rpm = RPM + Torque / EngineInertia * Time.fixedDeltaTime * 60f / (2f * Mathf.PI);
				// エンジンブレーキを適用
				rpm = rpm * (1f - Time.fixedDeltaTime * Mathf.InverseLerp(MaxPlainPressure, MinPressure, Pressure) * Mathf.InverseLerp(0f, MaxRPM, RPM) * EngineBrakingCoefficient);
				// 有効な範囲に丸めて更新
				RPM = Mathf.Clamp(rpm, IdlingRPM, MaxRPM);
			}
		}

		/// <summary>
		/// 亜酸化窒素噴射システムを更新
		/// </summary>
		private void UpdateNitrous()
		{
			if (HasNitrous && RemainingNitrous > 0f && Nitrous)
			{
				NitrousCurrentEfficiency = NitrousEfficiency;
				RemainingNitrous = Mathf.Max(RemainingNitrous - Time.fixedDeltaTime, 0f);
			}
			else
			{
				NitrousCurrentEfficiency = 1f;
			}
		}

		/// <summary>
		/// 出力トルクを更新する
		/// </summary>
		private void UpdateTorque()
		{
			// 素のトルクを計算
			float torque = Throttle * CalculatePlainTorqueAt(RPM);
			// レブリミットを超えていないか検査する
			torque = RPM > RevLimit ? 0f : torque;
			// 過給機による増幅を適用
			if (HasForcedInduction)
			{
				torque *= InterpolateForcedInductionEfficiency(Pressure);
			}
			// ナイトロによる増幅を適用
			if (HasNitrous)
			{
				torque *= NitrousCurrentEfficiency;
			}
			// エンジンからの出力トルクのプロパティを更新
			Torque = torque;
		}

		/// <summary>
		/// 駆動方式の違いよって構成が異なるデフを通じてトルクを伝達する
		/// </summary>
		private void ApplyTorque()
		{
			// 変速機を通してトルクを変換
			float torque = Transmission(Torque);
			// 前輪駆動の場合
			if (Drivetrain == Drivetrain.FrontWheelDrive)
			{
				// 前輪のデフを通してトルクを伝達
				float frontLeftRPM = Math.Abs(Suspension.FrontLeftWheelCollider.rpm);
				float frontRightRPM = Mathf.Abs(Suspension.FrontRightWheelCollider.rpm);
				float frontLeftOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? torque * frontLeftRPM / (frontLeftRPM + frontRightRPM) : torque / 2f;
				float frontRightOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? torque * frontRightRPM / (frontLeftRPM + frontRightRPM) : torque / 2f;
				float frontLeftLockedTorque = torque / 2f;
				float frontRightLockedTorque = torque / 2f;
				Suspension.FrontLeftWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(frontLeftOpenTorque, frontLeftLockedTorque, FrontDifferentialLocking);
				Suspension.FrontRightWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(frontRightOpenTorque, frontRightLockedTorque, FrontDifferentialLocking);
				// 後輪に動力は伝わらない
				foreach (var wheelCollider in Suspension.RearWheelColliders)
				{
					wheelCollider.motorTorque = 0f;
				}
			}
			// 後輪駆動の場合
			else if (Drivetrain == Drivetrain.RearWheelDrive)
			{
				// 後輪のデフを通してトルクを伝達
				float rearLeftRPM = Math.Abs(Suspension.RearLeftWheelCollider.rpm);
				float rearRightRPM = Mathf.Abs(Suspension.RearRightWheelCollider.rpm);
				float rearLeftOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? torque * rearLeftRPM / (rearLeftRPM + rearRightRPM) : torque / 2f;
				float rearRightOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? torque * rearRightRPM / (rearLeftRPM + rearRightRPM) : torque / 2f;
				float rearLeftLockedTorque = torque / 2f;
				float rearRightLockedTorque = torque / 2f;
				Suspension.RearLeftWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(rearLeftOpenTorque, rearLeftLockedTorque, RearDifferentialLocking);
				Suspension.RearRightWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(rearRightOpenTorque, rearRightLockedTorque, RearDifferentialLocking);
				// 前輪に動力は伝わらない
				foreach (var wheelCollider in Suspension.FrontWheelColliders)
				{
					wheelCollider.motorTorque = 0f;
				}
			}
			// 四輪駆動の場合
			else if (Drivetrain == Drivetrain.AllWheelDrive)
			{
				// センターデフの動作を再現
				float frontRPM = Math.Abs(Suspension.FrontWheelColliders.Average(x => x.rpm));
				float rearRPM = Math.Abs(Suspension.RearWheelColliders.Average(x => x.rpm));
				float frontOpenTorque = (frontRPM + rearRPM > 1f) ? torque * frontRPM / (frontRPM + rearRPM) : torque / 2f;
				float rearOpenTorque = (frontRPM + rearRPM > 1f) ? torque * rearRPM / (frontRPM + rearRPM) : torque / 2f;
				float frontLockedTorque = Mathf.Lerp(torque, 0f, CenterDifferentialBalance);
				float rearLockedTorque = Mathf.Lerp(0f, torque, CenterDifferentialBalance);
				float frontTorque = Mathf.Lerp(frontOpenTorque, frontLockedTorque, CenterDifferentialLocking);
				float rearTorque = Mathf.Lerp(rearOpenTorque, rearLockedTorque, CenterDifferentialLocking);
				// 前輪のデフを通してトルクを伝達
				float frontLeftRPM = Math.Abs(Suspension.FrontLeftWheelCollider.rpm);
				float frontRightRPM = Mathf.Abs(Suspension.FrontRightWheelCollider.rpm);
				float frontLeftOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? frontTorque * frontLeftRPM / (frontLeftRPM + frontRightRPM) : frontTorque / 2f;
				float frontRightOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? frontTorque * frontRightRPM / (frontLeftRPM + frontRightRPM) : frontTorque / 2f;
				float frontLeftLockedTorque = frontTorque / 2f;
				float frontRightLockedTorque = frontTorque / 2f;
				Suspension.FrontLeftWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(frontLeftOpenTorque, frontLeftLockedTorque, FrontDifferentialLocking);
				Suspension.FrontRightWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(frontRightOpenTorque, frontRightLockedTorque, FrontDifferentialLocking);
				// 後輪のデフを通してトルクを伝達
				float rearLeftRPM = Math.Abs(Suspension.RearLeftWheelCollider.rpm);
				float rearRightRPM = Mathf.Abs(Suspension.RearRightWheelCollider.rpm);
				float rearLeftOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? rearTorque * rearLeftRPM / (rearLeftRPM + rearRightRPM) : rearTorque / 2f;
				float rearRightOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? rearTorque * rearRightRPM / (rearLeftRPM + rearRightRPM) : rearTorque / 2f;
				float rearLeftLockedTorque = rearTorque / 2f;
				float rearRightLockedTorque = rearTorque / 2f;
				Suspension.RearLeftWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(rearLeftOpenTorque, rearLeftLockedTorque, RearDifferentialLocking);
				Suspension.RearRightWheelCollider.motorTorque = (Gear == -1 ? -1f : 1f) * Mathf.Lerp(rearRightOpenTorque, rearRightLockedTorque, RearDifferentialLocking);
			}
		}

		/// <summary>
		/// 車輪の減衰とエンジンブレーキをシミュレートする
		/// </summary>
		private void ApplyWheelDamping()
		{
			// Nレンジ以外の場合
			if (Gear != 0)
			{
				// エンジンブレーキによる減衰を近似で求める
				float dampingByEngineBraking = Mathf.InverseLerp(MaxPlainPressure, MinPressure, Pressure) * Mathf.InverseLerp(0f, MaxRPM, RPM) * EngineBrakingCoefficient;
				// 
				float damping = RPM + 1f > MaxRPM ? Mathf.Pow(EngineBrakingCoefficient, 2f) : 0f;
				// 車輪の素の減衰と合わせる
				float totalDamping = WheelDampingRate + dampingByEngineBraking + damping;
				// エンジンブレーキは駆動輪にのみ適用
				switch (Drivetrain)
				{
					// 前輪駆動の場合
					case Drivetrain.FrontWheelDrive:
						foreach (var wheelCollider in Suspension.FrontWheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						foreach (var wheelCollider in Suspension.RearWheelColliders)
						{
							wheelCollider.wheelDampingRate = WheelDampingRate;
						}
						break;
					// 後輪駆動の場合
					case Drivetrain.RearWheelDrive:
						foreach (var wheelCollider in Suspension.FrontWheelColliders)
						{
							wheelCollider.wheelDampingRate = WheelDampingRate;
						}
						foreach (var wheelCollider in Suspension.RearWheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						break;
					// 四輪駆動の場合
					case Drivetrain.AllWheelDrive:
						foreach (var wheelCollider in Suspension.WheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						break;
				}
			}
			// Nレンジの場合
			else
			{
				// 素の減衰を適用
				foreach (var wheelCollider in Suspension.WheelColliders)
				{
					wheelCollider.wheelDampingRate = WheelDampingRate;
				}
			}
		}

		/// <summary>
		/// ある回転数における過給を含まないトルクを返す
		/// </summary>
		/// <param name="rpm">エンジン回転数</param>
		/// <returns>エンジン出力トルク</returns>
		private float CalculatePlainTorqueAt(float rpm)
		{
			float torque = 0f;
			switch (TorqueCurveModel)
			{
				case TorqueCurveModel.Parametric:
					torque = ParametricTorqueCurve != null ? ParametricTorqueCurve.EngineTorque(rpm, IdlingRPM, MaxRPM) : 0f;
					break;
				case TorqueCurveModel.AnimationCurve:
					torque = TorqueAnimationCurve != null && TorqueAnimationCurve.length > 0 ? TorqueAnimationCurve.Evaluate(rpm) : 0f;
					break;
			}
			return torque * TorqueMultiplier;
		}

		/// <summary>
		/// 過給器によるトルク増幅を近似補間する（増幅されないときは等倍を返す）
		/// </summary>
		/// <param name="pressure">過給圧</param>
		/// <returns>トルク増幅値</returns>
		private float InterpolateForcedInductionEfficiency(float pressure)
		{
			if (pressure < MaxPlainPressure)
			{
				return 1f;
			}
			else
			{
				float t = Mathf.InverseLerp(MaxPlainPressure, MaxPressure, pressure);
				float d = Mathf.Lerp(1f, MaxForcedInductionEfficiency, t);
				return Mathf.Lerp(d, MaxForcedInductionEfficiency, t);
			}
		}

		/// <summary>
		/// 変速比を返す
		/// </summary>
		/// <param name="gear">ギア番号</param>
		/// <returns>変速比</returns>
		private float GearRatio(int gear)
		{
			if (gear == 0)
			{
				return 0f;
			}
			else if (gear == -1)
			{
				return ReverseGearRatio;
			}
			else
			{
				return GearRatios.ElementAt(gear - 1);
			}
		}

		/// <summary>
		/// 値を変速にかける
		/// </summary>
		/// <param name="v">値</param>
		/// <returns>変速した値</returns>
		private float Transmission(float v)
		{
			return v * FinalDriveRatio * CurrentGearRatio;
		}

		/// <summary>
		/// 値を特定のギアで変速にかける
		/// </summary>
		/// <param name="v">値</param>
		/// <param name="gear">ギア番号</param>
		/// <returns>変速した値</returns>
		private float Transmission(float v, int gear)
		{
			return v * FinalDriveRatio * GearRatio(gear);
		}

		/// <summary>
		/// 値を逆変速にかける
		/// </summary>
		/// <param name="v">値</param>
		/// <returns>逆変速した値</returns>
		private float InverseTransmission(float v)
		{
			return v / FinalDriveRatio / CurrentGearRatio;
		}

		/// <summary>
		/// 現在の回転数で最大トルクを発生するギアを返す（ドライブレンジにないときは現在のギアを返す）
		/// </summary>
		/// <returns>最大トルクを発生するギア番号または現在のギア番号</returns>
		public int MaxTorqueGear()
		{
			if (Gear != 0 && Gear != -1)
			{
				int max = 0;
				float maxTorque = 0f;
				for (int g = 1; g <= TopGear; g++)
				{
					float torque = InverseTransmission(Transmission(CalculatePlainTorqueAt(Transmission(InverseTransmission(RPM), g)), g));
					if (maxTorque < torque)
					{
						max = g;
						maxTorque = torque;
					}
				}
				return max;
			}
			else
			{
				return Gear;
			}
		}
	}
}
