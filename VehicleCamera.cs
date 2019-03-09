using UnityEngine;

namespace WanganSystem.Vehicle
{
	// 車を追従するカメラ
	[RequireComponent(typeof(Camera))]
	public class VehicleCamera : MonoBehaviour
	{
		// パブリック変数
		public Rigidbody target;
		public float cameraHeight = 1f;
		public float targetHeight = 1f;
		public float minSpeed = 0f;
		public float maxSpeed = 100f;
		public float minSpeedDistance = 3f;
		public float maxSpeedDistance = 4f;
		public float minSpeedFieldOfView = 55f;
		public float maxSpeedFieldOfView = 65f;
		public float noise = 0.5f;
		public bool subjective = false;
		public Vector3 subjectiveOffset;

		// プライベート変数
		private new Camera camera; // カメラ（キャッシュ）
		private float speed; // 速度（キャッシュ）

		// 初期化
		void Start()
		{
			camera = GetComponent<Camera>();
		}

		// 物理フレームごとの処理
		//void FixedUpdate()
		//{
			
		//}

		// 遅延ルーチン
		void FixedUpdate()
		{

			// 物理フレームごとに速度をキャッシュ
			speed = target.velocity.magnitude;

			float speedFactor = Mathf.InverseLerp(speed, minSpeed, maxSpeed);
			camera.fieldOfView = Mathf.Lerp(minSpeedFieldOfView, maxSpeedFieldOfView, speedFactor);
			if (subjective)
			{
				transform.position = target.transform.position + target.transform.TransformDirection(subjectiveOffset);
				transform.rotation = target.transform.rotation;
			}
			else
			{
				float distance = Mathf.Lerp(minSpeedDistance, maxSpeedDistance, speedFactor);
				transform.position = target.transform.position + target.transform.up * cameraHeight - target.transform.forward * distance;
				transform.LookAt(target.transform.position + target.transform.up * targetHeight);
			}
			transform.position = transform.position + target.velocity.magnitude / 100 * new Vector3(Random.Range(-noise / 2, noise / 2), Random.Range(-noise / 2, noise / 2));
		}
	}
}
