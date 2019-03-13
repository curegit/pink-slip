using System;
using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// トルク曲線のパラメータによる表現
	/// </summary>
	[Serializable]
	public class ParametricTorqueCurve
	{
		/// <summary>
		/// アイドリング回転数のトルクのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float idlingTorque = 280f;

		/// <summary>
		/// トルクの最大値のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxTorque = 580f;

		/// <summary>
		/// トルクが最大値をとる下限の回転数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxTorqueLowerRPM = 3200f;

		/// <summary>
		/// トルクが最大値をとる上限の回転数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxTorqueUpperRPM = 5600f;

		/// <summary>
		/// レッドライン境界のトルクのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float redlineTorque = 460f;

		/// <summary>
		/// レッドライン境界のRPMのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float redline = 7200f;

		/// <summary>
		/// アイドリング回転数のトルク
		/// </summary>
		public float IdlingTorque
		{
			get
			{
				return idlingTorque;
			}
			set
			{
				idlingTorque = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// トルクの最大値
		/// </summary>
		public float MaxTorque
		{
			get
			{
				return maxTorque;
			}
			set
			{
				maxTorque = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// トルクが最大値をとる下限の回転数
		/// </summary>
		public float MaxTorqueLowerRPM
		{
			get
			{
				return maxTorqueLowerRPM;
			}
			set
			{
				maxTorqueLowerRPM = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// トルクが最大値をとる上限の回転数
		/// </summary>
		public float MaxTorqueUpperRPM
		{
			get
			{
				return maxTorqueUpperRPM;
			}
			set
			{
				maxTorqueUpperRPM = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// レッドライン境界のトルク
		/// </summary>
		public float RedlineTorque
		{
			get
			{
				return redlineTorque;
			}
			set
			{
				redlineTorque = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// レッドライン境界のRPM
		/// </summary>
		public float Redline
		{
			get
			{
				return redline;
			}
			set
			{
				redline = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// ある回転数における出力トルク返す
		/// </summary>
		/// <param name="rpm">回転数</param>
		/// <param name="idlingRPM">アイドリング回転数</param>
		/// <param name="maxRPM">最大の回転数</param>
		/// <returns>トルク</returns>
		public float EngineTorque(float rpm, float idlingRPM, float maxRPM)
		{
			if (rpm < 0f)
			{
				return 0f;
			}
			else if (rpm < idlingRPM)
			{
				return Mathf.Lerp(0f, IdlingTorque, rpm / idlingRPM);
			}
			else if (rpm < MaxTorqueLowerRPM)
			{
				return MaxTorque * (-Mathf.Pow(rpm / MaxTorqueLowerRPM - 1f, 2f) + 1f);
			}
			else if (rpm < MaxTorqueUpperRPM)
			{
				return MaxTorque;
			}
			else if (rpm < Redline)
			{
				float t = Mathf.InverseLerp(MaxTorqueUpperRPM, Redline, rpm);
				return Bias(MaxTorque, RedlineTorque, t, 0.2f);
			}
			else
			{
				float t = Mathf.InverseLerp(Redline, maxRPM, rpm);
				return Bias(RedlineTorque, 0f, t, 0.6f);
			}
		}

		/// <summary>
		/// 偏りのある補間関数
		/// </summary>
		/// <param name="x">値</param>
		/// <param name="bias">偏り</param>
		/// <returns>補間値</returns>
		private static float Bias(float x, float bias = 0.5f)
		{
			return Mathf.Pow(x, Mathf.Log(bias) / Mathf.Log(0.5f));
		}

		/// <summary>
		/// 偏りのある補間関数
		/// </summary>
		/// <param name="a">開始値</param>
		/// <param name="b">終了値</param>
		/// <param name="t">媒介変数</param>
		/// <param name="bias">偏り</param>
		/// <returns>補間値</returns>
		private static float Bias(float a, float b, float t, float bias = 0.5f)
		{
			return Mathf.Lerp(a, b, Bias(t, bias));
		}
	}
}
