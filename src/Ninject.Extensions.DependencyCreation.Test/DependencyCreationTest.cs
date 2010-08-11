//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationTest.cs" company="bbv Software Services AG">
//   Copyright (c) 2008 bbv Software Services AG
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

namespace Ninject.Extensions.DependencyCreation.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Extensions.NamedScope;
    using Xunit;

    /// <summary>
    /// Integration Test for Dependency Creation Module.
    /// </summary>
    public class DependencyCreationTest
    {
        /// <summary>
        /// The Name of the scope used in the tests.
        /// </summary>
        private const string ScopeName = "Scope";

        /// <summary>
        /// The kernel used in the tests.
        /// </summary>
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCreationTest"/> class.
        /// </summary>
        public DependencyCreationTest()
        {
            this.kernel = new StandardKernel(new NinjectSettings { LoadExtensions = false });
            this.kernel.Load(new NamedScopeModule());
            this.kernel.Load(new DependencyCreationModule());
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="DependencyCreationTest"/> class.
        /// </summary>
        ~DependencyCreationTest()
        {
            this.kernel.Dispose();
        }

        /// <summary>
        /// Test interface for the parent.
        /// </summary>
        private interface IParent
        {
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

            Parent parent1 = this.kernel.Get<Parent>();
            Parent parent2 = this.kernel.Get<Parent>();
            Assert.False(dependencies[0].IsDisposed);
            Assert.False(dependencies[1].IsDisposed);

            parent2.Dispose();
            Assert.False(dependencies[0].IsDisposed);
            Assert.True(dependencies[1].IsDisposed);

            parent1.Dispose();
            Assert.True(dependencies[0].IsDisposed);
        }

        /// <summary>
        /// The dependencies are created even if they are defined by a interface
        /// of the actually created object. 
        /// </summary>
        [Fact]
        public void ParentResolvableByInterface()
        {
            this.kernel.Bind<Parent>().ToSelf().DefinesNamedScope(ScopeName);
            this.kernel.DefineDependency<IParent, Dependency>();

            bool dependencyCreated = false;
            this.kernel.Bind<Dependency>().ToMethod(ctx => { dependencyCreated = true; return new Dependency(); }).InNamedScope(ScopeName);

            this.kernel.Get<Parent>();

            Assert.True(dependencyCreated);
        }

        /// <summary>
        /// Multiple dependencies can be created for an object.
        /// </summary>
        [Fact]
        public void MultipleDependenciesCanBeCreated()
        {
            this.kernel.Bind<Parent>().ToSelf().DefinesNamedScope(ScopeName);
            this.kernel.DefineDependency<IParent, Dependency>();
            this.kernel.DefineDependency<IParent, Dependency2>();

            Dependency dependency1 = null;
            Dependency2 dependency2 = null;
            this.kernel.Bind<Dependency>().ToMethod(ctx => { dependency1 = new Dependency(); return dependency1; }).InNamedScope(ScopeName);
            this.kernel.Bind<Dependency2>().ToMethod(ctx => { dependency2 = new Dependency2(); return dependency2; }).InNamedScope(ScopeName);

            Parent parent = this.kernel.Get<Parent>();
            parent.Dispose();

            Assert.NotNull(dependency1);
            Assert.NotNull(dependency2);
            Assert.True(dependency1.IsDisposed);
            Assert.True(dependency2.IsDisposed);
        }

        /// <summary>
        /// A dependency can have its own dependency.
        /// </summary>
        [Fact]
        public void NestedDependenciesCanBeCreated()
        {
            this.kernel.Bind<Parent>().ToSelf().DefinesNamedScope(ScopeName);
            this.kernel.DefineDependency<IParent, Dependency>();
            this.kernel.DefineDependency<Dependency, Dependency2>();

            Dependency dependency1 = null;
            Dependency2 dependency2 = null;
            this.kernel.Bind<Dependency>().ToMethod(ctx => { dependency1 = new Dependency(); return dependency1; }).InNamedScope(ScopeName);
            this.kernel.Bind<Dependency2>().ToMethod(ctx => { dependency2 = new Dependency2(); return dependency2; }).InNamedScope(ScopeName);

            Parent parent = this.kernel.Get<Parent>();
            parent.Dispose();

            Assert.NotNull(dependency1);
            Assert.NotNull(dependency2);
            Assert.True(dependency1.IsDisposed);
            Assert.True(dependency2.IsDisposed);
        }

        /// <summary>
        /// The parent object.
        /// </summary>
        private class Parent : DisposeNotifyingObject, IParent
        {
        }

        /// <summary>
        /// The first dependency object.
        /// </summary>
        private class Dependency : DisposeNotifyingObject
        {
        }

        /// <summary>
        /// The second dependency object.
        /// </summary>
        private class Dependency2 : DisposeNotifyingObject
        {
        }
    }
}