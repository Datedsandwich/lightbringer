using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour 
{
	// Data Members ------------------------------------------
	private static readonly GUISkin skin = Resources.Load<GUISkin>("GameSkin");

	public static FloatingText Show(string text, string style, IFloatingTextPositioner positioner)
	{
		var go = new GameObject("floating Text");
		var floatingText = go.AddComponent<FloatingText>();
		floatingText.style = skin.GetStyle(style);
		floatingText._positioner = positioner;
		floatingText._content = new GUIContent(text);
		return floatingText;
	}

	private GUIContent _content;
	private IFloatingTextPositioner _positioner;

	public string text{get{return _content.text;} set {_content.text = value;}}
	public GUIStyle style {get; set;}
	// End Data Members --------------------------------------
	
	// Methods -----------------------------------------------------------------
	public void OnGUI()
	{
		Vector2 position = new Vector2();
		var contentSize = style.CalcSize(_content);
		if(!_positioner.GetPosition(ref position, _content, contentSize))
		{
			Destroy (gameObject);
			return;
		}

		GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y), _content, style);
	}
	// -------------------------------------------------------
	
	// End Methods -------------------------------------------------------------
}
