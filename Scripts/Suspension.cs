using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Speedcar
{
	[RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
	public class Suspension : MonoBehaviour
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
		private float frontNaturalFrequency = 1.2f;

		[SerializeField]
		private float rearNaturalFrequency = 1.2f;

		[SerializeField]
		private float dampingRatio = 0.7f;

		[SerializeField]
		private float springRateBias = 0.5f;

		[SerializeField]
		private float forceShift = 0.1f;

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
		private float toeAngle = 0.1f;

		[SerializeField]
		private float antiOversteerCoefficient;

		[SerializeField]
		private float frontBrakeTorque;

		[SerializeField]
		private float rearBrakeTorque;

		[SerializeField]
		private float brakeBias = 0.5f;

		[SerializeField]
		private float frontStabilizerCoefficient;

		[SerializeField]
		private float rearStabilizerCoefficient;

		[SerializeField]
		private float stabilizerBias = 0.5f;

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

		public float SpringRateBias
		{
			get
			{
				return springRateBias;
			}
			set
			{
				springRateBias = Mathf.Clamp01(value);
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


		public float ToeAngle
		{
			get
			{
				return toeAngle;
			}
			set
			{
				toeAngle = Mathf.Max(value, 0f);
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

		public float FrontBrakeTorque
		{
			get
			{
				return frontBrakeTorque;
			}
			set
			{
				frontBrakeTorque = Mathf.Max(value, 0f);
			}
		}


		public float RearBrakeTorque
		{
			get
			{
				return rearBrakeTorque;
			}
			set
			{
				rearBrakeTorque = Mathf.Max(value, 0f);
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

		public float FrontStabilizerCoefficient
		{
			get
			{
				return frontStabilizerCoefficient;
			}
			set
			{
				frontStabilizerCoefficient = Mathf.Max(value, 0f);
			}
		}

		public float RearStabilizerCoefficient
		{
			get
			{
				return rearStabilizerCoefficient;
			}
			set
			{
				rearStabilizerCoefficient = Mathf.Max(value, 0f);
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

		public float ForwardSlip
		{
			get
			{
				var hits = WheelHits.Where(x => x != null);
				return hits.Any() ? hits.Average(h => h.Value.forwardSlip) : 0f;
			}
		}

		public float ForwardExtremumSlip
		{
			get
			{
				return WheelColliders.Average(w => w.forwardFriction.extremumSlip);
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
				//Vector3 left = FrontLeftWheelCollider.transform.position + FrontLeftWheelCollider.transform.TransformPoint(FrontLeftWheelCollider.center);
				//Vector3 right = FrontRightWheelCollider.transform.position + FrontRightWheelCollider.transform.TransformPoint(FrontRightWheelCollider.center);
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
				//Vector3 left = RearLeftWheelCollider.transform.position + RearLeftWheelCollider.transform.TransformPoint(RearLeftWheelCollider.center);
				//Vector3 right = RearRightWheelCollider.transform.position + RearRightWheelCollider.transform.TransformPoint(RearRightWheelCollider.center);
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
				/*
				Vector3 frontLeft = FrontLeftWheelCollider.transform.position + FrontLeftWheelCollider.transform.TransformPoint(FrontLeftWheelCollider.center);
				Vector3 frontRight = FrontRightWheelCollider.transform.position + FrontRightWheelCollider.transform.TransformPoint(FrontRightWheelCollider.center);
				Vector3 rearLeft = RearLeftWheelCollider.transform.position + RearLeftWheelCollider.transform.TransformPoint(RearLeftWheelCollider.center);
				Vector3 rearRight = RearRightWheelCollider.transform.position + RearRightWheelCollider.transform.TransformPoint(RearRightWheelCollider.center);
				*/
				Vector3 leftDiff = Rigidbody.transform.InverseTransformDirection(frontLeft - rearLeft);
				Vector3 rightDiff = Rigidbody.transform.InverseTransformDirection(frontRight - rearRight);
				float left = Mathf.Sqrt(leftDiff.y * leftDiff.y + leftDiff.z * leftDiff.z);
				float right = Mathf.Sqrt(rightDiff.y * rightDiff.y + rightDiff.z * rightDiff.z);
				return (left + right) / 2f;
			}
		}


		private Rigidbody Rigidbody { get; set; }

		// Use this for initialization
		private void Start()
		{
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



			print(FrontTrack);
			print(RearTrack);
			print(Wheelbase);
		}

		private void FixedUpdate()
		{
			GetGroundHits();
			ApplySteering();
			ApplyBrakes();
			Stabilize();
			PreventOversteer();
		}

		private void GetGroundHits()
		{
			WheelHit hit;
			FrontLeftWheelHit = FrontLeftWheelCollider.GetGroundHit(out hit) ? hit : default(WheelHit?);
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
				FrontLeftWheelCollider.steerAngle = -inner + ToeAngle;
				FrontRightWheelCollider.steerAngle = -outer - ToeAngle;
			}
			else if (SteerRate > 0f)
			{
				FrontLeftWheelCollider.steerAngle = outer + ToeAngle;
				FrontRightWheelCollider.steerAngle = inner - ToeAngle;
			}
			else
			{
				FrontLeftWheelCollider.steerAngle = ToeAngle;
				FrontRightWheelCollider.steerAngle = -ToeAngle;
			}
		}

		private void ApplyBrakes()
		{
			float frontBrakeTorque = Brake * Mathf.Lerp(FrontBrakeTorque * 2f, 0f, BrakeBias);
			float rearBrakeTorque = Brake * Mathf.Lerp(0f, RearBrakeTorque * 2f, BrakeBias);
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
			float travelLeftFront = FrontLeftWheelHit == null ? 1.0f : (-(FrontLeftWheelCollider.transform.InverseTransformPoint(FrontLeftWheelHit.Value.point).y - FrontLeftWheelCollider.center.y) - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
			float travelRightFront = FrontRightWheelHit == null ? 1.0f : (-(FrontRightWheelCollider.transform.InverseTransformPoint(FrontRightWheelHit.Value.point).y - FrontRightWheelCollider.center.y) - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;


			float frontCoefficient = Mathf.Lerp(2 * FrontStabilizerCoefficient, 0f, StabilizerBias);
			float rearCoefficient = Mathf.Lerp(0f, 2f * RearStabilizerCoefficient, StabilizerBias);

			float frontAntiRollForce = (travelLeftFront - travelRightFront) * frontCoefficient;

			if (FrontLeftWheelHit != null) Rigidbody.AddForceAtPosition(FrontLeftWheelCollider.transform.up * -frontAntiRollForce, WheelPosition(FrontLeftWheelCollider));
			if (FrontRightWheelHit != null) Rigidbody.AddForceAtPosition(FrontRightWheelCollider.transform.up * frontAntiRollForce, WheelPosition(FrontRightWheelCollider));




		}

		private void PreventOversteer()
		{
			// 要確認
			Vector3 driveDirection = transform.forward;
			Vector3 velocityDirection = (Rigidbody.velocity - transform.up * Vector3.Dot(Rigidbody.velocity, transform.up)).normalized;
			float angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(driveDirection, velocityDirection), transform.up));
			float angularVelocity = Rigidbody.angularVelocity.y;


			/*
			if (angle * SteerRate < 0)
			{
				var sideway = wheelCollider.sidewaysFriction;
				// sideway.stiffness = oldStiffness * (1.0f - Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelocity)));

				sideway.stiffness = Mathf.Lerp(oldStiffness, oldStiffness * (1.0f - Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelocity))), Mathf.Abs(SteerRate));
				wheelCollider.sidewaysFriction = sideway;
			}
			else
			{
				var sideway = wheelCollider.sidewaysFriction;
				sideway.stiffness = oldStiffness;
				wheelCollider.sidewaysFriction = sideway;
			}
			*/

		}





		private void AdjustSprings()
		{
			float frontNaturalFrequencySquared = Mathf.Pow(FrontNaturalFrequency, 2f);
			float rearNaturalFrequencySquared = Mathf.Pow(RearNaturalFrequency, 2f);
			float biasedFrontNaturalFrequencySquared = Mathf.Lerp(2f * frontNaturalFrequencySquared, 0f, SpringRateBias);
			float biasedRearNaturalFrequencySquared = Mathf.Lerp(0f, 2f * rearNaturalFrequencySquared, SpringRateBias);
			foreach (var wheelCollider in FrontWheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.spring = wheelCollider.sprungMass * biasedFrontNaturalFrequencySquared * Mathf.Pow(2 * Mathf.PI, 2f);
				wheelCollider.suspensionSpring = suspension;
			}
			foreach (var wheelCollider in RearWheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.spring = wheelCollider.sprungMass * biasedRearNaturalFrequencySquared * Mathf.Pow(2 * Mathf.PI, 2f);
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
				float gravity = Mathf.Abs(Physics.gravity.y);
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

		private static Vector3 WheelPosition(WheelCollider wheelCollider)
		{
			return wheelCollider.transform.TransformPoint(wheelCollider.center);
		}
	}
}
