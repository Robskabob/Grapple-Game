using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomTeam : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int TeamID;
	public Color Color;
	public Text Header;
	public UnityEngine.UI.Button Button;
	public Graphic Graphic;
	public List<RoomPlayer> Players;
	public Vector2 TargetSize;
	public float Speed = 5;

	public const float Hight = 50;

	private void Update()
	{
		RectTransform Rect = transform as RectTransform;
		Rect.sizeDelta = Vector2.Lerp(Rect.sizeDelta, TargetSize, Time.deltaTime * Speed);
	}

	public void UpdateOrder()
	{
		for (int i = 0; i < Players.Count; i++)
		{
			RectTransform Rect = Players[i].transform as RectTransform;
			Players[i].TargetPosition = new Vector2(0, -(Hight + 65) - (i * (Hight + 15)));
			Rect.sizeDelta = new Vector2(-30, Hight);
		}
	}


	public void JoinPlayer(RoomPlayer Player)
	{
		Player.transform.SetParent(transform);
		Player.Team = TeamID;
		Player.TargetColor = Color;
		if (!Players.Contains(Player))
			Players.Add(Player);

		UpdateOrder();
	}

	public void LeavePlayer(RoomPlayer Player)
	{
		Players.Remove(Player);

		UpdateOrder();
	}
	public void Deselect()
	{
		Button.interactable = false;
		Button.interactable = true;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		TargetSize = new Vector2(40, -10);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TargetSize = new Vector2(-20, -30);
	}
}
