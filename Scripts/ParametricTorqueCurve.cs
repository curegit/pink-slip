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
		/// パワーバンド上限の回転数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxRPM = 6400f;

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
		/// 仕事率の最大値のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxPower = 320000f;

		/// <summary>
		/// 仕事率が最大値をとる回転数のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float maxPowerRPM = 6000f;

		/// <summary>
		/// パワーバンド上限の回転数
		/// </summary>
		public float MaxRPM
		{
			get
			{
				return maxRPM;
			}
			set
			{
				maxRPM = Mathf.Max(value, 0f);
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
		/// 仕事率の最大値
		/// </summary>
		public float MaxPower
		{
			get
			{
				return maxPower;
			}
			set
			{
				maxPower = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 仕事率が最大値をとる回転数
		/// </summary>
		public float MaxPowerRPM
		{
			get
			{
				return maxPowerRPM;
			}
			set
			{
				maxPowerRPM = Mathf.Max(value, 0f);
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
			else
			{
				float maxPowerTorque = MaxPower / (MaxPowerRPM * 2f * Mathf.PI / 60);
				float aproxFactor = (MaxTorque - maxPowerTorque) / (2f * MaxTorqueRPM * MaxPowerRPM - Mathf.Pow(MaxPowerRPM, 2f) - Mathf.Pow(MaxTorqueRPM, 2f));
				float torque = Mathf.Max(aproxFactor * Mathf.Pow(rpm - MaxTorqueRPM, 2f) + MaxTorque, 0f);
				if (rpm > MaxRPM)
				{
					return Mathf.Max(torque * (1f - ((rpm - MaxRPM) * 0.001f)), 0f);
				}
				else
				{
					return torque;
				}
			}
		}
	}
}
