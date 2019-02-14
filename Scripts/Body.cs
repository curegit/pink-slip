using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 
	/// </summary>
	[RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
	public class Body : MonoBehaviour
	{
		
		[SerializeField]
		private Transform centerOfMass;

		[SerializeField]
		private Vector3 inertiaTensor = new Vector3(1500f, 2000f, 1000f);

		[SerializeField]
		private Vector3 inertiaTensorRotation = new Vector3(0.5f, 0f, 0f);

		[SerializeField]
		private float gravityMultiplier = 1.2f;

		[SerializeField]
		private float downforceCoefficient = 1f;

		[SerializeField]
		private float maxDownforce = 12000f;

		[SerializeField]
		private float maxAngularSpeed = 0.8f;

		[SerializeField]
		private float maxDepenetrationSpeed = 20;

		[SerializeField]
		private int solverIterations = 16;

		[SerializeField]
		private int solverVelocityIterations = 8;



		public Transform CenterOfMass
		{
			get
			{
				return centerOfMass;
			}
			set
			{
				centerOfMass = value;
			}
		}

		public Vector3 InertiaTensor
		{
			get
			{
				return inertiaTensor;
			}
			set
			{
				inertiaTensor = value;
			}
		}

		public Vector3 InertiaTensorRotation
		{
			get
			{
				return inertiaTensorRotation;
			}
			set
			{
				inertiaTensorRotation = value;
			}
		}

		public float GravityMultiplier
		{
			get
			{
				return gravityMultiplier;
			}
			set
			{
				gravityMultiplier = value;
			}
		}

		public float DownforceCoefficient
		{
			get
			{
				return downforceCoefficient;
			}
			set
			{
				downforceCoefficient = value;
			}
		}

		public float MaxDownforce
		{
			get
			{
				return maxDownforce;
			}
			set
			{
				maxDownforce = value;
			}
		}

		public float MaxAngularSpeed
		{
			get
			{
				return maxAngularSpeed;
			}
			set
			{
				maxAngularSpeed = value;
			}
		}

		public float MaxDepenetrationSpeed
		{
			get
			{
				return maxDepenetrationSpeed;
			}
			set
			{
				maxDepenetrationSpeed = value;
			}
		}

		public int SolverIterations
		{
			get
			{
				return solverIterations;
			}
			set
			{
				solverIterations = value;
			}
		}

		public int SolverVelocityIterations
		{
			get
			{
				return solverVelocityIterations;
			}
			set
			{
				solverVelocityIterations = value;
			}
		}

		public float ForwardVelocity { get; private set; }

		public float DriftVelocity { get; private set; }

		public float DriftAngle { get; private set; }

		public float Horsepower { get; private set; }

		private float KineticEnergy { get; set; }

		private Vector3 AngularVelocity { get; set; }

		private Rigidbody Rigidbody { get; set; }

		private void Start()
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			UpdateRigidbodyProperties();
		}

		private void FixedUpdate()
		{
			AddExtraGravity();
			AddDownforce();
			UpdateMeasurements();
		}

		private void OnCollisionEnter(Collision collision)
		{
			// TODO
			Rigidbody.angularVelocity = Vector3.MoveTowards(AngularVelocity, Rigidbody.angularVelocity, 0.4f);
		}

		private void AddExtraGravity()
		{
			var extraGravity = Physics.gravity * (GravityMultiplier - 1f);
			Rigidbody.AddForce(extraGravity, ForceMode.Acceleration);
		}

		private void AddDownforce()
		{
			var downforce = Mathf.Clamp(Mathf.Pow(ForwardVelocity, 2f) * DownforceCoefficient, 0f, MaxDownforce);
			Rigidbody.AddRelativeForce(Vector3.down * downforce, ForceMode.Force);
		}

		private void UpdateRigidbodyProperties()
		{
			Rigidbody.solverIterations = SolverIterations;
			Rigidbody.solverVelocityIterations = SolverVelocityIterations;
			Rigidbody.maxAngularVelocity = MaxAngularSpeed;
			Rigidbody.maxDepenetrationVelocity = MaxDepenetrationSpeed;
			Rigidbody.inertiaTensor = InertiaTensor;
			Rigidbody.inertiaTensorRotation = Quaternion.Euler(InertiaTensorRotation);
			Rigidbody.centerOfMass = CenterOfMass ? Vector3.Scale(transform.InverseTransformPoint(CenterOfMass.position), transform.lossyScale) : Rigidbody.centerOfMass;
		}

		private void UpdateMeasurements()
		{
			var localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
			ForwardVelocity = localVelocity.z;
			DriftVelocity = localVelocity.x;

			// TODO
			DriftAngle = Mathf.Atan2(ForwardVelocity, DriftVelocity) * Mathf.Rad2Deg;

			var kineticEnergy = Rigidbody.mass * Rigidbody.velocity.sqrMagnitude / 2f;

			const float Watt2PS = 0.001359619f;
			Horsepower = (kineticEnergy - KineticEnergy) / Time.fixedDeltaTime * Watt2PS;

			KineticEnergy = kineticEnergy;
		}
	}
}
