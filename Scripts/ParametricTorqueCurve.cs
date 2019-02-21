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
		/// トルクの最大値のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxTorque = 650f;

		/// <summary>
		/// トルクが最大値をとる回転数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxTorqueRPM = 5000f;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private float asymptoteTorque = 400f;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
		private float redline = 7000f;

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
		/// トルクが最大値をとる回転数
		/// </summary>
		public float MaxTorqueRPM
		{
			get
			{
				return maxTorqueRPM;
			}
			set
			{
				maxTorqueRPM = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public float AsymptoteTorque
		{
			get
			{
				return asymptoteTorque;
			}
			set
			{
				asymptoteTorque = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 
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
		/// <returns>トルク</returns>
		public float EngineTorque(float rpm)
		{
			if (rpm < 0f)
			{
				return 0f;
			}
			else if (rpm < MaxTorqueRPM)
			{
				return MaxTorque * (-Mathf.Pow(rpm / MaxTorqueRPM - 1f, 2f) + 1f);
			}
			else if (rpm < Redline)
			{
				float t = Mathf.InverseLerp(MaxTorqueRPM, Redline, rpm);
				return (0.5f * Mathf.Cos(t * Mathf.PI) + 0.5f) * (MaxTorque - AsymptoteTorque) + AsymptoteTorque;
			}
			else
			{
				return AsymptoteTorque;
			}
		}
	}
}
