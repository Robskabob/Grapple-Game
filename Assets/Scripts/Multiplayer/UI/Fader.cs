using System;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public Text T;
    public Graphic Graphic;
    public float Value;
    public int Max;
    public bool Active;
    public RoomPlayer LocalPlayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            Value += Time.deltaTime;
            int IntValue = (int)Value;
            T.text = (Max - IntValue).ToString();
            T.transform.localScale = Vector3.one * (Value - IntValue) * 2;

            Graphic.color = Color.Lerp(Color.clear, Color.black, Value / Max);
            T.color = Color.Lerp(Color.clear, Color.black, Value / Max);
            if (Value > Max) 
            {
                gameObject.SetActive(false);
                LocalPlayer.StartNewGame(RoomPlayer.Lobby.Level);
            }
        }
        else
        {
            Value = 0;
            Graphic.color = Color.Lerp(Graphic.color, Color.clear, Time.deltaTime);
            T.color = Color.Lerp(T.color, Color.clear, Time.deltaTime);
            T.transform.localScale = Vector3.one * Mathf.Lerp(T.transform.localScale.x,0, Time.deltaTime);
        }
    }
}


public class ToastVisual : MonoBehaviour 
{
    public Image Image;
    public Text Title;
    public Text Body;
    public Text Action1;
    public Text Action2;

}
[System.Serializable]
public class Toast 
{
    public string ImagePath;
    public string Title;
    public string Body;

    public string Action1;
    public string Action2;

    public float Time;
    public float Importance;
    public ToastPurpose Purpose;

    public enum ToastPurpose//bad name
    {
        Discord,
        Invite,
        Request,
    }

	public Toast Clone()
	{
        Toast T = new Toast();
        T.ImagePath = ImagePath;
        T.Title = Title;
        T.Body = Body;
        T.Action1 = Action1;
        T.Action2 = Action2;
        T.Time = Time;
        T.Importance = Importance;
        T.Purpose = Purpose;
        return T;
	}
}
