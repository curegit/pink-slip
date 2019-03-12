using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 車両の入力インターフェイス
	/// </summary>
	[RequireComponent(typeof(Powertrain), typeof(Suspension), typeof(Body)), DisallowMultipleComponent]
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
		[SerializeField]
		private float antiLockBrakeEpsilon = 0.2f;

		/// <summary>
		/// 摩擦極大点のスリップに対してABSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float antiLockBrakeSlipMargin = 0.3f;

		/// <summary>
		/// ABS使用時のブレーキ踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float antiLockBrakeMaxStepDelta = 0.6f;

		/// <summary>
		/// TCSを有効にするかどうかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool useTractionControl = true;

		/// <summary>
		/// TCSのよるアクセルの戻し率のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float tractionControlEpsilon = 0.1f;

		/// <summary>
		/// 摩擦極大点のスリップに対してTCSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float tractionControlSlipMargin = 0.2f;

		/// <summary>
		/// TCS使用時のアクセル踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float tractionControlMaxGasDelta = 0.3f;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private bool limitSteerRate = true;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private float limitedMaxSteerRate = 0.2f;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private float limitedSteerRateSpeed = 90f;







		private float gas;

		private float brake;

		private float handBrake;

		private int gear;

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
		/// 
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
		/// 
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
		/// 
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
		/// 
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

		public float AdjustedGas { get; private set; }

		public float AdjustedBrake { get; private set; }


		public float AdjustedSteerRate { get; private set; }


		private Body Body { get; set; }

		private Powertrain Powertrain { get; set; }

		private Suspension Suspension { get; set; }

		private Rigidbody Rigidbody { get; set; }



		public void ShiftUp()
		{

		}

		public void ShiftDown()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		private void Start()
		{
			Body = GetComponent<Body>();
			Powertrain = GetComponent<Powertrain>();
			Suspension = GetComponent<Suspension>();
			Rigidbody = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// 
		/// </summary>
		private void FixedUpdate()
		{
			// 
			AdjustGas();
			//
			AdjustBrake();

			AdjustSteerRate();


			// 
			Powertrain.Throttle = AdjustedGas;

			Powertrain.Gear = Mathf.Min(Gear, Powertrain.TopGear);

			Suspension.Brake = AdjustedBrake;

			Suspension.HandBrake = HandBrake;

			Suspension.SteerRate = AdjustedSteerRate;



		}

		/// <summary>
		/// 
		/// </summary>
		private void AdjustGas()
		{

			if (UseTractionControl)
			{


				// 駆動輪のみ？
				float accelerationSlip = Mathf.Max(Suspension.ForwardSlip, 0f);

				//
				float extremumSlip = Suspension.ForwardExtremumSlip * (1f - TractionControlSlipMargin);

				float tcsGas = AdjustedGas * (1f - TractionControlEpsilon);

				if (accelerationSlip > extremumSlip)
				{
					//
					if (Gas < tcsGas)
					{
						AdjustedGas = Gas;
					}
					//
					else
					{
						Debug.Log("TCS");
						AdjustedGas = tcsGas;
					}
				}
				//
				else
				{
					//
					if (Gas < AdjustedGas)
					{
						AdjustedGas = Gas;
					}
					//
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
		/// 
		/// </summary>
		private void AdjustBrake()
		{
			// ABSがついている場合はスリップをタイヤ摩擦の線形領域に収める制御を行う
			if (UseAntiLockBrake)
			{
				//const float eps = 0.3f;
				//const float maxBrakeDelta = 1f;

				//
				float brakingSlip = -Mathf.Min(Suspension.ForwardSlip, 0f);
				//
				float extremumSlip = Suspension.ForwardExtremumSlip * (1f - AntiLockBrakeSlipMargin);
				//
				float absBrake = AdjustedBrake * (1f - AntiLockBrakeEpsilon);
				//
				if (brakingSlip > extremumSlip)
				{
					//
					if (Brake < absBrake)
					{
						AdjustedBrake = Brake;
					}
					//
					else
					{
						Debug.Log("ABS");
						AdjustedBrake = absBrake;
					}
				}
				//
				else
				{
					//
					if (Brake < AdjustedBrake)
					{
						AdjustedBrake = Brake;
					}
					//
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
		public void AdjustSteerRate()
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
