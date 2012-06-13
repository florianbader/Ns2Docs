using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ns2Docs.Tests
{
    public abstract class TestBase
    {
        [SetUp]
        public void SetUp()
        {
            Arrange();
            Act();
        }

        protected virtual void Arrange()
        {}

        protected abstract void Act();
    }
}
