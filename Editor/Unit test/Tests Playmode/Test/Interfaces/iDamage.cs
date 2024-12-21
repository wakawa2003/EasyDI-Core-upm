using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public interface iDamage : IEasyDIDecore<iDamage>
    {
        public int _damage { get; set; }
        int Damage { get => (Decore != null ? Decore.Damage : 0) + _damage; set => _damage = value; }

        public class Temp : iDamage
        {
            public int _damage { get; set; }
            [Inject] public iDamage Decore { get; set; }
            public iDamage PrevDecore { get; set; }
        }
    }
}
