//-------------------------------------------------------------------------------
// <copyright file="DependencyCreationExtensionMethods.cs" company="bbv Software Services AG">
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
    using System.Linq;
    using Ninject;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Extension methods for the definition that an object has a dependency.
    /// </summary>
    public static class DependencyCreationExtensionMethods
    {
        /// <summary>
        /// Defines that an object has a dependency to another object. 
        /// This other object is created upon activation.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <typeparam name="TDependency">The type of the dependency.</typeparam>
        /// <param name="kernel">The kernel.</param>
        public static void DefineDependency<TParent, TDependency>(this IKernel kernel)
        {
            kernel.Components.GetAll<IPlanningStrategy>().OfType<DependencyCreationPlanningStrategy>().Single()
                .DefineDependency<TParent, TDependency>();
        }
    }
}