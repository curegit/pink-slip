using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// 車両の足回り
	/// </summary>
	[RequireComponent(typeof(Body), typeof(Rigidbody)), DisallowMultipleComponent]
	public class Chassis : MonoBehaviour
	{
		/// <summary>
		/// 左前輪のホイールコライダーのバッキングフィールド
		/// </summary>
		[SerializeField]
		private WheelCollider frontLeftWheelCollider;

		/// <summary>
		/// 右前輪のホイールコライダーのバッキングフィールド
		/// </summary>
		[SerializeField]
		private WheelCollider frontRightWheelCollider;

		/// <summary>
		/// 左後輪のホイールコライダーのバッキングフィールド
		/// </summary>
		[SerializeField]
		private WheelCollider rearLeftWheelCollider;

		/// <summary>
		/// 右後輪のホイールコライダーのバッキングフィールド
		/// </summary>
		[SerializeField]
		private WheelCollider rearRightWheelCollider;

		/// <summary>
		/// 前輪のスプリングの固有振動数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float frontNaturalFrequency = 1.9f;

		/// <summary>
		/// 後輪のスプリングの固有振動数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float rearNaturalFrequency = 1.9f;

		/// <summary>
		/// サスペンションの減衰比のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float dampingRatio = 0.3f;

		/// <summary>
		/// 作用点を重心から鉛直下向きにどれだけずらすかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float forceShift = 0.1f;

		/// <summary>
		/// サスペンション長を自動設定するかのバッキングフィールド
		/// </summary>
		[SerializeField]
		private bool autoConfigureSuspensionDistance;

		/// <summary>
		/// 手動設定の前輪のサスペンション長のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float frontSuspensionDistance = 0.25f;

		/// <summary>
		/// 手動設定の前輪のサスペンション長のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float rearSuspensionDistance = 0.25f;

		/// <summary>
		/// ステアリングの最大舵角のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxSteerAngle = 30f;

		/// <summary>
		/// アッカーマンアングルの適用率のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float ackermannCoefficient = 1f;

		/// <summary>
		/// 前輪のトー角のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float frontToeAngle = 0f;

		/// <summary>
		/// 後輪のトー角のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float rearToeAngle = 0.1f;

		/// <summary>
		/// オーバーステアアシストの適用量のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float antiOversteer = 0.1f;

		/// <summary>
		/// ブレーキトルク量のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float brakeTorque = 2000f;

		/// <summary>
		/// 前後のブレーキバイアスのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float brakeBias = 0.5f;

		/// <summary>
		/// ハンドブレーキトルク量のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float handBrakeTorque = 300f;

		/// <summary>
		/// スプリングレートに対するスタビライザーの硬さのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float stabilizerCoefficient = 1.1f;

		/// <summary>
		/// 前後のスタビライザーの配分のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float stabilizerBias = 0.5f;

		/// <summary>
		/// 摩擦曲線の表現の集合のバッキングフィールド
		/// </summary>
		[SerializeField]
		private FrictionCurveSet frictionCurveSet;

		/// <summary>
		/// サブステップを変更する速度の閾値のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float substepsSpeedThreshold = 10f;

		/// <summary>
		/// 閾値より上のサブステップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private int substepsAboveThreshold = 50;

		/// <summary>
		/// 閾値より下のサブステップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private int substepsBelowThreshold = 50;

		/// <summary>
		/// 左前輪の剛性を定数倍するのバッキングフィールド
		/// </summary>
		private float frontLeftWheelStiffnessMultiplier = 1f;

		/// <summary>
		/// 右前輪の剛性を定数倍するのバッキングフィールド
		/// </summary>
		private float frontRightWheelStiffnessMultiplier = 1f;

		/// <summary>
		/// 左後輪の剛性を定数倍するのバッキングフィールド
		/// </summary>
		private float rearLeftWheelStiffnessMultiplier = 1f;

		/// <summary>
		/// 右後輪の剛性を定数倍するのバッキングフィールド
		/// </summary>
		private float rearRightWheelStiffnessMultiplier = 1f;

		/// <summary>
		/// ブレーキ入力率のバッキングフィールド
		/// </summary>
		private float brake;

		/// <summary>
		/// ハンドブレーキ入力率のバッキングフィールド
		/// </summary>
		private float handBrake;

		/// <summary>
		/// ステアリング入力率のバッキングフィールド
		/// </summary>
		private float steerRate;

		/// <summary>
		/// 左前輪のホイールコライダー
		/// </summary>
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

		/// <summary>
		/// 右前輪のホイールコライダー
		/// </summary>
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

		/// <summary>
		/// 左後輪のホイールコライダー
		/// </summary>
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

		/// <summary>
		/// 右後輪のホイールコライダー
		/// </summary>
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

		/// <summary>
		/// すべてのホイールコライダーの反復子を返す
		/// </summary>
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

		/// <summary>
		/// 前輪のホイールコライダーの反復子を返す
		/// </summary>
		public IEnumerable<WheelCollider> FrontWheelColliders
		{
			get
			{
				yield return FrontLeftWheelCollider;
				yield return FrontRightWheelCollider;
			}
		}

		/// <summary>
		/// 後輪のホイールコライダーの反復子を返す
		/// </summary>
		public IEnumerable<WheelCollider> RearWheelColliders
		{
			get
			{
				yield return RearLeftWheelCollider;
				yield return RearRightWheelCollider;
			}
		}

		/// <summary>
		/// 前輪のスプリングの固有振動数
		/// </summary>
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

		/// <summary>
		/// 後輪のスプリングの固有振動数
		/// </summary>
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

		/// <summary>
		/// サスペンションの減衰比
		/// </summary>
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

		/// <summary>
		/// 作用点を重心から鉛直下向きにどれだけずらすか
		/// </summary>
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

		/// <summary>
		/// サスペンション長を自動設定するか
		/// </summary>
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

		/// <summary>
		/// 手動設定の前輪のサスペンション長
		/// </summary>
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

		/// <summary>
		/// 手動設定の前輪のサスペンション長
		/// </summary>
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

		/// <summary>
		/// ステアリングの最大舵角
		/// </summary>
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

		/// <summary>
		/// アッカーマンアングルの適用率（0=使用しない 1=完全なアッカーマンアングル）
		/// </summary>
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

		/// <summary>
		/// 前輪のトー角
		/// </summary>
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

		/// <summary>
		/// 後輪のトー角
		/// </summary>
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

		/// <summary>
		/// オーバーステアアシストの適用量
		/// </summary>
		private float AntiOversteer
		{
			get
			{
				return antiOversteer;
			}
			set
			{
				antiOversteer = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// ブレーキトルク量
		/// </summary>
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

		/// <summary>
		/// 前後のブレーキバイアス
		/// </summary>
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

		/// <summary>
		/// ハンドブレーキトルク量
		/// </summary>
		private float HandBrakeTorque
		{
			get
			{
				return handBrakeTorque;
			}
			set
			{
				handBrakeTorque = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// スプリングレートに対するスタビライザーの硬さ
		/// </summary>
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

		/// <summary>
		/// 前後のスタビライザーの配分
		/// </summary>
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

		/// <summary>
		/// 摩擦曲線の表現の集合
		/// </summary>
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

		/// <summary>
		/// サブステップを変更する速度の閾値
		/// </summary>
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

		/// <summary>
		/// 閾値より上のサブステップ
		/// </summary>
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

		/// <summary>
		/// 閾値より下のサブステップ
		/// </summary>
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

		/// <summary>
		/// 左前輪の接触情報
		/// </summary>
		public WheelHit? FrontLeftWheelHit { get; private set; }

		/// <summary>
		/// 右前輪の接触情報
		/// </summary>
		public WheelHit? FrontRightWheelHit { get; private set; }

		/// <summary>
		/// 左後輪の接触情報
		/// </summary>
		public WheelHit? RearLeftWheelHit { get; private set; }

		/// <summary>
		/// 右後輪の接触情報
		/// </summary>
		public WheelHit? RearRightWheelHit { get; private set; }

		/// <summary>
		/// すべてのホイールの接触情報の反復子を返す
		/// </summary>
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

		/// <summary>
		/// 前輪の接触情報の反復子を返す
		/// </summary>
		public IEnumerable<WheelHit?> FrontWheelHits
		{
			get
			{
				yield return FrontLeftWheelHit;
				yield return FrontRightWheelHit;
			}
		}

		/// <summary>
		/// 後輪の接触情報の反復子を返す
		/// </summary>
		public IEnumerable<WheelHit?> RearWheelHits
		{
			get
			{
				yield return RearLeftWheelHit;
				yield return RearRightWheelHit;
			}
		}

		/// <summary>
		/// 左前輪の剛性を定数倍する（ランタイム用）
		/// </summary>
		public float FrontLeftWheelStiffnessMultiplier
		{
			get
			{
				return frontLeftWheelStiffnessMultiplier;
			}
			set
			{
				frontLeftWheelStiffnessMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 右前輪の剛性を定数倍する
		/// </summary>
		public float FrontRightWheelStiffnessMultiplier
		{
			get
			{
				return frontRightWheelStiffnessMultiplier;
			}
			set
			{
				frontRightWheelStiffnessMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 左後輪の剛性を定数倍する
		/// </summary>
		public float RearLeftWheelStiffnessMultiplier
		{
			get
			{
				return rearLeftWheelStiffnessMultiplier;
			}
			set
			{
				rearLeftWheelStiffnessMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 右後輪の剛性を定数倍する
		/// </summary>
		public float RearRightWheelStiffnessMultiplier
		{
			get
			{
				return rearRightWheelStiffnessMultiplier;
			}
			set
			{
				rearRightWheelStiffnessMultiplier = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// ブレーキ入力率（0=無効 1=完全）
		/// </summary>
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

		/// <summary>
		/// ハンドブレーキ入力率（0=無効 1=完全）
		/// </summary>
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

		/// <summary>
		/// ステアリング入力率（-1=最左 0=中立 1=最右）
		/// </summary>
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

		/// <summary>
		/// 前輪のトレッド
		/// </summary>
		public float FrontTrack
		{
			get
			{
				Vector3 left = WheelBasePosition(FrontLeftWheelCollider);
				Vector3 right = WheelBasePosition(FrontRightWheelCollider);
				Vector3 diff = Rigidbody.transform.InverseTransformDirection(left - right);
				return Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
			}
		}

		/// <summary>
		/// 後輪のトレッド
		/// </summary>
		public float RearTrack
		{
			get
			{
				Vector3 left = WheelBasePosition(RearLeftWheelCollider);
				Vector3 right = WheelBasePosition(RearRightWheelCollider);
				Vector3 diff = Rigidbody.transform.InverseTransformDirection(left - right);
				return Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
			}
		}

		/// <summary>
		/// ホイールベース
		/// </summary>
		public float Wheelbase
		{
			get
			{
				Vector3 frontLeft = WheelBasePosition(FrontLeftWheelCollider);
				Vector3 frontRight = WheelBasePosition(FrontRightWheelCollider);
				Vector3 rearLeft = WheelBasePosition(RearLeftWheelCollider);
				Vector3 rearRight = WheelBasePosition(RearRightWheelCollider);
				Vector3 leftDiff = Rigidbody.transform.InverseTransformDirection(frontLeft - rearLeft);
				Vector3 rightDiff = Rigidbody.transform.InverseTransformDirection(frontRight - rearRight);
				float left = Mathf.Sqrt(leftDiff.y * leftDiff.y + leftDiff.z * leftDiff.z);
				float right = Mathf.Sqrt(rightDiff.y * rightDiff.y + rightDiff.z * rightDiff.z);
				return (left + right) / 2f;
			}
		}

		/// <summary>
		/// 車体コンポーネント
		/// </summary>
		private Body Body { get; set; }

		/// <summary>
		/// 剛体コンポーネント
		/// </summary>
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
			FixTargetPositions();
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
			float handBrakeTorque = HandBrake * HandBrakeTorque;
			float totalFrontBrakeTorque = frontBrakeTorque;
			float totalRearBrakeTorque = rearBrakeTorque + handBrakeTorque;
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
			// TODO
			float frontLeftTravel = FrontLeftWheelHit == null ? 1.0f : (-(FrontLeftWheelCollider.transform.InverseTransformPoint(FrontLeftWheelHit.Value.point).y - FrontLeftWheelCollider.center.y) - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
			float frontRightTravel = FrontRightWheelHit == null ? 1.0f : (-(FrontRightWheelCollider.transform.InverseTransformPoint(FrontRightWheelHit.Value.point).y - FrontRightWheelCollider.center.y) - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;
			float rearLeftTravel = RearLeftWheelHit == null ? 1.0f : (-(RearLeftWheelCollider.transform.InverseTransformPoint(RearLeftWheelHit.Value.point).y - RearLeftWheelCollider.center.y) - RearLeftWheelCollider.radius) / RearLeftWheelCollider.suspensionDistance;
			float rearRightTravel = RearRightWheelHit == null ? 1.0f : (-(RearRightWheelCollider.transform.InverseTransformPoint(RearRightWheelHit.Value.point).y - RearRightWheelCollider.center.y) - RearRightWheelCollider.radius) / RearRightWheelCollider.suspensionDistance;
			/*
			float frontLeftTravel = FrontLeftWheelHit == null ? 1.0f : (-(FrontLeftWheelCollider.transform.InverseTransformPoint(FrontLeftWheelHit.Value.point).y - FrontLeftWheelCollider.center.y) - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
			float frontRightTravel = FrontRightWheelHit == null ? 1.0f : (-(FrontRightWheelCollider.transform.InverseTransformPoint(FrontRightWheelHit.Value.point).y - FrontRightWheelCollider.center.y) - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;
			float rearLeftTravel = RearLeftWheelHit == null ? 1.0f : (-(RearLeftWheelCollider.transform.InverseTransformPoint(RearLeftWheelHit.Value.point).y - RearLeftWheelCollider.center.y) - RearLeftWheelCollider.radius) / RearLeftWheelCollider.suspensionDistance;
			float rearRightTravel = RearRightWheelHit == null ? 1.0f : (-(RearRightWheelCollider.transform.InverseTransformPoint(RearRightWheelHit.Value.point).y - RearRightWheelCollider.center.y) - RearRightWheelCollider.radius) / RearRightWheelCollider.suspensionDistance;
			*/
			float stabilizerForce = StabilizerCoefficient * (FrontLeftWheelCollider.suspensionSpring.spring + RearLeftWheelCollider.suspensionSpring.spring) / 2f;
			float frontCoefficient = Mathf.Lerp(2 * stabilizerForce, 0f, StabilizerBias);
			float rearCoefficient = Mathf.Lerp(0f, 2f * stabilizerForce, StabilizerBias);
			float frontAntiRollForce = (frontLeftTravel - frontRightTravel) * frontCoefficient;
			float rearAntiRollForce = (rearLeftTravel - rearRightTravel) * rearCoefficient;
			var upward = Rigidbody.rotation * Vector3.up;
			if (FrontLeftWheelHit != null) Rigidbody.AddForceAtPosition(upward * -frontAntiRollForce, WheelBasePosition(FrontLeftWheelCollider));
			if (FrontRightWheelHit != null) Rigidbody.AddForceAtPosition(upward * frontAntiRollForce, WheelBasePosition(FrontRightWheelCollider));
			if (RearLeftWheelHit != null) Rigidbody.AddForceAtPosition(upward * -rearAntiRollForce, WheelBasePosition(RearLeftWheelCollider));
			if (RearRightWheelHit != null) Rigidbody.AddForceAtPosition(upward * rearAntiRollForce, WheelBasePosition(RearRightWheelCollider));
			/*
			if (FrontLeftWheelHit != null) Rigidbody.AddForceAtPosition(FrontLeftWheelCollider.transform.up * -frontAntiRollForce, WheelPosition(FrontLeftWheelCollider));
			if (FrontRightWheelHit != null) Rigidbody.AddForceAtPosition(FrontRightWheelCollider.transform.up * frontAntiRollForce, WheelPosition(FrontRightWheelCollider));
			if (RearLeftWheelHit != null) Rigidbody.AddForceAtPosition(RearLeftWheelCollider.transform.up * -rearAntiRollForce, WheelPosition(RearLeftWheelCollider));
			if (RearRightWheelHit != null) Rigidbody.AddForceAtPosition(RearRightWheelCollider.transform.up * rearAntiRollForce, WheelPosition(RearRightWheelCollider));
			*/
		}







		/// <summary>
		/// 
		/// </summary>
		private void UpdateFrictionCurve()
		{
			// 
			var frontLeftForward = FrictionCurveSet.FrontForwardFriction;
			var frontLeftSideways = FrictionCurveSet.FrontSidewaysFriction;
			frontLeftForward.stiffness = frontLeftForward.stiffness * FrontLeftWheelStiffnessMultiplier;
			frontLeftSideways.stiffness = frontLeftSideways.stiffness * FrontLeftWheelStiffnessMultiplier;
			FrontLeftWheelCollider.forwardFriction = frontLeftForward;
			FrontLeftWheelCollider.sidewaysFriction = frontLeftSideways;
			// 
			var frontRightForward = FrictionCurveSet.FrontForwardFriction;
			var frontRightSideways = FrictionCurveSet.FrontSidewaysFriction;
			frontRightForward.stiffness = frontRightForward.stiffness * FrontRightWheelStiffnessMultiplier;
			frontRightSideways.stiffness = frontRightSideways.stiffness * FrontRightWheelStiffnessMultiplier;
			FrontRightWheelCollider.forwardFriction = frontRightForward;
			FrontRightWheelCollider.sidewaysFriction = frontRightSideways;
			// 
			var rearLeftForward = FrictionCurveSet.RearForwardFriction;
			var rearLeftSideways = FrictionCurveSet.RearSidewaysFriction;
			rearLeftForward.stiffness = rearLeftForward.stiffness * RearLeftWheelStiffnessMultiplier;
			rearLeftSideways.stiffness = rearLeftSideways.stiffness * RearLeftWheelStiffnessMultiplier;
			RearLeftWheelCollider.forwardFriction = rearLeftForward;
			RearLeftWheelCollider.sidewaysFriction = rearLeftSideways;
			// 
			var rearRightForward = FrictionCurveSet.RearForwardFriction;
			var rearRightSideways = FrictionCurveSet.RearSidewaysFriction;
			rearRightForward.stiffness = rearRightForward.stiffness * RearRightWheelStiffnessMultiplier;
			rearRightSideways.stiffness = rearRightSideways.stiffness * RearRightWheelStiffnessMultiplier;
			RearRightWheelCollider.forwardFriction = rearRightForward;
			RearRightWheelCollider.sidewaysFriction = rearRightSideways;
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

		private void FixTargetPositions()
		{
			foreach (var wheelCollider in WheelColliders)
			{
				var suspension = wheelCollider.suspensionSpring;
				suspension.targetPosition = 0.5f;
				wheelCollider.suspensionSpring = suspension;
			}
		}

		private void AdjustForcePoints()
		{
			foreach (var wheelCollider in WheelColliders)
			{
				float mass = Rigidbody.transform.InverseTransformPoint(Rigidbody.worldCenterOfMass).y;
				float wheel = Rigidbody.transform.InverseTransformPoint(WheelBasePosition(wheelCollider)).y;
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
			wheelCollider.GetWorldPose(out var position, out var rotation);
			return position;
		}

		private Vector3 WheelBasePosition(WheelCollider wheelCollider)
		{
			return wheelCollider.transform.TransformPoint(wheelCollider.center);
		}
	}
}
