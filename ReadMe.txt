This module is used to create a dependency toghether with another object without having a reference to it. It is used to completely decouple components and is normaly used togetter with an event broker for comunication between these components. 

Example:
this.kernel.Bind<IParent>().To<Parent>();
this.kernel.DefineDependency<IParent, Dependency>();
this.kernel.Bind<Dependency>().ToSelf().InCreatorScope();

Parent parent = this.kernel.Get<Parent>();

This will create the Dependency togetter with the Parent. And they will both be destroyed when parent is collected by the garbage collector. Normally, you would attach them both to an event broker so that they can communicate without knowing each others existence for complete decoupling of these components.

Passing the creator to the dependency
-------------------------------------
There are some very rare cases where the dependency gets created before the one using them. E.g. there are UI frameworks that create the view automatically but because a MV(V)P should be used the presenter has to be created with the view and the view needs to be passed:

this.kernel.Bind<View>().ToSelf();
this.kernel.DefineDependency<View, Presenter();
this.kernel.Bind<Presenter>().ToSelf().WithCreatorAsConstructorArgument("view").InCreatorScope();

public class Presenter
{
    private IView view;
	public Presenter(IView view) { this. view = view; }
}

This will create a new Presenter whenever a new View is created and passes the view into the presenter using the given parameter name ("view"). Note that the presenter will get a proxy to the view which forwards all calls to the view. The proxy has only a weak reference to the view in order that the view can be released.

NOTE: Only use this if you can't create the presenter first. Try all to change the creation order. This is only the last way if you can't influence the creation order.
