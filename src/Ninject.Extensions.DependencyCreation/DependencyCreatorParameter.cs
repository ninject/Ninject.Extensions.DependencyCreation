//-------------------------------------------------------------------------------
// <copyright file="DependencyCreatorParameter.cs" company="bbv Software Services AG">
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
    using System;
    using Ninject.Parameters;

    /// <summary>
    /// This parameter contains the instance of the binding that requested the creation of an dependency.
    /// </summary>
    public class DependencyCreatorParameter : Parameter
    {
        /// <summary>
        /// The instance of the object requesting a dependency.
        /// </summary>
        private readonly WeakReference creator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyCreatorParameter"/> class.
        /// </summary>
        /// <param name="creator">The instance of the object requesting a dependency.</param>
        public DependencyCreatorParameter(object creator) : base("DependencyCreator", ctx => null, false)
        {
            this.creator = new WeakReference(creator);
        }

        /// <summary>
        /// Gets the instance of the object requesting a dependency.
        /// </summary>
        /// <value>The instance of the object requesting a dependency.</value>
        public object Creator
        {
            get
            {
                return this.creator.IsAlive ? this.creator.Target : null;
            }
        }

    }
}