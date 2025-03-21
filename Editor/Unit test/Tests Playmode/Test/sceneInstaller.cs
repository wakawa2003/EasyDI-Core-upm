using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyDI.UnitTest
{
    public class sceneInstaller : MonoInstaller
    {
        public static string stringInstallForTag2 = "scene instaler string tag2";
        public static string idSingleton;
        public static string stringTagForTagSingleton = "value for string singleton";
        public static float buffSpeedValue = 3;
        public static int buffHeallValue = 54;

        public override void InstallBinding()
        {
            Debug.Log($"scene installing");
            ContainerBinding.Bind<string>(tags.tag2).To<string>().CustomGetInstance((obj, mem) =>
            {
                return stringInstallForTag2;
            });

            ContainerBinding.Bind<string>(tags.tagSingleton).To<string>().CustomGetInstance((obj, mem) =>
            {
                return stringTagForTagSingleton;
            }).AsSingleton();


            ContainerBinding.Bind<ingameControllerTest>().To<ingameControllerTest>().CustomGetInstance((a, b) =>
            {
                return FindObjectOfType<ingameControllerTest>();
            }).AsSingleton();


            ContainerBinding.Bind<classIsSingleton>().To<classIsSingleton>().CustomGetInstance((a, b) =>
            {
                Debug.Log($"custom get instance in singleton!!");
                idSingleton = GUID.Generate().ToString();
                return new classIsSingleton(idSingleton);
            }).AsSingleton();

            ContainerBinding.Decore<iSpeed>().To<buffSpeedInScene>().CustomGetInstance((a, b) => new buffSpeedInScene());

            //ContainerBinding.Bind<iHealth>().To<iHealth.Temp>().CustomGetInstance((obj, mem) =>
            //{
            //    return new iHealth.Temp();
            //}).AsTransient();

            ContainerBinding.Decore<iHealth>().To<buffHealth>().CustomGetInstance((obj, mem) =>
            {
                return new buffHealth(buffHeallValue);
            }).AsTransient();
            //ContainerBinding.Decore<iHealth>().To<buffHealth>().CustomGetInstance((obj, mem) =>
            //{
            //    return new buffHealth(3);
            //}).AsTransient();

            //ContainerBinding.Bind<iDamage>().To<iDamage.Temp>().CustomGetInstance((obj, mem) =>
            //{
            //    return new iDamage.Temp();
            //}).AsTransient();


            ContainerBinding.Bind<iAttacker>().To<iAttacker.Temp>().CustomGetInstance((obj, mem) =>
            {
                return new iAttacker.Temp();
            }).AsTransient();

            ContainerBinding.Decore<iDamage>().To<buffDamage>().CustomGetInstance((obj, mem) =>
            {
                return new buffDamage();
            }).AsTransient();

            ContainerBinding.Decore<iCharacter>().To<buffCharacter>().CustomGetInstance((obj, mem) =>
            {
                return new buffCharacter();
            }).AsTransient();
        }

        public class buffDamage : iDamage
        {
            int iDamage._damage { get; set; }
            [Inject] iDamage IEasyDIDecore<iDamage>.Decore { get; set; }
            iDamage IEasyDIDecore<iDamage>.PrevDecore { get; set; }
        }

        public class buffCharacter : iCharacter
        {
            public float Speed { get; set; }
            [Inject] public iSpeed Decore { get; set; }
            public iSpeed PrevDecore { get; set; }
            public int _health { get; set; }
            public int _maxHealth { get; set; }
            public int _damage { get; set; }
            [Inject] iHealth IEasyDIDecore<iHealth>.Decore { get; set; }
            [Inject] iAttacker IEasyDIDecore<iAttacker>.Decore { get; set; }
            [Inject] iDamage IEasyDIDecore<iDamage>.Decore { get; set; }
            [Inject] iCharacter IEasyDIDecore<iCharacter>.Decore { get; set; }
            iHealth IEasyDIDecore<iHealth>.PrevDecore { get; set; }
            iAttacker IEasyDIDecore<iAttacker>.PrevDecore { get; set; }
            iDamage IEasyDIDecore<iDamage>.PrevDecore { get; set; }
            iCharacter IEasyDIDecore<iCharacter>.PrevDecore { get; set; }
        }

        public class buffHealth : iHealth
        {

            public buffHealth(int health)
            {
                _health = health;
            }

            public int _health { get; set; }
            public int _maxHealth { get; set; }
            [Inject] iHealth IEasyDIDecore<iHealth>.Decore { get; set; }
            iHealth IEasyDIDecore<iHealth>.PrevDecore { get; set; }
        }

        public class buffSpeedInScene : iSpeed
        {
            [Inject] public iSpeed Decore { get; set; }
            public iSpeed PrevDecore { get; set; }
            public float Speed
            {
                get
                {
                    return Decore == null ? sceneInstaller.buffSpeedValue : Decore.Speed + sceneInstaller.buffSpeedValue;
                }
                set { }
            }

        }


    }
}
