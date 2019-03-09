using UnityEditor;

namespace Speedcar.EditorOnly
{
	/// <summary>
	/// インスペクター拡張のための基底クラス
	/// </summary>
	public class SpeedcarEditor : Editor
	{
		/// <summary>
		/// ヘッダを表示をする
		/// </summary>
		/// <param name="label">表示する文字列</param>
		protected void Header(string label)
		{
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
		}

		/// <summary>
		/// 空白をあける
		/// </summary>
		protected void Space()
		{
			EditorGUILayout.Space();
		}
	}
}
