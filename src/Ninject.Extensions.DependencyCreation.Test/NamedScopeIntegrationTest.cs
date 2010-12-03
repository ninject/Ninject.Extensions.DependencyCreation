//-------------------------------------------------------------------------------
// <copyright file="NamedScopeIntegrationTest.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 bbv Software Services AG
//   Author: Remo Gloor remo.gloor@bbv.ch
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Extensions.DependencyCreation
{
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Extensions.ContextPreservation;
    using Ninject.Extensions.DependencyCreation.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ninject.Extensions.NamedScope;
    using Assert = AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using Ninject.Extensions.NamedScope;
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Extensions.DependencyCreation.MSTestAttributes;
    using Ninject.Extensions.NamedScope;
    using Xunit;
    using Xunit.Should;
#endif

    /// <summary>
    /// Integration Test for Dependency Creation Module.
    /// </summary>
    [TestClass]
    public class NamedScopeIntegrationTest
    {
        /// <summary>
        /// The Name of the scope used in the tests.
        /// </summary>
        private const string ScopeName = "Scope";

        /// <summary>
        /// The kernel used in the tests.
        /// </summary>
        private IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedScopeIntegrationTest"/> class.
        /// </summary>
        public NamedScopeIntegrationTest()
        {
            this.SetUp();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NamedScopeIntegrationTest"/> class.
        /// </summary>
        ~NamedScopeIntegrationTest()
        {
            this.kernel.Dispose();
        }

        /// <summary>
        /// Sets up all tests.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
#if !SILVERLIGHT
            this.kernel = new StandardKernel(new NinjectSettings { LoadExtensions = false });
#else
            this.kernel = new StandardKernel();
#endif
            this.kernel.Load(new ContextPreservationModule());
            this.kernel.Load(new NamedScopeModule());
            this.kernel.Load(new DependencyCreationModule());
        }

        /// <summary>
        /// Dependencies are disposed with their parent.
        /// </summary>
        [Fact]
        public void DependencyDisposedWithParent()
        {
            this.kernel.Bind<Parent>().ToSelf().DefinesNamedScope(ScopeName);
            this.kernel.DefineDependency<Parent, Dependency>();

            IList<Dependency> dependencies = new List<Dependency>();
            this.kernel.Bind<Dependency>().ToMethod(ctx => { dependencies.Add(new Dependency()); return dependencies.Last(); }).InNamedScope(ScopeName);

            var parent1 = this.kernel.Get<Parent>();
            var parent2 = this.kernel.Get<Parent>();
            dependencies[0].IsDisposed.ShouldBeFalse();
            dependencies[1].IsDisposed.ShouldBeFalse();

            parent2.Dispose();
            dependencies[0].IsDisposed.ShouldBeFalse();
            dependencies[1].IsDisposed.ShouldBeTrue();

            parent1.Dispose();
            dependencies[0].IsDisposed.ShouldBeTrue();
        }
    }
}