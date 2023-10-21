namespace PinkSlip
{
	/// <summary>
	/// トルク曲線の表現方法
	/// </summary>
	public enum TorqueCurveModel
	{
		/// <summary>
		/// 近似モデルでトルク曲線を表現する
		/// </summary>
		Parametric = 1,

		/// <summary>
		/// アニメーションカーブでトルク曲線を表現する
		/// </summary>
		AnimationCurve = 2,
	}
}
