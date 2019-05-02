using System.Linq;
using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 車両の入力インターフェイス
	/// </summary>
	[RequireComponent(typeof(Powertrain), typeof(Chassis)), RequireComponent(typeof(Body), typeof(Rigidbody)), DisallowMultipleComponent]
	public class CarController : MonoBehaviour
	{
		/// <summary>
		/// ABSを有効にするかどうかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool useAntiLockBrake = true;

		/// <summary>
		/// ABSのよるブレーキの戻し率のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float antiLockBrakeEpsilon = 0.2f;

		/// <summary>
		/// 摩擦極大点のスリップに対してABSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float antiLockBrakeSlipMargin = 0.5f;

		/// <summary>
		/// ABS使用時のブレーキ踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float antiLockBrakeMaxStepDelta = 0.6f;

		/// <summary>
		/// TCSを有効にするかどうかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool useTractionControl = true;

		/// <summary>
		/// TCSのよるアクセルの戻し率のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float tractionControlEpsilon = 0.1f;

		/// <summary>
		/// 摩擦極大点のスリップに対してTCSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float tractionControlSlipMargin = 0.3f;

		/// <summary>
		/// TCS使用時のアクセル踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float tractionControlMaxGasDelta = 0.4f;

		/// <summary>
		/// ステアリング範囲を速さによって制限するかどうかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool limitSteerRate = true;

		/// <summary>
		/// 最大制限時のステアリング範囲の割合のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float limitedMaxSteerRate = 0.2f;

		/// <summary>
		/// ステアリング範囲の制限が最大になる速さのバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float limitedSteerRateSpeed = 90f;

		/// <summary>
		/// 生のアクセル入力のバッキングフィールド
		/// </summary>
		private float gas;

		/// <summary>
		/// 生のブレーキ入力のバッキングフィールド
		/// </summary>
		private float brake;

		/// <summary>
		/// ハンドブレーキ入力のバッキングフィールド
		/// </summary>
		private float handBrake;

		/// <summary>
		/// ギアの入力のバッキングフィールド
		/// </summary>
		private int gear;

		/// <summary>
		/// 生のステアリング割合の入力のバッキングフィールド
		/// </summary>
		private float steerRate;

		/// <summary>
		/// ABSを有効にするかどうか
		/// </summary>
		public bool UseAntiLockBrake
		{
			get
			{
				return useAntiLockBrake;
			}
			set
			{
				useAntiLockBrake = value;
			}
		}

		/// <summary>
		/// ABSのよるブレーキの戻し率
		/// </summary>
		public float AntiLockBrakeEpsilon
		{
			get
			{
				return antiLockBrakeEpsilon;
			}
			set
			{
				antiLockBrakeEpsilon = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 摩擦極大点のスリップに対してABSが目標とするスリップの割合
		/// </summary>
		public float AntiLockBrakeSlipMargin
		{
			get
			{
				return antiLockBrakeSlipMargin;
			}
			set
			{
				antiLockBrakeSlipMargin = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// ABS使用時のブレーキ踏み戻しの割合制限
		/// </summary>
		public float AntiLockBrakeMaxStepDelta
		{
			get
			{
				return antiLockBrakeMaxStepDelta;
			}
			set
			{
				antiLockBrakeMaxStepDelta = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// TCSを有効にするかどうか
		/// </summary>
		public bool UseTractionControl
		{
			get
			{
				return useTractionControl;
			}
			set
			{
				useTractionControl = value;
			}
		}

		/// <summary>
		/// TCSのよるアクセルの戻し率
		/// </summary>
		public float TractionControlEpsilon
		{
			get
			{
				return tractionControlEpsilon;
			}
			set
			{
				tractionControlEpsilon = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 摩擦極大点のスリップに対してTCSが目標とするスリップの割合
		/// </summary>
		public float TractionControlSlipMargin
		{
			get
			{
				return tractionControlSlipMargin;
			}
			set
			{
				tractionControlSlipMargin = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// TCS使用時のアクセル踏み戻しの割合制限
		/// </summary>
		public float TractionControlMaxGasDelta
		{
			get
			{
				return tractionControlMaxGasDelta;
			}
			set
			{
				tractionControlMaxGasDelta = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// ステアリング範囲を速度によって制限するかどうか
		/// </summary>
		public bool LimitSteerRate
		{
			get
			{
				return limitSteerRate;
			}
			set
			{
				limitSteerRate = value;
			}
		}

		/// <summary>
		/// 最大制限時のステアリング範囲の割合
		/// </summary>
		public float LimitedMaxSteerRate
		{
			get
			{
				return limitedMaxSteerRate;
			}
			set
			{
				limitedMaxSteerRate = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// ステアリング範囲の制限が最大になる速さ
		/// </summary>
		public float LimitedSteerRateSpeed
		{
			get
			{
				return limitedSteerRateSpeed;
			}
			set
			{
				limitedSteerRateSpeed = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 生のアクセル入力
		/// </summary>
		public float Gas
		{
			private get
			{
				return gas;
			}
			set
			{
				gas = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 生のブレーキ入力
		/// </summary>
		public float Brake
		{
			private get
			{
				return brake;
			}
			set
			{
				brake = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// ハンドブレーキ入力
		/// </summary>
		public float HandBrake
		{
			private get
			{
				return handBrake;
			}
			set
			{
				handBrake = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// ギアの入力
		/// </summary>
		public int Gear
		{
			private get
			{
				return gear;
			}
			set
			{
				gear = Mathf.Max(value, -1);
			}
		}

		/// <summary>
		/// 生のステアリング割合の入力
		/// </summary>
		public float SteerRate
		{
			private get
			{
				return steerRate;
			}
			set
			{
				steerRate = Mathf.Clamp(value, -1f, 1f);
			}
		}

		/// <summary>
		/// 調節済みのアクセル入力
		/// </summary>
		public float AdjustedGas { get; private set; }

		/// <summary>
		/// 調節済みのブレーキ入力
		/// </summary>
		public float AdjustedBrake { get; private set; }

		/// <summary>
		/// 調節済みのステアリング割合の入力
		/// </summary>
		public float AdjustedSteerRate { get; private set; }

		/// <summary>
		/// 車体コンポーネント
		/// </summary>
		private Body Body { get; set; }

		/// <summary>
		/// 動力系コンポーネント
		/// </summary>
		private Powertrain Powertrain { get; set; }

		/// <summary>
		/// 足回りのコンポーネント
		/// </summary>
		private Chassis Chassis { get; set; }

		/// <summary>
		/// 剛体コンポーネント
		/// </summary>
		private Rigidbody Rigidbody { get; set; }

		/// <summary>
		/// 初期化時に呼ばれるメソッド
		/// </summary>
		private void Start()
		{
			Body = GetComponent<Body>();
			Powertrain = GetComponent<Powertrain>();
			Chassis = GetComponent<Chassis>();
			Rigidbody = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// 毎物理フレームに呼ばれるメソッド
		/// </summary>
		private void FixedUpdate()
		{
			// 各種入力を調節する
			AdjustGas();
			AdjustBrake();
			AdjustSteerRate();
			// 各コンポーネントに入力を伝達
			Powertrain.Throttle = AdjustedGas;
			Powertrain.Gear = Mathf.Min(Gear, Powertrain.TopGear);
			Chassis.Brake = AdjustedBrake;
			Chassis.HandBrake = HandBrake;
			Chassis.SteerRate = AdjustedSteerRate;
		}

		/// <summary>
		/// アクセルを調節する
		/// </summary>
		private void AdjustGas()
		{
			// TCSが有効な場合はスリップを抑えるようにをアクセルを調節する
			if (UseTractionControl)
			{
				// 駆動輪の接地情報を取得
				var poweredWheelHits = Chassis.WheelHits;
				if (Powertrain.Drivetrain == Drivetrain.FrontWheelDrive) poweredWheelHits = Chassis.FrontWheelHits;
				if (Powertrain.Drivetrain == Drivetrain.RearWheelDrive) poweredWheelHits = Chassis.RearWheelHits;
				// 加速による駆動輪のスリップを求める
				var hits = poweredWheelHits.Where(h => h.HasValue);
				float forwardSlip = hits.Any() ? hits.Average(h => h.Value.forwardSlip) : 0f;
				float accelerationSlip = Mathf.Abs(forwardSlip);
				// 駆動輪を取得
				var poweredWheels = Chassis.WheelColliders;
				if (Powertrain.Drivetrain == Drivetrain.FrontWheelDrive) poweredWheels = Chassis.FrontWheelColliders;
				if (Powertrain.Drivetrain == Drivetrain.RearWheelDrive) poweredWheels = Chassis.RearWheelColliders;
				// 目標スリップを求める
				float forwardExtremumSlip = poweredWheels.Average(w => w.forwardFriction.extremumSlip);
				float targetSlip = forwardExtremumSlip * (1f - TractionControlSlipMargin);
				// TCSを適用した場合の補正値を求める
				float tcsGas = AdjustedGas * (1f - TractionControlEpsilon);
				// スリップが目標より多い場合
				if (accelerationSlip > targetSlip)
				{
					// 入力がTCS補正値より小さければそのまま
					if (Gas < tcsGas)
					{
						AdjustedGas = Gas;
					}
					// 入力がTCS補正値以上ならばTCSを作動
					else
					{
						AdjustedGas = tcsGas;
					}
				}
				// スリップが目標以下である場合
				else
				{
					// 入力が直前のものより小さければ直ちに従う
					if (Gas < AdjustedGas)
					{
						AdjustedGas = Gas;
					}
					// 入力が直前のもの以上ならば踏み込みを制限する
					else
					{
						AdjustedGas = Mathf.MoveTowards(AdjustedGas, Gas, TractionControlMaxGasDelta);
					}
				}
			}
			// TCSがない場合は入力をそのまま使用する
			else
			{
				AdjustedGas = Gas;
			}
		}

		/// <summary>
		/// ブレーキを調節する
		/// </summary>
		private void AdjustBrake()
		{
			// ABSがついている場合はスリップをタイヤ摩擦の線形領域に収める制御を行う
			if (UseAntiLockBrake)
			{
				// ブレーキングによるスリップを求める
				var hits = Chassis.WheelHits.Where(h => h.HasValue);
				float brakingSlip = Mathf.Abs(hits.Any() ? hits.Average(h => h.Value.forwardSlip) : 0f);
				// 目標スリップを求める
				float forwardExtremumSlip = Chassis.WheelColliders.Average(w => w.forwardFriction.extremumSlip);
				float targetSlip = forwardExtremumSlip * (1f - AntiLockBrakeSlipMargin);
				// ABSを適用した場合の補正値を求める
				float absBrake = AdjustedBrake * (1f - AntiLockBrakeEpsilon);
				// スリップが目標より多い場合
				if (brakingSlip > targetSlip)
				{
					// 入力がABS補正値より小さければそのまま
					if (Brake < absBrake)
					{
						AdjustedBrake = Brake;
					}
					// 入力がABS補正値以上ならばABSを作動
					else
					{
						AdjustedBrake = absBrake;
					}
				}
				// スリップが目標以下である場合
				else
				{
					// 入力が直前のものより小さければ直ちに従う
					if (Brake < AdjustedBrake)
					{
						AdjustedBrake = Brake;
					}
					// 入力が直前のもの以上ならば踏み込みを制限する
					else
					{
						AdjustedBrake = Mathf.MoveTowards(AdjustedBrake, Brake, AntiLockBrakeMaxStepDelta);
					}
				}
			}
			// ABSがない場合は入力をそのまま使用する
			else
			{
				AdjustedBrake = Brake;
			}
		}

		/// <summary>
		/// ステアリング角度を調節する
		/// </summary>
		private void AdjustSteerRate()
		{
			if (LimitSteerRate)
			{
				AdjustedSteerRate = Mathf.Lerp(SteerRate, SteerRate * LimitedMaxSteerRate, Mathf.InverseLerp(0f, LimitedSteerRateSpeed, Mathf.Abs(Body.ForwardVelocity)));
			}
			else
			{
				AdjustedSteerRate = SteerRate;
			}
		}
	}
}
