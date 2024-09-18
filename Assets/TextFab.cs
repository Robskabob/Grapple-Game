using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TextFab : MonoBehaviour, SavableFab
{
    public Text Text;

	public struct TextData : mapdata.savedata
	{
		public string Text;
		public int Size;

		public TextData(string text, int size)
		{
			Text = text;
			Size = size;
		}

		public int BoolNum()
		{
			return 0;
		}

		#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			EditorGUILayout.TextField("Text ", Text);
			EditorGUILayout.IntField("Size ", Size);
		}
		#endif

		public Transform LoadObject()
		{
			throw new System.NotImplementedException();
		}
	}
	public void LoadFab(mapdata.savedata data)
	{
		TextData Data = (TextData)data;
		Text.text = Data.Text;
		Text.fontSize = Data.Size;
	}

	public mapdata.savedata SaveFab()
	{
		return new TextData(Text.text,Text.fontSize);
	}
}
