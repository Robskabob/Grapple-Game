using UnityEngine;

public interface SavableFolder<T> where T : MonoBehaviour
{
    T This { get;}
}