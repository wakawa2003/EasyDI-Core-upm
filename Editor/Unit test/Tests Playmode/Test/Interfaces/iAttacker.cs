using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public interface iAttacker : iDamage, iHealth, IEasyDIDecore<iAttacker>
    {
        public class Temp : iAttacker
        {
            int iDamage._damage { get; set; }
            [Inject] iDamage IEasyDIDecore<iDamage>.Decore { get; set; }
            [Inject] iHealth IEasyDIDecore<iHealth>.Decore { get; set; }
            [Inject] iAttacker IEasyDIDecore<iAttacker>.Decore { get; set; }
            iDamage IEasyDIDecore<iDamage>.PrevDecore { get; set; }
            iHealth IEasyDIDecore<iHealth>.PrevDecore { get; set; }
            iAttacker IEasyDIDecore<iAttacker>.PrevDecore { get; set; }
            int iHealth._health { get; set; }
            int iHealth._maxHealth { get; set; }
        }
    }
}
