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
		private bool antiLockBrake = true;

		/// <summary>
		/// ABSのよるブレーキの戻し率のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float antiLockBrakeDelta = 0.2f;

		/// <summary>
		/// 摩擦極大点のスリップに対してABSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float antiLockBrakeSlipMargin = 0.3f;

		/// <summary>
		/// ABS使用時のブレーキ踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxBrakeDelta = 0.6f;

		/// <summary>
		/// TCSを有効にするかどうかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool tractionControl = true;

		/// <summary>
		/// TCSのよるアクセルの戻し率のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float tractionControlDelta = 0.1f;

		/// <summary>
		/// 摩擦極大点のスリップに対してTCSが目標とするスリップの割合のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float tractionControlSlipMargin = 0.1f;

		/// <summary>
		/// TCS使用時のアクセル踏み戻しの割合制限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxGasDelta = 0.2f;



		[SerializeField]
		private float steerLimit = 0.3f;
		[SerializeField]
		private float steerLimitSpeed = 70f;
		public float SteerLimit => steerLimit;
		public float SteerLimitSpeed => steerLimitSpeed;



		private float gas;

		private float brake;

		private float handBrake;

		private int gear;

		private float steerRate;

		/// <summary>
		/// ABSを有効にするかどうか
		/// </summary>
		public bool AntiLockBrake
		{
			get
			{
				return antiLockBrake;
			}
			set
			{
				antiLockBrake = value;
			}
		}

		/// <summary>
		/// ABSのよるブレーキの戻し率
		/// </summary>
		public float AntiLockBrakeDelta
		{
			get
			{
				return antiLockBrakeDelta;
			}
			set
			{
				antiLockBrakeDelta = Mathf.Clamp01(value);
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
		public float MaxBrakeDelta
		{
			get
			{
				return maxBrakeDelta;
			}
			set
			{
				maxBrakeDelta = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// TCSを有効にするかどうか
		/// </summary>
		public bool TractionControl
		{
			get
			{
				return tractionControl;
			}
			set
			{
				tractionControl = value;
			}
		}

		/// <summary>
		/// TCSのよるアクセルの戻し率
		/// </summary>
		public float TractionControlDelta
		{
			get
			{
				return tractionControlDelta;
			}
			set
			{
				tractionControlDelta = Mathf.Clamp01(value);
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
		public float MaxGasDelta
		{
			get
			{
				return maxBrakeDelta;
			}
			set
			{
				maxBrakeDelta = Mathf.Clamp01(value);
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


			AdjustedSteerRate = Mathf.Lerp(SteerRate, SteerRate * SteerLimit, Mathf.InverseLerp(0f, SteerLimitSpeed, Mathf.Abs(Body.ForwardVelocity)));



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

			if (TractionControl)
			{


				// 駆動輪のみ？
				float accelerationSlip = Mathf.Max(Suspension.ForwardSlip, 0f);

				//
				float extremumSlip = Suspension.ForwardExtremumSlip * (1f - TractionControlSlipMargin);

				float tcsGas = AdjustedGas * (1f - TractionControlDelta);

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
						AdjustedGas = Mathf.MoveTowards(AdjustedGas, Gas, MaxGasDelta);
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
			if (AntiLockBrake)
			{
				//const float eps = 0.3f;
				//const float maxBrakeDelta = 1f;

				//
				float brakingSlip = -Mathf.Min(Suspension.ForwardSlip, 0f);
				//
				float extremumSlip = Suspension.ForwardExtremumSlip * (1f - AntiLockBrakeSlipMargin);
				//
				float absBrake = AdjustedBrake * (1f - AntiLockBrakeDelta);
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
						AdjustedBrake = Mathf.MoveTowards(AdjustedBrake, Brake, MaxBrakeDelta);
					}
				}
			}
			// ABSがない場合は入力をそのまま使用する
			else
			{
				AdjustedBrake = Brake;
			}
		}
	}
}
