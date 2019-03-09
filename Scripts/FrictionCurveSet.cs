using System;
using UnityEngine;

namespace Speedcar
{
	/// <summary>
	/// ホイールコライダに渡す摩擦曲線の表現
	/// </summary>
	[Serializable]
	public class FrictionCurveSet
	{
		[SerializeField]
		private float frontForwardExtremumSlip;

		[SerializeField]
		private float frontForwardExtremumValue;

		[SerializeField]
		private float frontForwardAsymptoteSlip;

		[SerializeField]
		private float frontForwardAsymptoteValue;

		[SerializeField]
		private float frontForwardStiffness;


		[SerializeField]
		private float frontSidewayExtremumSlip;

		[SerializeField]
		private float frontSidewayExtremumValue;

		[SerializeField]
		private float frontSidewayAsymptoteSlip;

		[SerializeField]
		private float frontSidewayAsymptoteValue;

		[SerializeField]
		private float frontSidewayStiffness;



		[SerializeField]
		private float rearForwardExtremumSlip;

		[SerializeField]
		private float rearForwardExtremumValue;

		[SerializeField]
		private float rearForwardAsymptoteSlip;

		[SerializeField]
		private float rearForwardAsymptoteValue;

		[SerializeField]
		private float rearForwardStiffness;


		[SerializeField]
		private float rearSidewayExtremumSlip;

		[SerializeField]
		private float rearSidewayExtremumValue;

		[SerializeField]
		private float rearSidewayAsymptoteSlip;

		[SerializeField]
		private float rearSidewayAsymptoteValue;

		[SerializeField]
		private float rearSidewayStiffness;








		public float FrontForwardExtremumSlip
		{
			get
			{
				return frontForwardExtremumSlip;
			}
			set
			{
				frontForwardExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		public float FrontForwardExtremumValue
		{
			get
			{
				return frontForwardExtremumValue;
			}
			set
			{
				frontForwardExtremumValue = Mathf.Max(value, 0f);
			}
		}

		public float FrontForwardAsymptoteSlip
		{
			get
			{
				return frontForwardAsymptoteSlip;
			}
			set
			{
				frontForwardAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		public float FrontForwardAsymptoteValue
		{
			get
			{
				return frontForwardAsymptoteValue;
			}
			set
			{
				frontForwardAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

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






		public float FrontSidewayExtremumSlip
		{
			get
			{
				return frontSidewayExtremumSlip;
			}
			set
			{
				frontSidewayExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		public float FrontSidewayExtremumValue
		{
			get
			{
				return frontSidewayExtremumValue;
			}
			set
			{
				frontSidewayExtremumValue = Mathf.Max(value, 0f);
			}
		}

		public float FrontSidewayAsymptoteSlip
		{
			get
			{
				return frontSidewayAsymptoteSlip;
			}
			set
			{
				frontSidewayAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		public float FrontSidewayAsymptoteValue
		{
			get
			{
				return frontSidewayAsymptoteValue;
			}
			set
			{
				frontSidewayAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

		public float FrontSidewayStiffness
		{
			get
			{
				return frontSidewayStiffness;
			}
			set
			{
				frontSidewayStiffness = Mathf.Max(value, 0f);
			}
		}




		public float RearForwardExtremumSlip
		{
			get
			{
				return rearForwardExtremumSlip;
			}
			set
			{
				rearForwardExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		public float RearForwardExtremumValue
		{
			get
			{
				return rearForwardExtremumValue;
			}
			set
			{
				rearForwardExtremumValue = Mathf.Max(value, 0f);
			}
		}

		public float RearForwardAsymptoteSlip
		{
			get
			{
				return rearForwardAsymptoteSlip;
			}
			set
			{
				rearForwardAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		public float RearForwardAsymptoteValue
		{
			get
			{
				return rearForwardAsymptoteValue;
			}
			set
			{
				rearForwardAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

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






		public float RearSidewayExtremumSlip
		{
			get
			{
				return rearSidewayExtremumSlip;
			}
			set
			{
				rearSidewayExtremumSlip = Mathf.Max(value, 0f);
			}
		}

		public float RearSidewayExtremumValue
		{
			get
			{
				return rearSidewayExtremumValue;
			}
			set
			{
				rearSidewayExtremumValue = Mathf.Max(value, 0f);
			}
		}

		public float RearSidewayAsymptoteSlip
		{
			get
			{
				return rearSidewayAsymptoteSlip;
			}
			set
			{
				rearSidewayAsymptoteSlip = Mathf.Max(value, 0f);
			}
		}

		public float RearSidewayAsymptoteValue
		{
			get
			{
				return rearSidewayAsymptoteValue;
			}
			set
			{
				rearSidewayAsymptoteValue = Mathf.Max(value, 0f);
			}
		}

		public float RearSidewayStiffness
		{
			get
			{
				return rearSidewayStiffness;
			}
			set
			{
				rearSidewayStiffness = Mathf.Max(value, 0f);
			}
		}




		public WheelFrictionCurve FrontForwardFriction => new WheelFrictionCurve
		{
			extremumSlip = FrontForwardExtremumSlip,
			extremumValue = FrontForwardExtremumValue,
			asymptoteSlip = FrontForwardAsymptoteSlip,
			asymptoteValue = FrontForwardAsymptoteValue,
			stiffness = FrontForwardStiffness,
		};

		public WheelFrictionCurve FrontSidewayFriction => new WheelFrictionCurve
		{
			extremumSlip = FrontSidewayExtremumSlip,
			extremumValue = FrontSidewayExtremumValue,
			asymptoteSlip = FrontSidewayAsymptoteSlip,
			asymptoteValue = FrontSidewayAsymptoteValue,
			stiffness = FrontSidewayStiffness,
		};

		public WheelFrictionCurve RearForwardFriction => new WheelFrictionCurve
		{
			extremumSlip = RearForwardExtremumSlip,
			extremumValue = RearForwardExtremumValue,
			asymptoteSlip = RearForwardAsymptoteSlip,
			asymptoteValue = RearForwardAsymptoteValue,
			stiffness = RearForwardStiffness,
		};

		public WheelFrictionCurve RearSidewayFriction => new WheelFrictionCurve
		{
			extremumSlip = RearSidewayExtremumSlip,
			extremumValue = RearSidewayExtremumValue,
			asymptoteSlip = RearSidewayAsymptoteSlip,
			asymptoteValue = RearSidewayAsymptoteValue,
			stiffness = RearSidewayStiffness,
		};
	}
}
