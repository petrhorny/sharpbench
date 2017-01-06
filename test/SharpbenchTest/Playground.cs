using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace SharpbenchTest
{
    /// <summary>
    /// Only for manual testing purposes and experiments.
    /// </summary>
    [TestFixture]
    public class Playground
    {
        private class Foo
        {
            public string PublicValue { get; set; }
            public string PrivateValue { get; internal set; }
        }

        [Test]
        public void ManualTest()
        {
            var obj = new Foo();
            obj.PublicValue = "public";
            obj.PrivateValue = "private";
            var json = JsonConvert.SerializeObject(obj);            
            var foo2 = JsonConvert.DeserializeObject<Foo>(json);
        }     
    }
}
