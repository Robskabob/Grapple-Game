public class Crate : Holdable
{
	public override bool Directional => false;

	public override void LoadFab(mapdata.savedata data)
	{

	}

	public override mapdata.savedata SaveFab()
	{
		return null;
	}

	public override void RpcUse()
	{
		CmdDrop();
	}

	public override void RpcUseOther()
	{

	}
}