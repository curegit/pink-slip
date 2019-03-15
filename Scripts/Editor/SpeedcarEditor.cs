using UnityEngine;
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
			EditorGUILayout.LabelField(label);
		}

		/// <summary>
		/// 空白をあける
		/// </summary>
		protected void Space()
		{
			EditorGUILayout.Space();
		}

		/// <summary>
		/// インデントを増やす
		/// </summary>
		protected void IncreaseIndent()
		{
			EditorGUI.indentLevel++;
		}

		/// <summary>
		/// インデントを減らす
		/// </summary>
		protected void DecreaseIndent()
		{
			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// 既定の方法でフィールドを表示する
		/// </summary>
		/// <param name="field">フィールド名</param>
		/// <param name="label">ラベルテキスト</param>
		/// <param name="tooltip">ヒント</param>
		protected void PropertyField(string field, string label = null, string tooltip = null)
		{
			if (label == null)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(field), true);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(field), new GUIContent(label, tooltip));
			}
		}
	}
}
