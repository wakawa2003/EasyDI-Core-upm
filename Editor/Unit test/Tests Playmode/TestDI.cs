using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace EasyDI.UnitTest
{

    public class TestDI
    {

        public static characterController characterController;
        public static gunController gunController;

        [UnityTest, SetupUnitTestEasyDI]
        public IEnumerator TestDIWithMainProblem()
        {
            yield return new WaitForSeconds(0.5f);

            Debug.Log($"-----------start check--------------");

            Debug.Log($"character health after decore: {(characterController as iHealth).Health}");
            Debug.Log($"character speed after decore: {characterController.Speed}");

            Assert.That(characterController.stringInMethod, Is.Not.Null.And.Not.Empty,
                "Injection failed for stringInMethod.");
            Debug.Log("inject stringInMethod complete!!");


            Assert.That(characterController.StringProperties1, Is.Not.Null.And.Not.Empty,
                "Injection failed for StringProperties1.");
            Debug.Log("inject StringProperties1 complete!!");


            Assert.That(characterController.stringFieldTag1, Is.Not.Null.And.Not.Empty,
                "Injection failed for stringFieldTag1.");
            Debug.Log("inject stringFieldTag1 complete!!");


            //for character
            Assert.That(characterController.StringProperties1, Is.Not.EqualTo(projectInstaller.stringDefault),
                "Scene value did not override the project context value for StringProperties1.");
            Debug.Log("inject overide project context value complete!!");

            Assert.That(characterController.stringFieldTag2, Is.EqualTo(sceneInstaller.stringInstallForTag2),
                "Scene value was not injected for stringFieldTag2.");
            Debug.Log("inject  overide project context value complete!!");

            Assert.That(characterController.stringInMethod, Is.Not.EqualTo(projectInstaller.stringDefault),
                "Scene value did not override the project context value for stringInMethod.");
            Debug.Log("inject overide project context value complete!!");

            Assert.That(characterController.stringFieldSingletonHasTag, Is.EqualTo(sceneInstaller.stringTagForTagSingleton),
                "Singleton value was not injected for stringFieldSingletonHasTag.");
            Debug.Log("inject inject singleton complete!!");

            Assert.That(characterController.intInmethod, Is.EqualTo(characterInstaller.intInMethod),
                "Singleton value was not injected for intInmethod.");
            Debug.Log("inject inject singleton complete!!");

            //for gun
            Assert.That(gunController.stringField, Is.Not.Null.And.Not.Empty,
                "Injection failed for gunController.stringField.");
            Debug.Log("inject gunController.stringField complete!!");

            Assert.That(gunController.characterOwner, Is.Not.Null,
                "Injection failed for gunController.characterOwner.");
            Debug.Log("inject gunController.characterOwner complete!!");

            Assert.That(gunController.stringField, Is.Not.EqualTo(projectInstaller.stringDefault),
                "Scene value did not override the project context value for gunController.stringField.");
            Debug.Log("inject gunController.stringField complete!!");


            //yield return new WaitForSeconds(0.5f);

            //check singleton inject
            Assert.That(characterController.classIsSingleton, Is.SameAs(gunController.classIsSingleton),
                "Singleton injection returned different instances.");
            Debug.Log("inject singleton inject complete!!");


            Assert.That(characterController.Speed,
                Is.EqualTo(sceneInstaller.buffSpeedValue + characterInstaller.buffSpeedValue1 * 2 + characterInstaller.buffSpeedValue2),
                "Decorator speed value is incorrect.");
            Debug.Log("inject decorator buffSpeed complete!!");


            Assert.That((characterController as iHealth).Health, Is.EqualTo(sceneInstaller.buffHeallValue),
                "Decorator health value is incorrect.");
            Debug.Log("inject decorator Health complete!!");

            Debug.Log("characterController decorator List: ");
            foreach (var item in (characterController as iHealth).ToListDecore())
            {
                Debug.Log($"        {item.GetType()} hash: {item.GetHashCode()}");
            }
            Assert.That(testGetDeliverClass(), Is.True, "Test Get Deliver Class failed.");
            Debug.Log("Test Get Deliver Class complete!!");
            Debug.Log($"--------------end check--------------");
        }

        static bool testGetDeliverClass()
        {
            DerivedClass derived = new DerivedClass();
            List<MemberInfo> outL = new List<MemberInfo>();
            List<InjectAttribute> aaa = new List<InjectAttribute>();
            ContextBase.GetAllMemberNeedInject(derived.GetType(), ref outL, ref aaa);

            //Debug.Log("Test Get Deliver Class:");
            //foreach (var member in outL)
            //{
            //    Debug.Log($"{member.MemberType}: {member.Name}");
            //}
            //Debug.Log("----------------");
            return outL.Count == 3;
        }
        class BaseClass
        {
            [Inject] public int BaseField;
            [Inject] public void BaseMethod() { }
        }

        class DerivedClass : BaseClass
        {
            public new int BaseField;
            [Inject] public void DerivedMethod() { }
        }
        public class SetupUnitTestEasyDIAttribute : NUnitAttribute, IOuterUnityTestAction
        {
            public IEnumerator BeforeTest(ITest test)
            {
                yield return setupProjectContext();
                yield return setupSceneContext();
                yield return setupGameObject();
                yield return setupIngameControllerTest();
            }

            public IEnumerator AfterTest(ITest test)
            {
                Debug.Log($"End test!!");
                yield return null;
            }

            IEnumerator setupProjectContext()
            {
                Scene scene = SceneManager.CreateScene($"Scene test {SceneManager.loadedSceneCount + 1}");
                SceneManager.SetActiveScene(scene);
                Debug.Log($"-setup Project Context");
                var projectCOntext = new GameObject("Project context");
                projectCOntext.gameObject.AddComponent<projectInstaller>();
                projectCOntext.gameObject.AddComponent<ProjectContext>();
                var p = ProjectContext.Ins;
                Assert.That(p, Is.Not.Null, "ProjectContext was not initialized.");
                Assert.That(projectCOntext.gameObject, Is.SameAs(p.gameObject),
                    "ProjectContext was initialized on the wrong GameObject.");
                yield return null;
            }

            IEnumerator setupSceneContext()
            {
                Debug.Log($"--setup Scene Context");
                var obj = new GameObject("Scene context");
                obj.gameObject.AddComponent<sceneInstaller>();
                obj.gameObject.AddComponent<SceneContext>();

                yield return null;
            }

            IEnumerator setupGameObject()
            {
                Debug.Log($"---setup GO");
                var character = new GameObject("character Test 1");
                character.gameObject.AddComponent<characterInstaller>();
                TestDI.characterController = character.gameObject.AddComponent<characterController>();
                character.gameObject.AddComponent<GameObjectContext>();



                Debug.Log($"---setup GUn");
                var gun = new GameObject("Gun Test 1");
                gun.transform.SetParent(character.transform);
                gun.gameObject.AddComponent<gunInstaller>();
                TestDI.gunController = gun.gameObject.AddComponent<gunController>();
                gun.gameObject.AddComponent<GameObjectContext>();

                yield return null;
            }

            IEnumerator setupIngameControllerTest()
            {
                Debug.Log($"setup ingameControllerTest");
                var obj = new GameObject("ingame Controller");
                obj.gameObject.AddComponent<ingameControllerTest>();
                yield return null;
            }
        }
    }

}