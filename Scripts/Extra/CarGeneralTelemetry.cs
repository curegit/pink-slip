using System.Linq;
using UnityEngine;

namespace Speedcar.Extra
{
	/// <summary>
	/// 車の基本情報を簡易表示する
	/// </summary>
	public class CarGeneralTelemetry : MonoBehaviour
	{
		/// <summary>
		/// 車両のコンポーネント
		/// </summary>
		public CarController Car;

		/// <summary>
		/// ボックスの幅のバッキングフィールド
		/// </summary>
		[SerializeField]
		private int width = 300;

		/// <summary>
		/// ボックスの高さのバッキングフィールド
		/// </summary>
		[SerializeField]
		private int height = 120;

		/// <summary>
		/// マージンのバッキングフィールド
		/// </summary>
		[SerializeField]
		private int margin = 10;

		/// <summary>
		/// フォントサイズのバッキングフィールド
		/// </summary>
		[SerializeField]
		private int fontSize = 18;

		/// <summary>
		/// レッドライン表示にする回転割合のバッキングフィールド
		/// </summary>
		[SerializeField, Range(0f, 1f)]
		private float highRevRate = 0.9f;

		/// <summary>
		/// 作動中の機能の色
		/// </summary>
		public Color ActiveColor = new Color32(0, 180, 90, 255);

		/// <summary>
		/// 無効な機能の色
		/// </summary>
		public Color DisabledColor = new Color32(45, 45, 45, 130);

		/// <summary>
		/// レッドラインの色
		/// </summary>
		public Color HighRevColor = new Color32(225, 0, 0, 255);

		/// <summary>
		/// リバースギアの色
		/// </summary>
		public Color ReverseColor = new Color32(0, 64, 255, 255);

		/// <summary>
		/// ニュートラルギアの色
		/// </summary>
		public Color NeutralColor = new Color32(0, 210, 0, 255);

		/// <summary>
		/// 有効なドライブギアの色
		/// </summary>
		public Color DriveColor = new Color32(255, 70, 0, 255);

		/// <summary>
		/// 前フレームで追跡していた対象車両
		/// </summary>
		private CarController carController;

		/// <summary>
		/// 車体コンポーネント
		/// </summary>
		private Body body;

		/// <summary>
		/// 動力系コンポーネント
		/// </summary>
		private Powertrain powertrain;

		/// <summary>
		/// 足回りコンポーネント
		/// </summary>
		private Chassis chassis;

