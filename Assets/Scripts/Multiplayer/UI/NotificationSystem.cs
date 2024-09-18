using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NotificationSystem : MonoBehaviour
{
    public List<Toast> Toasts = new List<Toast>();
    public List<float> ToastTimes = new List<float>();
    public List<int> ToastPriorityOrder = new List<int>();
    public List<ToastVisual> ToastVisuals = new List<ToastVisual>();
    public void AddToast(Toast T)
    {
        Toasts.Add(T);
        ToastTimes.Add(T.Time);
        //ToastPriorityOrder.fi(Toasts.Count-1, ProjectionComparer<int>.Create(x => Toasts[a].Importance))//(a,b) => (Toasts[a].Importance > Toasts[b].Importance));
        //int index = ToastPriorityOrder.Count / 2;
        //int chunkSize = ToastPriorityOrder.Count/2;
        //float val = T.Importance;
        //while (chunkSize > 0 && index != 0) 
        //{
        //    //chunkSize /= 2;
        //    Debug.Log($"Val:{val} index:{index} Importance:{((index<0) ? 0 : (Toasts[ToastPriorityOrder[index]].Importance))} V < I = {((index < 0) ? false : (val < Toasts[ToastPriorityOrder[index]].Importance))} Chunk:{chunkSize} ChunkFL:{chunkSize - Mathf.FloorToInt(chunkSize / 2f)} ChunkCL:{chunkSize - Mathf.CeilToInt(chunkSize / 2f)}");
        //    if (val > Toasts[ToastPriorityOrder[index]].Importance) 
        //    {
        //        chunkSize -= Mathf.FloorToInt(chunkSize / 2f);
        //        index += chunkSize;
        //    }
        //	else
        //    {
        //        chunkSize -= Mathf.CeilToInt(chunkSize / 2f);
        //        index -= chunkSize;
        //    }
        //}
        //float key = T.Importance;
        //int min = 0;
        //int max = ToastPriorityOrder.Count - 1;
        //int mid = 0;
        //string str = "";
        //while (min <= max)
        //{
        //    mid = (min + max) / 2;
        //    str += " M:" + mid;
        //    if (key == Toasts[ToastPriorityOrder[mid]].Importance)
        //    {
        //        mid++;
        //        Debug.Log("Complete");
        //        break; 
        //    }
        //    else if (key < Toasts[ToastPriorityOrder[mid]].Importance)
        //    {
        //        max = mid - 1;
        //        str += " x:" + max;
        //    }
        //    else
        //    {
        //        min = mid + 1;
        //        str += " n:" + min;
        //    }
        //}
        //Debug.Log(str);
        //ToastPriorityOrder.Insert(mid,ToastPriorityOrder.Count);

        //var index = ToastPriorityOrder.BinarySearch(ToastPriorityOrder.Count, new ComparisonComparer<int>((a,b) => Toasts[ToastPriorityOrder[a]].Importance.CompareTo(T.Importance)));
        //if (index < 0) index = ~index;
        //Debug.Log(index);
        int index = 0;
        for (int i = 0; i < ToastPriorityOrder.Count; i++) 
        {
            if (T.Importance > Toasts[ToastPriorityOrder[i]].Importance)
			{
                index = i;
                break;
			}
        }
        ToastPriorityOrder.Insert(index, ToastPriorityOrder.Count);
    }
    public class ComparisonComparer<T> : IComparer<T>
    {
        private readonly System.Comparison<T> comparison;

        public ComparisonComparer(System.Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            return comparison(x, y);
        }
    }
    public void RemoveToast(int index)
    {
        Toasts.RemoveAt(index);
        ToastTimes.RemoveAt(index);
        for(int i = 0; i < ToastPriorityOrder.Count; i++) 
        {
			if (ToastPriorityOrder[i] > index) 
            {
                ToastPriorityOrder[i]--;
            }
        }
        ToastPriorityOrder.Remove(index);

    }
    private void Update()
	{
		for(int i = 0; i < Toasts.Count; i++) 
        {
            ToastTimes[i] -= Time.deltaTime;
            if(ToastTimes[i] < 0) 
            {
                RemoveToast(i);
                i--;
            }
        }
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(NotificationSystem))]
public class NotificationSystemInspector : Editor 
{
    NotificationSystem NS;
    Toast Test = new Toast();
    bool testToast;
    bool TostPriority;

    private void OnEnable()
    {
        NS = target as NotificationSystem;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Toast"))
        {
            NS.AddToast(Test.Clone());
        }
        if (GUILayout.Button("Add Random"))
        {
            Toast T = new Toast();
            T.Importance = Random.Range(0, 10f);
            T.Time = Random.Range(1, 10f);
            NS.AddToast(T);
        }
        if (GUILayout.Button("Clear"))
        {
            NS.Toasts.Clear();
            NS.ToastTimes.Clear();
            NS.ToastPriorityOrder.Clear();
            NS.ToastVisuals.Clear();
        }
        testToast = EditorGUILayout.Foldout(testToast,"Toast");
        if (testToast)
        {
            Test.Title = EditorGUILayout.TextField("Title", Test.Title);
            Test.Body = EditorGUILayout.TextField("Body", Test.Body);
            Test.Time = EditorGUILayout.FloatField("Time", Test.Time);
            Test.Importance = EditorGUILayout.FloatField("Importance", Test.Importance);
        }
        TostPriority = EditorGUILayout.Foldout(TostPriority, "Toast Priority");
        if (TostPriority)
        {
            for(int i = 0; i < NS.ToastPriorityOrder.Count; i++) 
            {
                int id = NS.ToastPriorityOrder[i];
                EditorGUILayout.BeginHorizontal();
                id = EditorGUILayout.IntField("ID",id);
                NS.Toasts[id].Importance = EditorGUILayout.FloatField("Import",NS.Toasts[id].Importance);
                EditorGUILayout.EndHorizontal();
                NS.ToastPriorityOrder[i] = id;
            }
        }
    }
}
#endif