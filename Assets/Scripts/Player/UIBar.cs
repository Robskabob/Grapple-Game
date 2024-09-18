using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBar : UIBehaviour
{
	public Image Bar;

	public void UpdateValue(float value) 
	{
		Bar.fillAmount = value;
	}
}
