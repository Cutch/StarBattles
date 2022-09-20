using DG.Tweening;
using log4net.Util;
using StarBattles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : DamageObject
{
    protected override void Begin()
    {
        transform.DOScaleY(1, 0.1f);
    }
    protected override void End()
    {
        transform.DOScaleY(0, 0.05f).OnComplete(() => Destroy(gameObject));
       
    }
}
