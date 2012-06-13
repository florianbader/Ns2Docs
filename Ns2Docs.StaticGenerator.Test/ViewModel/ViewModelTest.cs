using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ns2Docs.StaticGenerator.ViewModel;

namespace Ns2Docs.StaticGenerator.Test.ViewModel
{
    [TestFixture]
    public class ViewModelTest
    {
        [TestCase]
        public void Indexing()
        {
            StringViewModel viewModel = new StringViewModel("hello");
            int hash = "hello".GetHashCode();

            Assert.AreEqual(hash, viewModel["Hash"]);
            Assert.AreEqual("hello".Length, viewModel["Length"]);
            
            Uri a = new Uri("http://example.com/tables/Absorb/");
            Uri b = new Uri("http://example.com/tables/");

            Uri r = a.MakeRelativeUri(b);
            Assert.AreEqual("Meow/", Uri.UnescapeDataString(r.ToString()));
        }
    }
}
