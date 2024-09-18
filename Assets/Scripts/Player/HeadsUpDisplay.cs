using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
	public Text TopText;
	public PlayerController PC;

	public UIBar HealthBar;

	private void Update()
	{
		HealthBar.UpdateValue(PC.DP.Health / PC.DP.MaxHealth);
	}
}