using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

public class High : MonoBehaviour
{
    //public Volume V;
    //public VolumeProfile P;
    //public PaniniProjection PP;
    //public ColorAdjustments CA;
    //public ShadowsMidtonesHighlights SMH;
    //public LiftGammaGain LGG;
    public float Mult = 1;
    public float Mult4 = .1f;
    public float Drag4 = 5;
    public float Drag = 1.1f;
    public float Velocity = 0;

    public Vector4 SVel;
    public Vector4 MVel;
    public Vector4 HVel;
    public Vector4 LVel;
    public Vector4 GaVel;
    public Vector4 GnVel;
    // Start is called before the first frame update
    //void Start()
    //{
    //    P.TryGet(out PP);
    //    P.TryGet(out CA);
    //    P.TryGet(out SMH);
    //    P.TryGet(out LGG);
    //    SMH.shadows.value = Vector4.zero;
    //    SMH.midtones.value = Vector4.zero;
    //    SMH.highlights.value = Vector4.zero;
    //    LGG.lift.value = Vector4.zero;
    //    LGG.gamma.value = Vector4.zero;
    //    LGG.gain.value = Vector4.zero;
    //}
    //
    //// Update is called once per frame
    //void Update()
    //{
    //    Velocity += (Random.value - .5f);
    //    Velocity /= Drag;
    //    if (Mathf.Abs(CA.hueShift.value) == 180) 
    //    {
    //        CA.hueShift.value *= -1;
    //    }
    //    SMH.shadows.value=Shift4(SMH.shadows.value,ref SVel);
    //    SMH.midtones.value=Shift4(SMH.midtones.value,ref MVel);
    //    SMH.highlights.value=Shift4(SMH.highlights.value,ref HVel);
    //    LGG.lift.value=Shift4(LGG.lift.value,ref LVel);
    //    LGG.gamma.value=Shift4(LGG.gamma.value,ref GaVel);
    //    LGG.gain.value=Shift4(LGG.gain.value,ref GnVel);
    //
    //    CA.hueShift.value += Velocity * Mult;
    //}

    Vector4 Rand4() 
    {
        return new Vector4(Random.value - .5f, Random.value - .5f, Random.value - .5f, Random.value - .5f);
    }

    public Vector4 Shift4(Vector4 Value,ref Vector4 vel) 
    {
        vel += Rand4();
        vel -= Value / Drag4;
        vel /= Drag4;
        return Value + vel * Mult4;
    }
}
