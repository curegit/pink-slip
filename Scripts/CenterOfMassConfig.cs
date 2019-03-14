namespace Speedcar
{
	/// <summary>
	/// 車体の重心の設定方法
	/// </summary>
	public enum CenterOfMassConfig
	{
		/// <summary>
		/// 物理エンジンによって自動計算されたものを使用する
		/// </summary>
		Default = 0,

		/// <summary>
		/// 物理エンジンによって自動計算されたものとコライダーの中心から計算する
		/// </summary>
		Automatic = 1,

		/// <summary>
		/// 物理エンジンによって自動計算されたものからのずらしで指定する
		/// </summary>
		Offset = 2,

		/// <summary>
		/// Transformオブジェクトによって指定する
		/// </summary>
		Transform = 3,
	}
}
