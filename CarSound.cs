using UnityEngine;
using Speedcar;

namespace HighwayRun
{
	// 車のエンジン音
	[RequireComponent(typeof(AudioSource))]
	public class CarSound : MonoBehaviour
	{
		// シリアライズフィールド
		[SerializeField]
		private Powertrain engine; // エンジンへの参照
		[SerializeField]
		private AudioClip engineSound; // エンジン音
		[SerializeField]
		private float lowerRpm; // 線形補間用下側RPM
		[SerializeField]
		private float upperRpm; // 線形補間用上側RPM
		[SerializeField]
		private float lowerRpmPitch; // 上側RPMのときのピッチ
		[SerializeField]
		private float upperRpmPitch; // 下側RPMのときのピッチ
		[SerializeField]
		private float maxGasVolume; // 最大スロットルのときの音量
		[SerializeField]
		private float zeroGasVolume; // 最小スロットルのときの音量

		// プライベート変数
		private AudioSource audioSource; // オーディオソースへの参照

		// 初期化
		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.clip = engineSound;
			audioSource.loop = true;
			audioSource.Play();
		}

		// 毎フレームの処理
		private void Update()
		{
			float t = Mathf.InverseLerp(lowerRpm, upperRpm, engine.RPM);
			float pitch = Mathf.LerpUnclamped(lowerRpmPitch, upperRpmPitch, t);
			float volume = Mathf.Lerp(zeroGasVolume, maxGasVolume, engine.Throttle) * Mathf.Lerp(zeroGasVolume, maxGasVolume, Mathf.InverseLerp(engine.IdlingRPM, engine.RevLimit, engine.RPM));
			audioSource.pitch = pitch;
			audioSource.volume = volume;
		}
	}
}
