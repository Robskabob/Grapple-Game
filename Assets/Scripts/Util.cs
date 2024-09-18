using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
	public static void setAllLayers(this GameObject gm, int layer)
	{
		gm.layer = layer;
		foreach (Transform child in gm.transform)
		{
			gm.layer = layer;
			setAllLayers(child.gameObject, layer);
		}
	}
	public static int SetBitTrue(int mask, int bit)
	{
		return mask | (1 << bit);
	}
	public static int SetBitFalse(int mask, int bit)
	{
		return mask & ~(1 << bit);
	}
	public static bool BitCollison(int maskA, int maskB) 
	{
		//Debug.Log(maskA +" & " + maskB + " = "+ (maskA & maskB) + " | " + ((maskA & maskB) != 0));
		return (maskA & maskB) != 0;
	}
	public static int MakeBitMask(bool[] flags) 
	{
		int mask = 0;
		for (int i = 0; i < flags.Length; i++) {
			if (flags[i]) {
				mask = SetBitTrue(mask,i);
			}
		}
		return mask;
	}
}
