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
    public class MethodTest
    {
        [TestCase]
        public void QualifiedName()
        {
            MockRepository mocks = new MockRepository();
            #region Weapon
            ITable weapon = MockRepository.GenerateStub<ITable>();
            weapon.Stub(x => x.Name).Return("Weapon");
            #endregion

            #region Parameters
            IParameter amount = MockRepository.GenerateStub<IParameter>();
            amount.Stub(x => x.Name).Return("amount");


            IParameter speed = MockRepository.GenerateStub<IParameter>();
            speed.Stub(x => x.Name).Return("speed");

            IParameter time = MockRepository.GenerateStub<IParameter>();
            time.Stub(x => x.Name).Return("time");
            #endregion

            Method setCameraShake = new Method(weapon, "SetCameraShake");
            setCameraShake.Parameters.Add(amount);
            setCameraShake.Parameters.Add(speed);
            setCameraShake.Parameters.Add(time);

            string qualifiedName = setCameraShake.QualifiedName;
            
            Assert.AreEqual("Weapon:SetCameraShake(amount, speed, time)", qualifiedName);
        }

        [TestCase]
        public void Overrides()
        {
            ITable baseTable = MockRepository.GenerateStub<ITable>();
            Method doSomething_baseTable = new Method(baseTable, "DoSomething");
            baseTable.BaseTable = null;
            baseTable.Expect(x => x.Methods).Return(new Method[] { doSomething_baseTable });

            ITable childTable = MockRepository.GenerateStub<ITable>();
            Method doSomething = new Method(childTable, "DoSomething");
            childTable.BaseTable = baseTable;

            IMethod overrides = doSomething.Overrides;

            Assert.AreEqual(doSomething_baseTable, overrides);
        }
    }
}
