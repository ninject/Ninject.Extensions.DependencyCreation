//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationDirective.cs" company="bbv Software Services AG">
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

namespace Ninject.Extensions.DependencyCreation
{
    using System;
    using Ninject.Planning.Directives;

    /// <summary>
    /// This directive defines that upon activation of the object a dependency has to be created.
    /// </summary>
    public class DependencyCreationDirective : IDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCreationDirective"/> class.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency.</param>
        public DependencyCreationDirective(Type dependencyType)
        {
            this.DependencyType = dependencyType;
        }

        /// <summary>
        /// Gets the type of the dependency.
        /// </summary>
        /// <value>The type of the dependency.</value>
        public Type DependencyType { get; private set; }
    }
}