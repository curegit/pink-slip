using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Speedcar
{
	[RequireComponent(typeof(Body), typeof(Rigidbody)), DisallowMultipleComponent]
	public class Chassis : MonoBehaviour
	{

		[SerializeField]
		private WheelCollider frontLeftWheelCollider;

		[SerializeField]
		private WheelCollider frontRightWheelCollider;

		[SerializeField]
		private WheelCollider rearLeftWheelCollider;

		[SerializeField]
		private WheelCollider rearRightWheelCollider;




		[SerializeField]
		private float frontNaturalFrequency = 1.5f;

		[SerializeField]
		private float rearNaturalFrequency = 1.5f;

		[SerializeField]
		private float dampingRatio = 0.95f;

		[SerializeField]
		private float forceShift = 0.15f;



		[SerializeField]
		private bool autoConfigureSuspensionDistance;

		[SerializeField]
		private float frontSuspensionDistance = 0.25f;

		[SerializeField]
		private float rearSuspensionDistance = 0.25f;




		[SerializeField]
		private float maxSteerAngle = 30f;

		[SerializeField]
		private float ackermannCoefficient = 1f;

		[SerializeField]
		private float frontToeAngle = 0.1f;

		[SerializeField]
		private float rearToeAngle = 0f;



		[SerializeField]
		private float brakeTorque;

		[SerializeField]
		private float brakeBias = 0.5f;

		[SerializeField]
		private float stabilizerCoefficient;

		[SerializeField]
		private float stabilizerBias = 0.5f;




		[SerializeField]
		private FrictionCurveSet frictionCurveSet;

		public float DriftOrGrip;

		public float StiffnessMultiplier { get; set; }
		public float BrakeBonusStiffness;
		public float SteerDriftBonusStiffness;
		public float NoSteerBonusStiffness;

		[SerializeField]
		private float antiOversteerCoefficient;


		[SerializeField]
		private float substepsSpeedThreshold = 25f;

		[SerializeField]
		private int substepsAboveThreshold = 8;

		[SerializeField]
		private int substepsBelowThreshold = 4;







		private float brake;

		private float handBrake;

		private float steerRate;


		public WheelCollider FrontLeftWheelCollider
		{
			get
			{
				return frontLeftWheelCollider;
			}
			set
			{
				frontLeftWheelCollider = value;
			}
		}

		public WheelCollider FrontRightWheelCollider
		{
			get
			{
				return frontRightWheelCollider;
			}
			set
			{
				frontRightWheelCollider = value;
			}
		}

		public WheelCollider RearLeftWheelCollider
		{
			get
			{
				return rearLeftWheelCollider;
			}
			set
			{
				rearLeftWheelCollider = value;
			}
		}

		public WheelCollider RearRightWheelCollider
		{
			get
			{
				return rearRightWheelCollider;
			}
			set
			{
				rearRightWheelCollider = value;
			}
		}

		public IEnumerable<WheelCollider> WheelColliders
		{
			get
			{
				yield return FrontLeftWheelCollider;
				yield return FrontRightWheelCollider;
				yield return RearLeftWheelCollider;
				yield return RearRightWheelCollider;
			}
		}

		public IEnumerable<WheelCollider> FrontWheelColliders
		{
			get
			{
				yield return FrontLeftWheelCollider;
				yield return FrontRightWheelCollider;
			}
		}

		public IEnumerable<WheelCollider> RearWheelColliders
		{
			get
			{
				yield return RearLeftWheelCollider;
				yield return RearRightWheelCollider;
			}
		}

		public float FrontNaturalFrequency
		{
			get
			{
				return frontNaturalFrequency;
			}
			set
			{
				frontNaturalFrequency = Mathf.Max(value, 0f);
			}
		}

		public float RearNaturalFrequency
		{
			get
			{
				return rearNaturalFrequency;
			}
			set
			{
				rearNaturalFrequency = Mathf.Max(value, 0f);
			}
		}

		public float DampingRatio
		{
			get
			{
				return dampingRatio;
			}
			set
			{
				dampingRatio = Mathf.Max(value, 0f);
			}
		}

		public float ForceShift
		{
			get
			{
				return forceShift;
			}
			set
			{
				forceShift = value;
			}
		}

		public bool AutoConfigureSuspensionDistance
		{
			get
			{
				return autoConfigureSuspensionDistance;
			}
			set
			{
				autoConfigureSuspensionDistance = value;
			}
		}


		public float FrontSuspensionDistance
		{
			get
			{
				return frontSuspensionDistance;
			}
			set
			{
				frontSuspensionDistance = Mathf.Max(value, 0f);
			}
		}

		public float RearSuspensionDistance
		{
			get
			{
				return rearSuspensionDistance;
			}
			set
			{
				rearSuspensionDistance = Mathf.Max(value, 0f);
			}
		}


		public float MaxSteerAngle
		{
			get
			{
				return maxSteerAngle;
			}
			set
			{
				maxSteerAngle = Mathf.Max(value, 0f);
			}
		}



		public float AckermannCoefficient
		{
			get
			{
				return ackermannCoefficient;
			}
			set
			{
				ackermannCoefficient = Mathf.Clamp01(value);
			}
		}


		public float FrontToeAngle
		{
			get
			{
				return frontToeAngle;
			}
			set
			{
				frontToeAngle = Mathf.Max(value, 0f);
			}
		}

		public float RearToeAngle
		{
			get
			{
				return rearToeAngle;
			}
			set
			{
				rearToeAngle = Mathf.Max(value, 0f);
			}
		}


		private float AntiOversteerCoefficient
		{
			get
			{
				return antiOversteerCoefficient;
			}
			set
			{
				antiOversteerCoefficient = Mathf.Max(value, 0f);
			}
		}

		public float BrakeTorque
		{
			get
			{
				return brakeTorque;
			}
			set
			{
				brakeTorque = Mathf.Max(value, 0f);
			}
		}

		public float BrakeBias
		{
			get
			{
				return brakeBias;
			}
			set
			{
				brakeBias = Mathf.Clamp01(value);
			}
		}

		public float StabilizerCoefficient
		{
			get
			{
				return stabilizerCoefficient;
			}
			set
			{
				stabilizerCoefficient = Mathf.Max(value, 0f);
			}
		}


		public float StabilizerBias
		{
			get
			{
				return stabilizerBias;
			}
			set
			{
				stabilizerBias = Mathf.Clamp01(value);
			}
		}




		public FrictionCurveSet FrictionCurveSet
		{
			get
			{
				return frictionCurveSet;
			}
			set
			{
				frictionCurveSet = value;
			}
		}










		public float SubstepsSpeedThreshold
		{
			get
			{
				return substepsSpeedThreshold;
			}
			set
			{
				substepsSpeedThreshold = Mathf.Max(value, 0f);
			}
		}




		public int SubstepsAboveThreshold
		{
			get
			{
				return substepsAboveThreshold;
			}
			set
			{
				substepsAboveThreshold = Mathf.Max(value, 1);
			}
		}



		public int SubstepsBelowThreshold
		{
			get
			{
				return substepsBelowThreshold;
			}
			set
			{
				substepsBelowThreshold = Mathf.Max(value, 1);
			}
		}





	






		public WheelHit? FrontLeftWheelHit { get; private set; }

		public WheelHit? FrontRightWheelHit { get; private set; }

		public WheelHit? RearLeftWheelHit { get; private set; }

		public WheelHit? RearRightWheelHit { get; private set; }

		public IEnumerable<WheelHit?> WheelHits
		{
			get
			{
				yield return FrontLeftWheelHit;
				yield return FrontRightWheelHit;
				yield return RearLeftWheelHit;
				yield return RearRightWheelHit;
			}
		}

		public IEnumerable<WheelHit?> FrontWheelHits
		{
			get
			{
				yield return FrontLeftWheelHit;
				yield return FrontRightWheelHit;
			}
		}

		public IEnumerable<WheelHit?> RearWheelHits
		{
			get
			{
				yield return RearLeftWheelHit;
				yield return RearRightWheelHit;
			}
		}











		public float Brake
		{
			get
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
			get
			{
				return handBrake;
			}
			set
			{
				handBrake = Mathf.Clamp01(value);
			}
		}

		public float SteerRate
		{
			get
			{
				return steerRate;
			}
			set
			{
				steerRate = Mathf.Clamp(value, -1f, 1f);
			}
		}

		public float FrontTrack
		{
			get
			{
				Vector3 left = WheelPosition(FrontLeftWheelCollider);
				Vector3 right = WheelPosition(FrontRightWheelCollider);
				Vector3 diff = Rigidbody.transform.InverseTransformDirection(left - right);
				return Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
			}
		}

		public float RearTrack
		{
			get
			{
				Vector3 left = WheelPosition(RearLeftWheelCollider);
				Vector3 right = WheelPosition(RearRightWheelCollider);
				Vector3 diff = Rigidbody.transform.InverseTransformDirection(left - right);
				return Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
			}
		}

		public float Wheelbase
		{
			get
			{
				Vector3 frontLeft = WheelPosition(FrontLeftWheelCollider);
				Vector3 frontRight = WheelPosition(FrontRightWheelCollider);
				Vector3 rearLeft = WheelPosition(RearLeftWheelCollider);
				Vector3 rearRight = WheelPosition(RearRightWheelCollider);
				Vector3 leftDiff = Rigidbody.transform.InverseTransformDirection(frontLeft - rearLeft);
				Vector3 rightDiff = Rigidbody.transform.InverseTransformDirection(frontRight - rearRight);
				float left = Mathf.Sqrt(leftDiff.y * leftDiff.y + leftDiff.z * leftDiff.z);
				float right = Mathf.Sqrt(rightDiff.y * rightDiff.y + rightDiff.z * rightDiff.z);
				return (left + right) / 2f;
			}
		}


		private Body Body { get; set; }



		private Rigidbody Rigidbody { get; set; }

		// Use this for initialization
		private void Start()
		{
			Body = GetComponent<Body>();
			Rigidbody = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		private void Update()
		{
			AdjustSprings();
			AdjustDampers();
			AdjustForcePoints();
			ConfigureSuspensionDistance();
			ConfigureSubsteps();
		}

		private void FixedUpdate()
		{
			GetGroundHits();
			ApplySteering();
			ApplyBrakes();
			Stabilize();
			UpdateFrictionCurve();
		}

		private void GetGroundHits()
		{
			//WheelHit hit;
			FrontLeftWheelHit = FrontLeftWheelCollider.GetGroundHit(out var hit) ? hit : default(WheelHit?);
			FrontRightWheelHit = FrontRightWheelCollider.GetGroundHit(out hit) ? hit : default(WheelHit?);
			RearLeftWheelHit = RearLeftWheelCollider.GetGroundHit(out hit) ? hit : default(WheelHit?);
			RearRightWheelHit = RearRightWheelCollider.GetGroundHit(out hit) ? hit : default(WheelHit?);
		}




		private void ApplySteering()
		{
			float inner = Mathf.Abs(SteerRate * MaxSteerAngle);
			float outer = Mathf.Lerp(inner, Ackermann(inner), AckermannCoefficient);
			if (SteerRate < 0f)
			{
				FrontLeftWheelCollider.steerAngle = -inner + FrontToeAngle;
				FrontRightWheelCollider.steerAngle = -outer - FrontToeAngle;
			}
			else if (SteerRate > 0f)
			{
				FrontLeftWheelCollider.steerAngle = outer + FrontToeAngle;
				FrontRightWheelCollider.steerAngle = inner - FrontToeAngle;
			}
			else
			{
				FrontLeftWheelCollider.steerAngle = FrontToeAngle;
				FrontRightWheelCollider.steerAngle = -FrontToeAngle;
			}

			RearLeftWheelCollider.steerAngle = RearToeAngle;
			RearRightWheelCollider.steerAngle = -RearToeAngle;
		}

		private void ApplyBrakes()
		{
			float frontBrakeTorque = Brake * Mathf.Lerp(BrakeTorque * 2f, 0f, BrakeBias);
			float rearBrakeTorque = Brake * Mathf.Lerp(0f, BrakeTorque * 2f, BrakeBias);
			// TODO ハンドブレーキ
			float totalFrontBrakeTorque = frontBrakeTorque;
			float totalRearBrakeTorque = rearBrakeTorque;
			foreach (var wheelCollider in FrontWheelColliders)
			{
				wheelCollider.brakeTorque = totalFrontBrakeTorque;
			}
			foreach (var wheelCollider in RearWheelColliders)
			{
				wheelCollider.brakeTorque = totalRearBrakeTorque;
			}
		}

		private void Stabilize()
		{
			float frontLeftTravel = FrontLeftWheelHit == null ? 1.0f : (-(FrontLeftWheelCollider.transform.InverseTransformPoint(FrontLeftWheelHit.Value.point).y - FrontLeftWheelCollider.center.y) - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
			float frontRightTravel = FrontRightWheelHit == null ? 1.0f : (-(FrontRightWheelCollider.transform.InverseTransformPoint(FrontRightWheelHit.Value.point).y - FrontRightWheelCollider.center.y) - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;
			float rearLeftTravel = RearLeftWheelHit == null ? 1.0f : (-(RearLeftWheelCollider.transform.InverseTransformPoint(RearLeftWheelHit.Value.point).y - RearLeftWheelCollider.center.y) - RearLeftWheelCollider.radius) / RearLeftWheelCollider.suspensionDistance;
			float rearRightTravel = RearRightWheelHit == null ? 1.0f : (-(RearRightWheelCollider.transform.InverseTransformPoint(RearRightWheelHit.Value.point).y - RearRightWheelCollider.center.y) - RearRightWheelCollider.radius) / RearRightWheelCollider.suspensionDistance;
			float frontCoefficient = Mathf.Lerp(2 * StabilizerCoefficient, 0f, StabilizerBias);
			float rearCoefficient = Mathf.Lerp(0f, 2f * StabilizerCoefficient, StabilizerBias);
			float frontAntiRollForce = (frontLeftTravel - frontRightTravel) * frontCoefficient;
			float rearAntiRollForce = (rearLeftTravel - rearRightTravel) * rearCoefficient;
			if (FrontLeftWheelHit != null) Rigidbody.AddForceAtPosition(FrontLeftWheelCollider.transform.up * -frontAntiRollForce, WheelPosition(FrontLeftWheelCollider));
			if (FrontRightWheelHit != null) Rigidbody.AddForceAtPosition(FrontRightWheelCollider.transform.up * frontAntiRollForce, WheelPosition(FrontRightWheelCollider));
			if (RearLeftWheelHit != null) Rigidbody.AddForceAtPosition(RearLeftWheelCollider.transform.up * -rearAntiRollForce, WheelPosition(RearLeftWheelCollider));
			if (RearRightWheelHit != null) Rigidbody.AddForceAtPosition(RearRightWheelCollider.transform.up * rearAntiRollForce, WheelPosition(RearRightWheelCollider));
		}







		/// <summary>
		/// 
		/// </summary>
		private void UpdateFrictionCurve()
		{

			float brakeBonus = BrakeBonusStiffness * Brake;

			foreach (var wheelCollider in FrontWheelColliders)
			{
				WheelFrictionCurve forward = FrictionCurveSet.FrontForwardFriction;
				forward.stiffness = forward.stiffness + brakeBonus;
				wheelCollider.forwardFriction = forward;


				WheelFrictionCurve sideway = FrictionCurveSet.FrontSidewayFriction;
				//sideway.stiffness = sideway.stiffness + Mathf.Abs(SteerRate) * SteerDriftBonusStiffness;
				//sideway.stiffness = sideway.stiffness + brakeBonus;
				wheelCollider.sidewaysFriction = sideway;
			}
			foreach (var wheelCollider in RearWheelColliders)
			{
				WheelFrictionCurve forward = FrictionCurveSet.RearForwardFriction;
				forward.stiffness = forward.stiffness + brakeBonus;
				wheelCollider.forwardFriction = forward;

				WheelFrictionCurve sideway = FrictionCurveSet.RearSidewayFriction;
				//sideway.stiffness = sideway.stiffness + Mathf.Abs(SteerRate) * SteerDriftBonusStiffness;
				//sideway.stiffness = sideway.stiffness + brakeBonus;
				wheelCollider.sidewaysFriction = sideway;
			}


			// 要確認
			var driveDirection = transform.forward;
			var velocityDirection = (Rigidbody.velocity - transform.up * Vector3.Dot(Rigidbody.velocity, transform.up)).normalized;
			float angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(driveDirection, velocityDirection), transform.up));
			float angularVelocity = Rigidbody.angularVelocity.y;

			foreach (var wheelCollider in FrontWheelColliders)
			{
				if (angle * SteerRate < 0)
				{
					var compensationFactor = 0.1f;

					var sideway = wheelCollider.sidewaysFriction;
					sideway.stiffness = sideway.stiffness * (1.0f - Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelocity)));

					//sideway.stiffness = Mathf.Lerp(Sstiff, newSStiff * (1.0f - Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelocity))), Mathf.Abs(SteerRate));
					//wheelCollider.sidewaysFriction = sideway;
				}
				//else if (angle * SteerRate > 0)
				//{
					//var compensationFactor = 0.1f;

					//var sideway = wheelCollider.sidewaysFriction;
					//sideway.stiffness = sideway.stiffness * (1.0f + Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelocity)));
				//}
				else
				{


					var sideway = wheelCollider.sidewaysFriction;
					//sideway.stiffness = oldStiffness;
					wheelCollider.sidewaysFriction = sideway;
				}



			}
			

			

		}





		private void AdjustSprings()
		{
			//float naturalFrequencySquared = Mathf.Pow(NaturalFrequency, 2f);
			//float rearNaturalFrequencySquared = Mathf.Pow(RearNaturalFrequency, 2f);
			//float biasedFrontNaturalFrequencySquared = Mathf.Lerp(2f * naturalFrequencySquared, 0f, SpringRateBias);
			//float biasedRearNaturalFrequencySquared = Mathf.Lerp(0f, 2f * naturalFrequencySquared, SpringRateBias);
			foreach (var wheelCollider in FrontWheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.spring = wheelCollider.sprungMass * Mathf.Pow(2 * Mathf.PI * FrontNaturalFrequency, 2f);
				wheelCollider.suspensionSpring = suspension;
			}
			foreach (var wheelCollider in RearWheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.spring = wheelCollider.sprungMass * Mathf.Pow(2 * Mathf.PI * RearNaturalFrequency, 2f);
				wheelCollider.suspensionSpring = suspension;
			}
		}

		private void AdjustDampers()
		{
			foreach (var wheelCollider in WheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.damper = 2f * DampingRatio * Mathf.Sqrt(wheelCollider.sprungMass * suspension.spring);
				wheelCollider.suspensionSpring = suspension;
			}
		}

		private void AdjustForcePoints()
		{
			foreach (var wheelCollider in WheelColliders)
			{
				float mass = Rigidbody.transform.InverseTransformPoint(Rigidbody.worldCenterOfMass).y;
				float wheel = Rigidbody.transform.InverseTransformPoint(WheelPosition(wheelCollider)).y;
				float wheelToMass = (mass - wheel) * Rigidbody.transform.lossyScale.y / wheelCollider.transform.lossyScale.y;
				float centerOfMassDistance = wheelToMass + wheelCollider.radius;
				float shift = ForceShift / wheelCollider.transform.lossyScale.y;
				wheelCollider.forceAppPointDistance = centerOfMassDistance - shift;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void ConfigureSuspensionDistance()
		{
			// 
			if (AutoConfigureSuspensionDistance)
			{
				// 
				float gravity = Mathf.Abs(Physics.gravity.y * Body.GravityMultiplier);
				// 
				foreach (var wheelCollider in WheelColliders)
				{
					//
					float distance = wheelCollider.sprungMass * gravity / (WheelColliders.Average(w => w.suspensionSpring.targetPosition) * wheelCollider.suspensionSpring.spring);
					// 
					wheelCollider.suspensionDistance = distance / wheelCollider.transform.lossyScale.y;
				}
			}
			// 
			else
			{
				foreach (var wheelCollider in FrontWheelColliders)
				{
					wheelCollider.suspensionDistance = FrontSuspensionDistance / wheelCollider.transform.lossyScale.y;
				}
				foreach (var wheelCollider in RearWheelColliders)
				{
					wheelCollider.suspensionDistance = RearSuspensionDistance / wheelCollider.transform.lossyScale.y;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void ConfigureSubsteps()
		{
			// リジッドボディに付随しているホイールコライダの中で一つに対してだけ呼び出せばよい
			FrontLeftWheelCollider.ConfigureVehicleSubsteps(SubstepsSpeedThreshold, SubstepsBelowThreshold, SubstepsAboveThreshold);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerAngle"></param>
		/// <returns></returns>
		private float Ackermann(float innerAngle)
		{
			float axleTrack = (FrontTrack + RearTrack) / 2f;
			return Mathf.Atan(1 / (axleTrack / Wheelbase + 1 / Mathf.Tan(innerAngle * Mathf.Deg2Rad))) * Mathf.Rad2Deg;
		}

		private Vector3 WheelPosition(WheelCollider wheelCollider)
		{
			return wheelCollider.transform.TransformPoint(wheelCollider.center);
		}
	}
}
