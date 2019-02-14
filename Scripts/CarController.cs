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
		private bool launchControl = true;

		[SerializeField]
		private float launchSpeedThreshold= 10f;

		[SerializeField]
		private bool tractionControl = true;

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

		private Powertrain Powertrain { get; set; }

		private Suspension Suspension { get; set; }

		private Rigidbody Rigidbody { get; set; }

		/// <summary>
		/// 
		/// </summary>
		private void Start()
		{
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
			// 
			Powertrain.Throttle = AdjustedGas;

			Powertrain.Gear = Mathf.Min(Gear, Powertrain.TopGear);

			Suspension.Brake = AdjustedBrake;

			Suspension.HandBrake = HandBrake;

			Suspension.SteerRate = SteerRate;
		}

		/// <summary>
		/// 
		/// </summary>
		private void AdjustGas()
		{
			const float eps = 0.1f;
			float speed = Rigidbody.transform.InverseTransformDirection(Rigidbody.velocity).z;


			// 駆動輪のみ？
			float accelerationSlip = Mathf.Max(-Suspension.ForwardSlip, 0f);

			// 
			if (LaunchControl && speed < LauchSpeedThreshold)
			{


				AdjustedGas = Gas;
			}
			// 
			else if (TractionControl)
			{


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
			// ABSがついている場合はスリップを摩擦係数極大点に合わせる制御を行う
			if (AntiLockBrake)
			{
				const float eps = 0.1f;
				const float maxBrakeDelta = 0.25f;

				//
				float brakingSlip = Mathf.Max(Suspension.ForwardSlip, 0f);
				//
				float extremumSlip = Suspension.ForwardExtremumSlip;
				//
				float absBrake = AdjustedBrake * (1f - eps);
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
						AdjustedBrake = Mathf.MoveTowards(AdjustedBrake, Brake, maxBrakeDelta);
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
