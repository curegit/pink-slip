using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 車体の特性と物理挙動の制御
	/// </summary>
	[RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
	public class Body : MonoBehaviour
	{
		/// <summary>
		/// 重心を表すトランスフォームのバッキングフィールド
		/// </summary>
		[SerializeField]
		private Transform centerOfMass;

		/// <summary>
		/// 主慣性モーメントのバッキングフィールド
		/// </summary>
		[SerializeField]
		private Vector3 inertiaTensor = new Vector3(1500f, 2000f, 1000f);

		/// <summary>
		/// 慣性主軸の回転のバッキングフィールド
		/// </summary>
		[SerializeField]
		private Vector3 inertiaTensorRotation = new Vector3(0.1f, 0f, 0f);

		/// <summary>
		/// 重力を追加適用する係数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float gravityMultiplier = 1.2f;

		/// <summary>
		/// 速度に比例する空気抵抗の係数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private Vector3 linearDrag = new Vector3(1.0f, 2.0f, 0.25f);

		/// <summary>
		/// 速度の二乗に比例する空気抵抗の係数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private Vector3 quadraticDrag = new Vector3(3.0f, 4.0f, 0.5f);

		/// <summary>
		/// ダウンフォースの比例係数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float downforceCoefficient = 1f;

		/// <summary>
		/// 重心から後ろへのダウンフォース作用点のずらしのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float downforceShift = 0.2f;

		/// <summary>
		/// ダウンフォースの上限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxDownforce = 12000f;

		/// <summary>
		/// 角速度の大きさの上限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxAngularSpeed = 0.8f;

		/// <summary>
		/// 接触時における角速度の変化の上限のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxAngularVelocityDeltaOnCollision = 0.4f;

		/// <summary>
		/// 物理ソルバが貫通状態を解決するために導入できる最大の速度のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxDepenetrationSpeed = 20f;

		/// <summary>
		/// 物理ソルバのイテレーション回数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private int solverIterations = 16;

		/// <summary>
		/// 物理ソルバの速度に関するイテレーション回数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private int solverVelocityIterations = 8;

		/// <summary>
		/// 重心を表すトランスフォーム
		/// </summary>
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

		/// <summary>
		/// 主慣性モーメント
		/// </summary>
		public Vector3 InertiaTensor
		{
			get
			{
				return inertiaTensor;
			}
			set
			{
				value.x = Mathf.Max(value.x, 0f);
				value.y = Mathf.Max(value.y, 0f);
				value.z = Mathf.Max(value.z, 0f);
				inertiaTensor = value;
			}
		}

		/// <summary>
		/// 慣性主軸の回転
		/// </summary>
		public Vector3 InertiaTensorRotation
		{
			get
			{
				return inertiaTensorRotation;
			}
			set
			{
				value.x = Mathf.Max(value.x, 0f);
				value.y = Mathf.Max(value.y, 0f);
				value.z = Mathf.Max(value.z, 0f);
				inertiaTensorRotation = value;
			}
		}

		/// <summary>
		/// 重力を追加適用する係数
		/// </summary>
		public float GravityMultiplier
		{
			get
			{
				return gravityMultiplier;
			}
			set
			{
				gravityMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 速度に比例する空気抵抗の係数
		/// </summary>
		public Vector3 LinearDrag
		{
			get
			{
				return linearDrag;
			}
			set
			{
				value.x = Mathf.Max(value.x, 0f);
				value.y = Mathf.Max(value.y, 0f);
				value.z = Mathf.Max(value.z, 0f);
				linearDrag = value;
			}
		}

		/// <summary>
		/// 速度の二乗に比例する空気抵抗の係数
		/// </summary>
		public Vector3 QuadraticDrag
		{
			get
			{
				return quadraticDrag;
			}
			set
			{
				value.x = Mathf.Max(value.x, 0f);
				value.y = Mathf.Max(value.y, 0f);
				value.z = Mathf.Max(value.z, 0f);
				quadraticDrag = value;
			}
		}

		/// <summary>
		/// ダウンフォースの比例係数
		/// </summary>
		public float DownforceCoefficient
		{
			get
			{
				return downforceCoefficient;
			}
			set
			{
				downforceCoefficient = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 重心から後ろへのダウンフォース作用点のずらし
		/// </summary>
		public float DownforceShift
		{
			get
			{
				return downforceShift;
			}
			set
			{
				downforceShift = value;
			}
		}

		/// <summary>
		/// ダウンフォースの上限
		/// </summary>
		public float MaxDownforce
		{
			get
			{
				return maxDownforce;
			}
			set
			{
				maxDownforce = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 角速度の大きさの上限
		/// </summary>
		public float MaxAngularSpeed
		{
			get
			{
				return maxAngularSpeed;
			}
			set
			{
				maxAngularSpeed = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 接触時における角速度の変化の上限
		/// </summary>
		public float MaxAngularVelocityDeltaOnCollision
		{
			get
			{
				return maxAngularVelocityDeltaOnCollision;
			}
			set
			{
				maxAngularVelocityDeltaOnCollision = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 物理ソルバが貫通状態を解決するために導入できる最大の速度
		/// </summary>
		public float MaxDepenetrationSpeed
		{
			get
			{
				return maxDepenetrationSpeed;
			}
			set
			{
				maxDepenetrationSpeed = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 物理ソルバのイテレーション回数
		/// </summary>
		public int SolverIterations
		{
			get
			{
				return solverIterations;
			}
			set
			{
				solverIterations = Mathf.Max(value, 0);
			}
		}

		/// <summary>
		/// 物理ソルバの速度に関するイテレーション回数
		/// </summary>
		public int SolverVelocityIterations
		{
			get
			{
				return solverVelocityIterations;
			}
			set
			{
				solverVelocityIterations = Mathf.Max(value, 0);
			}
		}

		/// <summary>
		/// このフレームの車体のx軸方向の速度
		/// </summary>
		public float SidewayVelocity { get; private set; }

		/// <summary>
		/// このフレームの車体のy軸方向の速度
		/// </summary>
		public float UpwardVelocity { get; private set; }

		/// <summary>
		/// このフレームの車体のz軸方向の速度
		/// </summary>
		public float ForwardVelocity { get; private set; }

		/// <summary>
		/// このフレームの車体の角速度
		/// </summary>
		private Vector3 AngularVelocity { get; set; }

		/// <summary>
		/// 車体の剛体コンポーネント
		/// </summary>
		private Rigidbody Rigidbody { get; set; }

		/// <summary>
		/// 初期化時に呼ばれるメソッド
		/// </summary>
		private void Start()
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// 毎フレームに呼ばれるメソッド
		/// </summary>
		private void Update()
		{
			UpdateRigidbodyProperties();
		}

		/// <summary>
		/// 毎物理フレームに呼ばれるメソッド
		/// </summary>
		private void FixedUpdate()
		{
			UpdateMeasurements();
			AddExtraGravity();
			AddAerodynamicFriction();
			AddDownforce();
		}

		/// <summary>
		/// 物理衝突があったときに呼ばれる
		/// </summary>
		/// <param name="collision">衝突相手</param>
		private void OnCollisionEnter(Collision collision)
		{
			var newAngularVelocity = Vector3.MoveTowards(AngularVelocity, Rigidbody.angularVelocity, MaxAngularVelocityDeltaOnCollision);
			Rigidbody.AddTorque(newAngularVelocity - AngularVelocity, ForceMode.VelocityChange);
		}

		/// <summary>
		/// 各種の値を記録する
		/// </summary>
		private void UpdateMeasurements()
		{
			SidewayVelocity = Vector3.Dot(Rigidbody.velocity, Rigidbody.rotation * Vector3.right);
			UpwardVelocity = Vector3.Dot(Rigidbody.velocity, Rigidbody.rotation * Vector3.up);
			ForwardVelocity = Vector3.Dot(Rigidbody.velocity, Rigidbody.rotation * Vector3.forward);
			AngularVelocity = Rigidbody.angularVelocity;
		}

		/// <summary>
		/// 追加の重力を適用する
		/// </summary>
		private void AddExtraGravity()
		{
			var extraGravity = Physics.gravity * (GravityMultiplier - 1f);
			Rigidbody.AddForce(extraGravity, ForceMode.Acceleration);
		}

		/// <summary>
		/// 空気抵抗を適用する
		/// </summary>
		private void AddAerodynamicFriction()
		{
			var localSpeed = new Vector3(Mathf.Abs(SidewayVelocity), Mathf.Abs(UpwardVelocity), Mathf.Abs(ForwardVelocity));
			var linear = -Vector3.Scale(LinearDrag, localSpeed);
			Rigidbody.AddRelativeForce(linear, ForceMode.Force);
			var quadratic = -Vector3.Scale(QuadraticDrag, Vector3.Scale(localSpeed, localSpeed));
			Rigidbody.AddRelativeForce(quadratic, ForceMode.Force);
		}

		/// <summary>
		/// ダウンフォースを適用する
		/// </summary>
		private void AddDownforce()
		{
			var downforce = Rigidbody.rotation * Vector3.down * Mathf.Clamp(Mathf.Pow(ForwardVelocity, 2f) * DownforceCoefficient, 0f, MaxDownforce);
			var position = Rigidbody.worldCenterOfMass + Rigidbody.rotation * Vector3.back * downforceShift;
			Rigidbody.AddForceAtPosition(downforce, position, ForceMode.Force);
		}

		/// <summary>
		/// 剛体コンポーネントの属性値を更新する
		/// </summary>
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
	}
}
