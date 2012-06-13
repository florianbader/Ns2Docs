using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.Spark;
using Rhino.Mocks;

namespace Ns2Docs.Model.Test.Spark
{
    [TestFixture]
    public class TableTest
    {
        private MockRepository mocks;

        [SetUp]
        public void CreateMocks()
        {
            mocks = new MockRepository();
        }

        [TestCase]
        public void InheritanceChain()
        {
            Table baseTable = new Table("BaseTable");
            Table middleTable = new Table("MiddleTable", baseTable);
            Table bottomTable = new Table("BottomTable", middleTable);

            IEnumerable<ITable> bottomInheritanceChain = bottomTable.InheritanceChain();
            
            Assert.AreEqual(
                new ITable[] {
                    baseTable,
                    middleTable,
                    bottomTable
                }, bottomInheritanceChain.ToArray());
            
        }

        [TestCase]
        public void AllMixins__Return_An_Enumerable_Containing_All_Of_The_Mixins_Used_By_The_Table()
        {
            Table baseTable = new Table("BaseTable");
            Table someMixin = new Table("SomeMixin");
            baseTable.Mixins.Add(someMixin);

            Table childTable = new Table("ChildTable", baseTable);
            Table anotherMixin = new Table("AnotherMixin");
            childTable.Mixins.Add(anotherMixin);

            IEnumerable<ITable> allMixins = childTable.AllMixins();

            Assert.Contains(anotherMixin, allMixins.ToArray());
            Assert.Contains(someMixin, allMixins.ToArray(), "Include mixins from base classes");
        }

        [TestCase]
        public void AllMethods()
        {
            Table weapon = new Table("Weapon");
            Table clipWeapon = new Table("ClipWeapon", weapon);
            
            IMethod getCanIdle = MockRepository.GenerateStub<IMethod>();
            getCanIdle.Expect(x => x.ExistsOnClient).Return(true);
            getCanIdle.Expect(x => x.ExistsOnServer).Return(true);
            weapon.Methods.Add(getCanIdle);

            Assert.Contains(getCanIdle, clipWeapon.AllMethods().ToArray());
        }

        [TestCase]
        public void AllMethods__Ignore_Overridden_Methods()
        {
            Table weapon = new Table("Weapon");
            Table clipWeapon = new Table("ClipWeapon", weapon);

            IMethod getCanIdle_Weapon = MockRepository.GenerateStub<IMethod>();
            getCanIdle_Weapon.Expect(x => x.Name).Return("GetCanIdle");
            getCanIdle_Weapon.Expect(x => x.ExistsOnServer).Return(true);
            weapon.Methods.Add(getCanIdle_Weapon);

            IMethod getCanIdle_ClipWeapon = MockRepository.GenerateStub<IMethod>();
            getCanIdle_ClipWeapon.Expect(x => x.Name).Return("GetCanIdle");
            getCanIdle_ClipWeapon.Expect(x => x.ExistsOnServer).Return(true);
            clipWeapon.Methods.Add(getCanIdle_ClipWeapon);

            IMethod[] allMethods = clipWeapon.AllMethods().ToArray();

            Assert.Contains(getCanIdle_ClipWeapon, allMethods);
            Assert.AreEqual(1, allMethods.Length, "Only contain ClipWeapon:CanIdle() because Weapon:CanIdle() is overridden.");
            Assert.IsFalse(allMethods.Contains(getCanIdle_Weapon));
        }

        [TestCase]
        public void AllMethods__Use_Nearest_Parent_Method_For_Methods_That_Are_Not_Overridden()
        {
            Table scriptActor = new Table("ScriptActor");
            Table weapon = new Table("Weapon", scriptActor);
            Table clipWeapon = new Table("ClipWeapon", weapon);

            IMethod onCreate_ScriptActor = MockRepository.GenerateStub<IMethod>();
            onCreate_ScriptActor.Expect(x => x.Name).Return("OnCreate");
            onCreate_ScriptActor.Expect(x => x.Table).Return(scriptActor);
            onCreate_ScriptActor.Expect(x => x.ExistsOnServer).Return(true);
            scriptActor.Methods.Add(onCreate_ScriptActor);

            IMethod onCreate_Weapon = MockRepository.GenerateStub<IMethod>();
            onCreate_Weapon.Expect(x => x.Name).Return("OnCreate");
            onCreate_Weapon.Expect(x => x.Table).Return(weapon);
            onCreate_Weapon.Expect(x => x.ExistsOnServer).Return(true);
            weapon.Methods.Add(onCreate_Weapon);
            
            IEnumerable<IMethod> allMethods = clipWeapon.AllMethods();

            Assert.AreEqual(1, allMethods.Count());
            Assert.AreEqual(onCreate_Weapon, allMethods.First());
            
        }

        [TestCase]
        public void AllMethods__Include__Mixin__Methods()
        {
            Table mac = new Table("MAC");
            Table upgradeableMixin = new Table("UpgradableMixin");
            mac.Mixins.Add(upgradeableMixin);

            IMethod getUpgrades = MockRepository.GenerateStub<IMethod>();
            getUpgrades.Expect(x => x.Table).Return(upgradeableMixin);
            getUpgrades.Expect(x => x.Name).Return("GetUpgrades");
            getUpgrades.Expect(x => x.ExistsOnServer).Return(true);
            upgradeableMixin.Methods.Add(getUpgrades);

            Assert.AreEqual(getUpgrades, mac.AllMethods().First());
        }

        [TestCase]
        public void AllMethods__Dont_Include___initmixin_From_Parent_Tables()
        {
            Table cls = new Table("Class");
            Table mixin = new Table("Mixin");
            cls.Mixins.Add(mixin);

            IMethod __initmixin = MockRepository.GenerateStub<IMethod>();
            __initmixin.Expect(x => x.Table).Return(mixin);
            __initmixin.Expect(x => x.Name).Return("__initmixin");
            __initmixin.Expect(x => x.ExistsOnServer).Return(true);
            mixin.Methods.Add(__initmixin);

            IMethod[] allMethods = cls.AllMethods().ToArray();

            Assert.IsEmpty(cls.AllMethods().ToArray());
        }

        [TestCase]
        public void AllMethods__Dont_Include___prepareclass_From_Parent_Tables()
        {
            Table cls = new Table("Class");
            Table mixin = new Table("Mixin");
            cls.Mixins.Add(mixin);

            IMethod __prepareclass = MockRepository.GenerateStub<IMethod>();
            __prepareclass.Expect(x => x.Table).Return(mixin);
            __prepareclass.Expect(x => x.Name).Return("__initmixin");
            __prepareclass.Expect(x => x.ExistsOnServer).Return(true);
            mixin.Methods.Add(__prepareclass);

            IMethod[] allMethods = cls.AllMethods().ToArray();

            Assert.IsEmpty(cls.AllMethods().ToArray());
        }
    }
}
