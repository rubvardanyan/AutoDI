# AutoDI
Have a question? [![Join the chat at https://gitter.im/AutoDIContainer/Lobby](https://badges.gitter.im/AutoDIContainer/Lobby.svg)](https://gitter.im/AutoDIContainer/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![NuGet Status](http://img.shields.io/nuget/v/AutoDI.svg?style=flat&label=AutoDI)](https://www.nuget.org/packages/AutoDI/)
[![NuGet Status](http://img.shields.io/nuget/v/AutoDI.Fody.svg?style=flat&label=AutoDI.Fody)](https://www.nuget.org/packages/AutoDI.Fody/)
[![NuGet Status](http://img.shields.io/nuget/v/AutoDI.AspNetCore.svg?style=flat&label=AutoDI.AspNetCore)](https://www.nuget.org/packages/AutoDI.AspNetCore/)

[![Build status](https://ci.appveyor.com/api/projects/status/ybmv50xxi3lb086o?svg=true)](https://ci.appveyor.com/project/Keboo/autodi)


AutoDI is both a dependency injection container and a framework to simplify working with dependency injection (DI). It is built on top of the [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/). It works very similar to [the way ASP.NET Core handles dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection).

It serves two purposes
1. It builds on the Microsoft.Extensions.DependencyInjection.Abstractions library to bring that same type of dependency injection to other platforms (.NET Framework, WPF, Xamarin, UWP, etc). 
2. It can generate container registrations _at compile time_ using convensions (of course you can also add on more at run-time as well).

See the [wiki](https://github.com/Keboo/AutoDI/wiki) for more details or check out the [Quick Start](https://github.com/Keboo/AutoDI/wiki/Quick-Start) page to get up and running fast.


## Why do I need this?

The goal of this library is to make dependency injection as simple as adding a nuget. 

In addition to standard constructor dependency injection, it also allows you to simply declare optional constructor parameters that will be resolved anytime the constructor is invoked.

In a typical DI setup you can often find your objects needing to take in dependencies that the object is not actually using, but needs the dependencies to pass into some other object. The bigger and deeper the object model gets, the worse this becomes. 
As a specific example, imagine you are inside of class `Foo` and wish to create an instance of `Bar`, but `Bar` has a dependency on `IService`. 
In many cases one of several options is used:
* Pass a facade to either the DI container or a factory that can create a `Bar`
* Add a dependency on `Foo` to take in an `IService` so that when it needs to create a `Bar` it can simply pass the instance in.

Though these options work, they add additional work. After all, in most cases when we want a `Bar` we simply call `new Bar();`. AutoDI lets you write exactly that, and moves the dependency resolution inside of Bar's constructor.
This dependency resolution is then forwarded to your favorite DI container to resolve it.

AutoDI.Container aims to take this one step further. Rather than needing to rely on a third party DI container, it will generate one at compile-time. In addition to being fast, it also saves you the haslte of needing to setup and maintain configuration code for your DI container.


## Does it work with <insert your favorite DI container / framework here>?
Probably. 
Take a look in the [Examples](https://github.com/Keboo/AutoDI/tree/master/Examples). There are examples of adding support for other DI containers as well as simple eamples for most frameworks.

*Don't see your favorite listed? I accept PRs.*

### Icon
![needle](https://raw.github.com/Keboo/AutoDI/master/Icons/needle.png)

[Needle](https://materialdesignicons.com/icon/needle) designed by [Austin Andrews](https://thenounproject.com/prosymbols/) from [Material Design Icons](https://materialdesignicons.com/)