		/// <summary>
		/// ボックスの幅
		/// </summary>
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = Mathf.Max(value, 1);
			}
		}

		/// <summary>
		/// ボックスの高さ
		/// </summary>
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = Mathf.Max(value, 1);
			}
		}

		/// <summary>
		/// マージン
		/// </summary>
		public int Margin
		{
			get
			{
				return margin;
			}
			set
			{
				margin = Mathf.Max(value, 1);
			}
		}

		/// <summary>
		/// フォントサイズ
		/// </summary>
		public int FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				fontSize = Mathf.Max(value, 1);
			}
		}

		/// <summary>
		/// レッドライン表示にする回転割合
		/// </summary>
		public float HighRevRate
		{
			get
			{
				return highRevRate;
			}
			set
			{
				highRevRate = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// 毎フレーム呼ばれる
		/// </summary>
		private void Update()
		{
			if (Car != carController)
			{
				carController = Car;
				body = Car.GetComponent<Body>();
				chassis = Car.GetComponent<Chassis>();
				powertrain = Car.GetComponent<Powertrain>();
			}
		}

		/// <summary>
		/// IMGUIとして呼ばれる
		/// </summary>
		private void OnGUI()
		{
			if (Car)
			{
				var style = GUI.skin.box;
				style.richText = true;
				style.fontSize = FontSize;
				style.alignment = TextAnchor.LowerRight;
				GUI.Box(new Rect(Screen.width - Width - Margin, Screen.height - Height - Margin, Width, Height), MakeTelemetryString(), style);
			}
		}

		/// <summary>
		/// 表示するテキストをつくる
		/// </summary>
		/// <returns>テレメトリーテキスト</returns>
		private string MakeTelemetryString()
		{
			int rpm = Mathf.RoundToInt(powertrain.RPM);
			int rev = Mathf.RoundToInt(powertrain.RevLimit);
			float angular = body.AngularSpeed;
			float angacc = body.AngularAcceleration;
			float speed = Mathf.Abs(body.ForwardVelocity);
			int mps = Mathf.RoundToInt(speed);
			int kph = Mathf.RoundToInt(speed * 3.6f);
			int mph = Mathf.RoundToInt(speed * 2.236936f);
			string boost = powertrain.HasForcedInduction ? $"{powertrain.Pressure / 100000f - 1.01325f:f1} bar" : $"<color=#{ColorUtility.ToHtmlStringRGBA(DisabledColor)}>{powertrain.Pressure / 100000f - 1.01325f:f1} bar</color>";
			string tachometer = powertrain.RPM >= powertrain.RevLimit * 0.9f ? $"<color=#{ColorUtility.ToHtmlStringRGBA(HighRevColor)}>{powertrain.RPM:f1} / {Mathf.RoundToInt(powertrain.RevLimit)} (rpm)</color>" : $"{powertrain.RPM:f1} / {Mathf.RoundToInt(powertrain.RevLimit)} (rpm)";
			string gears = Enumerable.Range(1, powertrain.TopGear).Select(g => powertrain.Gear == g ? $"-<color=#{ColorUtility.ToHtmlStringRGBA(DriveColor)}>[{g}]</color>" : $"- {g} ").Aggregate((l, r) => l + r);
			string gear = $"{(powertrain.Gear == -1 ? $"<color=#{ColorUtility.ToHtmlStringRGBA(ReverseColor)}>[R]</color>" : " R ")} - {(powertrain.Gear == 0 ? $"<color=#{ColorUtility.ToHtmlStringRGBA(NeutralColor)}>[N]</color>" : " N ")} {gears}";
			string aos = chassis.AntiOversteer > 0f ? (chassis.IsAntiOversteerWorking ? $"<color=#{ColorUtility.ToHtmlStringRGBA(ActiveColor)}>AOS</color>" : "AOS") : $"<color=#{ColorUtility.ToHtmlStringRGBA(DisabledColor)}>AOS</color>";
			string abs = carController.UseAntiLockBrake ? (carController.IsBrakeAdjusted ? $"<color=#{ColorUtility.ToHtmlStringRGBA(ActiveColor)}>ABS</color>" : "ABS") : $"<color=#{ColorUtility.ToHtmlStringRGBA(DisabledColor)}>ABS</color>";
			string tcs = carController.UseTractionControl ? (carController.IsGasAdjusted ? $"<color=#{ColorUtility.ToHtmlStringRGBA(ActiveColor)}>TCS</color>" : "TCS") : $"<color=#{ColorUtility.ToHtmlStringRGBA(DisabledColor)}>TCS</color>";
			string nos = powertrain.HasNitrous ? (powertrain.Nitrous && powertrain.RemainingNitrous > 0 ? $"<color=#{ColorUtility.ToHtmlStringRGBA(ActiveColor)}>NOS [{powertrain.RemainingNitrous:f2}s]</color>" : $"NOS [{powertrain.RemainingNitrous:f2}s]") : $"<color=#{ColorUtility.ToHtmlStringRGBA(DisabledColor)}>NOS</color>";
			string aosabstcs = $"{nos} - {aos} - {abs} - {tcs}";
			string angvelo = $"{angular:f3} (rad/s) | {angacc:f3} (rad/s²)";
			string velo = $"{mps:D3} (m/s) | {kph:D3} (km/h) | {mph:D3} (mph)";
			return $"{boost} | {tachometer}\n{gear}\n{aosabstcs}\n{angvelo}\n{velo}";
		}
	}
}
