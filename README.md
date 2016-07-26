# AutoDI
A framework for simplifying the creation of objects that only partially depend on resources in your favorite DI container.

It is availible on [Nuget](https://www.nuget.org/packages/AutoDI.Fody/)


This is not another [DI](https://en.wikipedia.org/wiki/Dependency_injection) container, rather its intent is to make interacting with your favorite DI container easier. It allows you to decorate constructor arguments so they are automatically resolved by your DI container. This also enables constructors that take in some initialization data, in addition to the service dependencies that are provided by the DI contianer.


## Examples

```C#
public class MyClass()
{
  public MyClass([Dependency] IService service = null)
  {
    if (service == null) throw new ArgumentNullException(nameof(service));
  }
}
```

## Setup 
Since AutoDI relies on a DI container fist make sure that you have your favorite DI container setup.

To use AutoDI there are only a few simple steps:
1. Install AutoDI from the [Nuget](https://www.nuget.org/packages/AutoDI.Fody/). Because this is a Fody extension it will add a FodyWeavers.xml file to your project that looks something like this:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <AutoDI />
</Weavers>
```
2. Create a bridge between AutoDI and your DI container by creating a new class that derives from AutoDI.IDependencyResolver. In the Resolve method simply invoke your DI container to resolve the dependency. 
3. Decorate the constructor arguments on any constructor where you want AutoDI to fill in the dependency (See examples below).

## Customization and features
AutoDI has several extension points that allow for a great deal of customization and control.
- DependencyResolver.Set can also take in an IGetResolver behavior. This interface allows you to return a different IDependencyResolver based on the requested type and services.
- DependencyAttribute's constructor takes in a params array of values. These values are passed the the IDependencyResolver.Resolve method allowing you to pass configuration keys or additional values to help resolve the instance.

## How does it work
It uses [Fody](https://github.com/Fody/Fody) to perform IL weaving, on your assembly. It effectively changes a constrctor like this:
```C#
public class MyClass
{
  private readonly IService _service;
  public MyClass([Dependency("Extra Data") IService service = null)
  {
    _service = service;
  }
}
```
into:
```C#
public class MyClass
{
  private readonly IService _service;
  public MyClass([Dependency("Extra Data") IService service = null)
  {
    var resolverRequest = new AutoDI.ResolverRequest(typeof(MyClass), new[]{typeof(IService)});
    AutoDI.IDependencyResolver resolver = AutoDI.DependencyResolver.Get(resolverRequest);
    if (resolver != null)
    {
      if (service == null)
      {
        service = resolver.Resolve<IService>("Extra Data");
      }
    }
    _service = service;
  }
}
```

## Limitations
Again, this is a *not* an IL weaving DI container. It is merely a bridge that allows for using simple syntax for instantiating objects. 
- Because it works by IL weaving extra commands into the constructor and requires you to opt-in by decorating constructor arguments, it is only possible to use this on asemblies that you can edit and compile. 
Since attribute can only contain constant values, it is not possible to pass run-time values to the IDependencyResolver.Resolve method.
- It only checks the dependency parameters for null when choosing whether to resolve them or not. So dependency parameters that do not default to null will never be resolved.
