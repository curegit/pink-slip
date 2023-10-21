using System;
using UnityEngine;

namespace PinkSlip
{
	/// <summary>
	/// ホイールコライダに渡す摩擦曲線の表現
	/// </summary>
	[Serializable]
	public class FrictionCurveSet
	{
		/// <summary>
		/// 縦方向の極大スリップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float forwardExtremumSlip = 1.0f;

		/// <summary>
		/// 縦方向の極大スリップの摩擦のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float forwardExtremumValue = 8.4f;

		/// <summary>
		/// 縦方向の漸近スリップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float forwardAsymptoteSlip = 2.0f;

		/// <summary>
		/// 縦方向の漸近スリップの摩擦のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float forwardAsymptoteValue = 7.2f;

		/// <summary>
		/// 横方向の極大スリップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float sidewaysExtremumSlip = 1.0f;

		/// <summary>
		/// 横方向の極大スリップの摩擦のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float sidewaysExtremumValue = 7.0f;

		/// <summary>
		/// 横方向の漸近スリップのバッキングフィールド
		/// </summary>
		[SerializeField]
		private float sidewaysAsymptoteSlip = 2.0f;

		/// <summary>
		/// 横方向の漸近スリップの摩擦のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float sidewaysAsymptoteValue = 6.0f;

		/// <summary>
		/// 前輪の縦方向の剛性のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float frontForwardStiffness = 1.0f;

		/// <summary>
		/// 前輪の横方向の剛性のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float rearForwardStiffness = 1.0f;

		/// <summary>
		/// 後輪の縦方向の剛性のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float frontSidewaysStiffness = 1.0f;

		/// <summary>
		/// 後輪の横方向の剛性のバッキングフィールド
		/// </summary>
		[SerializeField]
		private float rearSidewaysStiffness = 1.0f;

		/// <summary>
		/// 縦方向の極大スリップ
		/// </summary>
		public float ForwardExtremumSlip
		{
			get
			{
				return forwardExtremumSlip;
			}
			set
			{
				forwardExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 縦方向の極大スリップの摩擦
		/// </summary>
		public float ForwardExtremumValue
		{
			get
			{
				return forwardExtremumValue;
			}
			set
			{
				forwardExtremumValue = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 縦方向の漸近スリップ
		/// </summary>
		public float ForwardAsymptoteSlip
		{
			get
			{
				return forwardAsymptoteSlip;
			}
			set
			{
				forwardAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 縦方向の漸近スリップの摩擦
		/// </summary>
		public float ForwardAsymptoteValue
		{
			get
			{
				return forwardAsymptoteValue;
			}
			set
			{
				forwardAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 横方向の極大スリップ
		/// </summary>
		public float SidewaysExtremumSlip
		{
			get
			{
				return sidewaysExtremumSlip;
			}
			set
			{
				sidewaysExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 横方向の極大スリップの摩擦
		/// </summary>
		public float SidewaysExtremumValue
		{
			get
			{
				return sidewaysExtremumValue;
			}
			set
			{
				sidewaysExtremumValue = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 横方向の漸近スリップ
		/// </summary>
		public float SidewaysAsymptoteSlip
		{
			get
			{
				return sidewaysAsymptoteSlip;
			}
			set
			{
				sidewaysAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 横方向の漸近スリップの摩擦
		/// </summary>
		public float SidewaysAsymptoteValue
		{
			get
			{
				return sidewaysAsymptoteValue;
			}
			set
			{
				sidewaysAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 前輪の縦方向の剛性
		/// </summary>
		public float FrontForwardStiffness
		{
			get
			{
				return frontForwardStiffness;
			}
			set
			{
				frontForwardStiffness = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 前輪の横方向の剛性
		/// </summary>
		public float FrontSidewaysStiffness
		{
			get
			{
				return frontSidewaysStiffness;
			}
			set
			{
				frontSidewaysStiffness = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 後輪の縦方向の剛性
		/// </summary>
		public float RearForwardStiffness
		{
			get
			{
				return rearForwardStiffness;
			}
			set
			{
				rearForwardStiffness = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 後輪の横方向の剛性
		/// </summary>
		public float RearSidewaysStiffness
		{
			get
			{
				return rearSidewaysStiffness;
			}
			set
			{
				rearSidewaysStiffness = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// 前輪の縦方向の摩擦曲線を返す
		/// </summary>
		public WheelFrictionCurve FrontForwardFriction => new WheelFrictionCurve
		{
			extremumSlip = ForwardExtremumSlip,
			extremumValue = ForwardExtremumValue,
			asymptoteSlip = ForwardAsymptoteSlip,
			asymptoteValue = ForwardAsymptoteValue,
			stiffness = FrontForwardStiffness,
		};

		/// <summary>
		/// 後輪の縦方向の摩擦曲線を返す
		/// </summary>
		public WheelFrictionCurve RearForwardFriction => new WheelFrictionCurve
		{
			extremumSlip = ForwardExtremumSlip,
			extremumValue = ForwardExtremumValue,
			asymptoteSlip = ForwardAsymptoteSlip,
			asymptoteValue = ForwardAsymptoteValue,
			stiffness = RearForwardStiffness,
		};

		/// <summary>
		/// 前輪の横方向の摩擦曲線を返す
		/// </summary>
		public WheelFrictionCurve FrontSidewaysFriction => new WheelFrictionCurve
		{
			extremumSlip = SidewaysExtremumSlip,
			extremumValue = SidewaysExtremumValue,
			asymptoteSlip = SidewaysAsymptoteSlip,
			asymptoteValue = SidewaysAsymptoteValue,
			stiffness = FrontSidewaysStiffness,
		};

		/// <summary>
		/// 後輪の横方向の摩擦曲線を返す
		/// </summary>
		public WheelFrictionCurve RearSidewaysFriction => new WheelFrictionCurve
		{
			extremumSlip = SidewaysExtremumSlip,
			extremumValue = SidewaysExtremumValue,
			asymptoteSlip = SidewaysAsymptoteSlip,
			asymptoteValue = SidewaysAsymptoteValue,
			stiffness = RearSidewaysStiffness,
		};
	}
}
