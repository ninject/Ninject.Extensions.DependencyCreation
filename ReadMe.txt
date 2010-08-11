This module is used to create a dependency toghether with another object without having a reference to it.
It is used to completely decouple components and is normaly used togetter with an event broker for 
comunication bethween these components. 

Example:
this.kernel.Bind<IParent>().To<Parent>().DefinesNamedScope(ScopeName);
this.kernel.DefineDependency<IParent, Dependency>();
this.kernel.Bind<Dependency>().ToSelf().InNamedScope(ScopeName);

Parent parent = this.kernel.Get<Parent>();

This will create the Dependency togetter with the Parent. An they will both be destroyed when parent is
collected by the garbage collector. Normaly, you would attach them both to an event broker so that they
can communicate without knowing each others existence for complete decoupling of these components.