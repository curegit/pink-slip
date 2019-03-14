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
				EditorGUILayout.PropertyField(serializedObject.FindProperty(field));
			}
			else
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(field), new GUIContent(label, tooltip));
			}
		}

		/// <summary>
		/// floatのフィールドを扱うスライダーを表示する
		/// </summary>
		/// <param name="field">フィールド名</param>
		/// <param name="left">左端の値</param>
		/// <param name="right">右端の値</param>
		/// <param name="label">ラベルテキスト</param>
		/// <param name="tooltip">ヒント</param>
		protected void Slider(string field, float left, float right, string label = null, string tooltip = null)
		{
			if (label == null)
			{
				EditorGUILayout.Slider(serializedObject.FindProperty(field), left, right);
			}
			else
			{
				EditorGUILayout.Slider(serializedObject.FindProperty(field), left, right, new GUIContent(label, tooltip));
			}
		}

		/// <summary>
		/// intのフィールドを扱うスライダーを表示する
		/// </summary>
		/// <param name="field">フィールド名</param>
		/// <param name="left">左端の値</param>
		/// <param name="right">右端の値</param>
		/// <param name="label">ラベルテキスト</param>
		/// <param name="tooltip">ヒント</param>
		protected void IntSlider(string field, int left, int right, string label = null, string tooltip = null)
		{
			if (label == null)
			{
				EditorGUILayout.IntSlider(serializedObject.FindProperty(field), left, right);
			}
			else
			{
				EditorGUILayout.Slider(serializedObject.FindProperty(field), left, right, new GUIContent(label, tooltip));
			}
		}
	}
}
