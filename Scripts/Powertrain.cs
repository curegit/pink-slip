using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Speedcar
{
	[RequireComponent(typeof(Suspension)), DisallowMultipleComponent]
	public class Powertrain : MonoBehaviour
	{
		[SerializeField]
		private TorqueCurveModel torqueCurveModel = TorqueCurveModel.Parametric;




		[SerializeField]
		public AnimationCurve torqueAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1000, 10), new Keyframe(5000, 600), new Keyframe(7800, 800), new Keyframe(8000, 0));

		[SerializeField, Range(0f, 5f)]
		private float torqueMultiplier = 1f;

		[SerializeField, Range(100f, 3000f)]
		private float idlingRPM = 1000f;

		[SerializeField]
		private float redline = 7000f;

		[SerializeField]
		private float revLimit = 8000f;

		[SerializeField]
		private float maxRPM = 10000f;

		[SerializeField]
		private float engineInertia = 0.1f;

		[SerializeField]
		private float engineDampingRate = 0.5f;

		[SerializeField]
		private float wheelDampingRate = 0.08f;

		[SerializeField]
		private bool forcedInduction;

		[SerializeField]
		private ForcedInduction forcedInductionDevice = ForcedInduction.Turbocharger;

		[SerializeField]
		private bool nitrous;

		[SerializeField]
		private float nos;



		//public Func<float, float> TorqueCurveOverrideFunction { get; set; } = null;


		public float RPM { get; private set; }

		public float Torque { get; private set; }

		public float Pressure { get; private set; }



		[SerializeField, Range(0.1f, 5.0f)]
		private float[] gearRatios = { 3.2f, 1.9f, 1.3f, 1.0f, 0.7f, 0.5f };

		[SerializeField, Range(0.1f, 5.0f)]
		private float reverseGearRatio = 2.9f;

		[SerializeField, Range(0.1f, 5.0f)]
		private float finalDriveRatio = 3.7f;


		[SerializeField]
		private Drivetrain drivetrain = Drivetrain.AWD;

		[SerializeField]
		private float frontDifferentialLocking = 0f;

		[SerializeField]
		private float rearDifferentialLocking = 0f;

		[SerializeField]
		private float centerDifferentialLocking = 0f;

		[SerializeField]
		private float centerDifferentialBalance = 0.5f;




		private Suspension Suspension { get; set; }


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
				gearRatios = value.ToArray();
			}
		}

		public float ReverseGearRatio
		{
			get
			{
				return reverseGearRatio;
			}
			set
			{

			}
		}

		public float FinalDriveRatio
		{
			get
			{
				return finalDriveRatio;
			}
			set
			{

			}
		}


		public float CurrentGearRatio
		{
			get
			{
				if (gear == 0)
				{
					return 0f;
				}
				else if (gear == -1)
				{
					return reverseGearRatio;
				}
				else
				{
					return gearRatios[gear - 1];
				}
			}
		}

		public int TopGear => GearRatios.Count();


		private int gear = 1;

		public int Gear
		{
			get
			{
				return gear;
			}
			set
			{
				gear = Mathf.Clamp(value, -1, gearRatios.Length);
			}
		}

		private float throttle;

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

		private bool nitrousEmission;

		// Use this for initialization
		private void Start()
		{
			Suspension = GetComponent<Suspension>();
		}

		// Update is called once per frame
		private void FixedUpdate()
		{
			UpdatePressure();
			UpdateRPM();
			UpdateTorque();
			ApplyTorque();
			ApplyWheelDamping();
		}

		private void UpdatePressure()
		{

		}


		private void UpdateRPM()
		{
			// 
			if (gear != 0)
			{
				// 
				float wheelRPM = 0f;
				switch (drivetrain)
				{
					// 
					case Drivetrain.FWD:
						wheelRPM = Suspension.FrontWheelColliders.Average(x => x.rpm);
						break;
					// 
					case Drivetrain.RWD:
						wheelRPM = Suspension.RearWheelColliders.Average(x => x.rpm);
						break;
					// 
					case Drivetrain.AWD:
						float frontWheelRPM = Suspension.FrontWheelColliders.Average(x => x.rpm);
						float rearWheelRPM = Suspension.RearWheelColliders.Average(x => x.rpm);
						wheelRPM = (frontWheelRPM + rearWheelRPM) / 2f;
						break;
				}
				// 
				float rpm = Transmission(wheelRPM);


				// TODO
				rpm = Mathf.Lerp(rpm, RPM, 0.5f);

				// TODO
				RPM = Mathf.Clamp(rpm, idlingRPM, maxRPM);
			}
			// 
			else
			{
				// 
				float rpm = RPM + Torque / engineInertia * Time.fixedDeltaTime / (2f * Mathf.PI);

				// 負圧 TODO
				rpm = rpm - 0f;

				// 抵抗
				rpm = rpm * (1 - Time.fixedDeltaTime * engineDampingRate);

				RPM = Mathf.Clamp(rpm, idlingRPM, maxRPM);
			}
		}

		
		private void UpdateTorque()
		{
			//
			float torque = throttle * CalculateTorqueAt(RPM);
			// 過給機による増幅を
			if (forcedInduction)
			{

			}
			// ナイトロ
			if (nitrous)
			{

			}
			// エンジンからの出力トルクをプロパティに保存
			Torque = torque;
		}

		/// <summary>
		/// 
		/// </summary>
		private void ApplyTorque()
		{
			// 変速機を通してトルクを変換
			float torque = Transmission(Torque);
			// 駆動方式の違いよって構成が異なるデフを通じてトルクを伝達
			switch (drivetrain)
			{
				// 前輪駆動の場合
				case Drivetrain.FWD:
					// 前輪のデフを通してトルクを伝達
					float frontLeftRPM = Math.Abs(Suspension.FrontLeftWheelCollider.rpm);
					float frontRightRPM = Mathf.Abs(Suspension.FrontRightWheelCollider.rpm);
					float frontLeftOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? torque * frontLeftRPM / (frontLeftRPM + frontRightRPM) : torque / 2f;
					float frontRightOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? torque * frontRightRPM / (frontLeftRPM + frontRightRPM) : torque / 2f;
					float frontLeftLockedTorque = torque / 2f;
					float frontRightLockedTorque = torque / 2f;
					Suspension.FrontLeftWheelCollider.motorTorque = Mathf.Lerp(frontLeftOpenTorque, frontLeftLockedTorque, frontDifferentialLocking);
					Suspension.FrontRightWheelCollider.motorTorque = Mathf.Lerp(frontRightOpenTorque, frontRightLockedTorque, frontDifferentialLocking);
					// 後輪に動力は伝わらない
					foreach (var wheelCollider in Suspension.RearWheelColliders)
					{
						wheelCollider.motorTorque = 0f;
					}
					break;
				// 後輪駆動の場合
				case Drivetrain.RWD:
					// 後輪のデフを通してトルクを伝達
					float rearLeftRPM = Math.Abs(Suspension.RearLeftWheelCollider.rpm);
					float rearRightRPM = Mathf.Abs(Suspension.RearRightWheelCollider.rpm);
					float rearLeftOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? torque * rearLeftRPM / (rearLeftRPM + rearRightRPM) : torque / 2f;
					float rearRightOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? torque * rearRightRPM / (rearLeftRPM + rearRightRPM) : torque / 2f;
					float rearLeftLockedTorque = torque / 2f;
					float rearRightLockedTorque = torque / 2f;
					Suspension.RearLeftWheelCollider.motorTorque = Mathf.Lerp(rearLeftOpenTorque, rearLeftLockedTorque, rearDifferentialLocking);
					Suspension.RearRightWheelCollider.motorTorque = Mathf.Lerp(rearRightOpenTorque, rearRightLockedTorque, rearDifferentialLocking);
					// 前輪に動力は伝わらない
					foreach (var wheelCollider in Suspension.FrontWheelColliders)
					{
						wheelCollider.motorTorque = 0f;
					}
					break;
				// 四輪駆動の場合
				case Drivetrain.AWD:
					// センターデフの動作を再現
					float frontRPM = Math.Abs(Suspension.FrontWheelColliders.Average(x => x.rpm));
					float rearRPM = Math.Abs(Suspension.RearWheelColliders.Average(x => x.rpm));
					float frontOpenTorque = (frontRPM + rearRPM > 1f) ? torque * frontRPM / (frontRPM + rearRPM) : torque / 2f;
					float rearOpenTorque = (frontRPM + rearRPM > 1f) ? torque * rearRPM / (frontRPM + rearRPM) : torque / 2f;
					float frontLockedTorque = Mathf.Lerp(torque, 0f, centerDifferentialBalance);
					float rearLockedTorque = Mathf.Lerp(0f, torque, centerDifferentialBalance);
					float frontTorque = Mathf.Lerp(frontOpenTorque, frontLockedTorque, centerDifferentialLocking);
					float rearTorque = Mathf.Lerp(rearOpenTorque, rearLockedTorque, centerDifferentialLocking);
					// 前輪のデフを通してトルクを伝達
					frontLeftRPM = Math.Abs(Suspension.FrontLeftWheelCollider.rpm);
					frontRightRPM = Mathf.Abs(Suspension.FrontRightWheelCollider.rpm);
					frontLeftOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? frontTorque * frontLeftRPM / (frontLeftRPM + frontRightRPM) : frontTorque / 2f;
					frontRightOpenTorque = (frontLeftRPM + frontRightRPM > 1f) ? frontTorque * frontRightRPM / (frontLeftRPM + frontRightRPM) : frontTorque / 2f;
					frontLeftLockedTorque = frontTorque / 2f;
					frontRightLockedTorque = frontTorque / 2f;
					Suspension.FrontLeftWheelCollider.motorTorque = Mathf.Lerp(frontLeftOpenTorque, frontLeftLockedTorque, frontDifferentialLocking);
					Suspension.FrontRightWheelCollider.motorTorque = Mathf.Lerp(frontRightOpenTorque, frontRightLockedTorque, frontDifferentialLocking);
					// 後輪のデフを通してトルクを伝達
					rearLeftRPM = Math.Abs(Suspension.RearLeftWheelCollider.rpm);
					rearRightRPM = Mathf.Abs(Suspension.RearRightWheelCollider.rpm);
					rearLeftOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? rearTorque * rearLeftRPM / (rearLeftRPM + rearRightRPM) : rearTorque / 2f;
					rearRightOpenTorque = (rearLeftRPM + rearRightRPM > 1f) ? rearTorque * rearRightRPM / (rearLeftRPM + rearRightRPM) : rearTorque / 2f;
					rearLeftLockedTorque = rearTorque / 2f;
					rearRightLockedTorque = rearTorque / 2f;
					Suspension.RearLeftWheelCollider.motorTorque = Mathf.Lerp(rearLeftOpenTorque, rearLeftLockedTorque, rearDifferentialLocking);
					Suspension.RearRightWheelCollider.motorTorque = Mathf.Lerp(rearRightOpenTorque, rearRightLockedTorque, rearDifferentialLocking);
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void ApplyWheelDamping()
		{
			// 
			if (gear != 0)
			{
				//
				float dampingByEngineBraking = 0f;

				// 
				float dampingByEngineFriction = 0f;

				float totalDamping = wheelDampingRate + dampingByEngineBraking + dampingByEngineFriction;
				// 
				switch (drivetrain)
				{
					// 
					case Drivetrain.FWD:
						foreach (var wheelCollider in Suspension.FrontWheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						foreach (var wheelCollider in Suspension.RearWheelColliders)
						{
							wheelCollider.wheelDampingRate = wheelDampingRate;
						}
						break;
					// 
					case Drivetrain.RWD:
						foreach (var wheelCollider in Suspension.FrontWheelColliders)
						{
							wheelCollider.wheelDampingRate = wheelDampingRate;
						}
						foreach (var wheelCollider in Suspension.RearWheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						break;
					// 
					case Drivetrain.AWD:
						foreach (var wheelCollider in Suspension.WheelColliders)
						{
							wheelCollider.wheelDampingRate = totalDamping;
						}
						break;
				}
			}
			// 
			else
			{
				// 
				foreach (var wheelCollider in Suspension.WheelColliders)
				{
					wheelCollider.wheelDampingRate = wheelDampingRate;
				}
			}
		}

		private float CalculateTorqueAt(float rpm)
		{
			float torque = 0f;
			switch (torqueCurveModel)
			{
				case TorqueCurveModel.Parametric:

					/*TODO*/
					torque = 0f;
					break;
				case TorqueCurveModel.AnimationCurve:
					torque = torqueAnimationCurve != null && torqueAnimationCurve.length > 0 ? torqueAnimationCurve.Evaluate(rpm) : 0f;
					break;
			}
			return torque * torqueMultiplier;
		}

		private float Transmission(float v)
		{
			return v * finalDriveRatio * CurrentGearRatio;
		}

		private float Transmission(float v, int gear)
		{
			return v * finalDriveRatio * gearRatios[gear - 1];
		}

		public int MaxTorqueGear()
		{
			int max = 0;
			float maxTorque = 0f;
			for (int g = 1; g <= gearRatios.Length; g++)
			{
				float torque = Transmission(RPM, g);
				if (maxTorque < torque)
				{
					max = g;
					maxTorque = torque;
				}
			}
			return max;
		}
	}
}
