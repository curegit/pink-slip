using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(Powertrain), typeof(Suspension), typeof(Body)), DisallowMultipleComponent]
	public class CarController : MonoBehaviour
	{
		
		[SerializeField]
		private bool antiLockBrake = true;

		[SerializeField]
		private float antiLockBrakeDelta = 0.2f;

		[SerializeField]
		private float antiLockBrakeSlipMargin = 0.3f;

		[SerializeField]
		private float maxBrakeDelta = 0.6f;

		/*
		[SerializeField]
		private bool launchControl = true;

		[SerializeField]
		private float launchSpeedThreshold= 10f;
		*/
		[SerializeField]
		private bool tractionControl = true;
		


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
		/// 
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
		/// 
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
		/// 
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
		/// 
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


		/*
		/// <summary>
		/// 
		/// </summary>
		public bool LaunchControl
		{
			get
			{
				return launchControl;
			}
			set
			{
				launchControl = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float LauchSpeedThreshold
		{
			get
			{
				return launchSpeedThreshold;
			}
			set
			{
				launchSpeedThreshold = Mathf.Max(value, 0f);
			}
		}
		*/

		/// <summary>
		/// 
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
				AdjustedGas = Gas;

				const float eps = 0.2f;
				//float speed = Rigidbody.transform.InverseTransformDirection(Rigidbody.velocity).z;


				// 駆動輪のみ？
				float accelerationSlip = Mathf.Max(Suspension.ForwardSlip, 0f);

				//
				float extremumSlip = Suspension.ForwardExtremumSlip / Mathf.Sqrt(2f);

				AdjustedGas = Gas;
			}
			// TCS/LCSがない場合は入力をそのまま使用する
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
