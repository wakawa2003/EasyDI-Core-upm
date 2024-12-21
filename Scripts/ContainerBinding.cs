﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyDI
{
    /// <summary>
    /// Only save binding!
    /// </summary>
    public class ContainerBinding
    {
        public ContainerBinding(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public Dictionary<string, BindInfor> Dict_InjectName_And_BindInfor { get; private set; } = new Dictionary<string, BindInfor>();
        public Dictionary<string, List<BindInfor>> Dict_ListBindInforDecore { get; private set; } = new();

        public BindReturn<t> Bind<t>(string tag = "")
        {
            BindReturn<t> bindReturn = CreatNewBind<t>(tag, false);
            return bindReturn;
        }

        private BindReturn<t> CreatNewBind<t>(string tag, bool isDecore)
        {
            Type typeT = typeof(t);
            BindInfor bindInfor = new BindInfor(typeT);
            bindInfor.IsDecore = isDecore;
            var bindReturn = new BindReturn<t>(bindInfor);
            var key = EasyDIUltilities.BuildKeyInject(typeT, tag);
            AddBinding(key, bindInfor, bindInfor.IsDecore);
            return bindReturn;
        }

        public BindReturn<t> Decore<t>(string tag = "") where t : IEasyDIDecore<t>
        {
            return CreatNewBind<t>(tag, true);
        }

        public void AddBinding(string injectKey, BindInfor bindInfor, bool isDecore)
        {
            if (!isDecore)
            {
                if (Dict_InjectName_And_BindInfor.ContainsKey(injectKey))
                {
                    EasyDILog.LogError($"Exist more than one \"{injectKey}\" binding in \'{Name}\' container!");
                }
                else
                {
                    Dict_InjectName_And_BindInfor.Add(injectKey, bindInfor);
                }
            }
            else
            {
                if (Dict_ListBindInforDecore.ContainsKey(injectKey))
                {
                    if (Dict_ListBindInforDecore[injectKey] == null)
                        Dict_ListBindInforDecore[injectKey] = new List<BindInfor>();
                    Dict_ListBindInforDecore[injectKey].Add(bindInfor);
                }
                else
                {
                    Dict_ListBindInforDecore.Add(injectKey, new List<BindInfor> { bindInfor });
                }
            }
        }

    }


    public class BindInfor
    {
        Type typeTarget { get; }
        /// <summary>
        /// Not Null when set in binding.
        /// </summary>
        object objectData;
        public BindInfor(Type typeTarget)
        {
            this.typeTarget = typeTarget;
        }
        private BindInfor()
        {
            var a = ID;//get ID
        }
        /// <summary>
        /// Type of t parameter when set up.
        /// </summary>
        public Type TypeTarget => typeTarget;
        public bool IsDecore { get; set; } = false;

        public object ObjectData
        {
            get => objectData; set => objectData = value;
        }


        string id;
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                    id = this.GetHashCode().ToString();
                return id;
            }
        }
        public EGetInstanceMethod GetInstanceMethod { get; set; }
        public EnumTreatWithInstanceMethod TreatWithInstanceMethod { get; set; }
        /// <summary>
        /// object: instance object.
        /// member: member need inject in object
        /// </summary>
        public Func<object, MemberInfo, object> CustomGetInstancePredict { get; set; }
        public Func<object, MemberInfo, bool> WherePredict { get; set; } = delegate { return true; };//default is alway true

        //object getObjectData(object instance)
        //{
        //    CustomGetInstancePredict.
        //}

        /// <summary>
        /// Method how to get instance.
        /// </summary>
        public enum EGetInstanceMethod
        {
            UnSet, OnlyThisGameObject, ItSelfAndComponentInChild, ItSelfAndComponentInParent
        }

        /// <summary>
        /// Method how to treat instance after get it.
        /// </summary>
        public enum EnumTreatWithInstanceMethod
        {
            UnSet, Singleton, Transient
        }

    }
    public class BindReturn<a>
    {
        BindInfor bindInfor;

        //constructor
        public BindReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

        public ToReturn<a, b> To<b>() where b : a
        {
            return new ToReturn<a, b>(bindInfor);
        }
    }
    public class ToReturn<t, a>
    {
        BindInfor bindInfor;
        FromReturn<a> fromReturn;

        //constructor
        public ToReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
            fromReturn = new FromReturn<a>(bindInfor);
        }

        public FromReturn<a> FromComponentInChild()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInChild;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponentInChildren<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }

        public FromReturn<a> FromThisGameObject()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.OnlyThisGameObject;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponent<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }


        public FromReturn<a> FromThisAndParent()
        {
            bindInfor.GetInstanceMethod = BindInfor.EGetInstanceMethod.ItSelfAndComponentInParent;
            bindInfor.CustomGetInstancePredict = (ins, member) =>
            {
                if (ins is MonoBehaviour)
                {
                    return (ins as MonoBehaviour).GetComponentInParent<t>();
                }
                else
                {
                    EasyDILog.LogError($"{ins.GetType().Name} is Not Monobehaviour!!!!");
                }
                return null;
            };
            return fromReturn;
        }

        public FromReturn<a> FromInstance(a value)
        {
            bindInfor.ObjectData = value;
            return fromReturn;
        }


        /// <summary>
        /// <see langword="object"/> : instance object
        /// <para> <see langword="MemberInfo"/> : member </para>
        /// </summary>
        /// <param name="func"></param>
        public FromReturn<a> Where(Func<object, MemberInfo, bool> func)
        {
            bindInfor.WherePredict = func;
            return fromReturn;
        }

        /// <summary>
        /// <see langword="object"/> : instance object
        /// <para> <see langword="MemberInfo"/> : member </para>
        /// </summary>
        /// <param name="func"></param>
        public FromReturn<a> CustomGetInstance(Func<object, MemberInfo, object> func)
        {
            bindInfor.CustomGetInstancePredict = func;
            return fromReturn;
        }
    }

    public class FromReturn<a>
    {
        BindInfor bindInfor;

        //constructor
        public FromReturn(BindInfor bindInfor)
        {
            this.bindInfor = bindInfor;
        }

        public void AsSingleton()
        {
            bindInfor.TreatWithInstanceMethod = BindInfor.EnumTreatWithInstanceMethod.Singleton;
        }
        public void AsTransient()
        {
            bindInfor.TreatWithInstanceMethod = BindInfor.EnumTreatWithInstanceMethod.Transient;
        }
    }

}
