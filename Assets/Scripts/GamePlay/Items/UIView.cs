using UnityEngine.EventSystems;

public class UIView : UIBehaviour
{
	public Gun Gun;
	public UIBar Reload;
	public UIBar Ammo;

	public void Update()
	{
		Reload.UpdateValue(Gun._Wait / Gun.Reload);
		Ammo.UpdateValue(Gun._Clip / (float)Gun.Clip);
	}
}
