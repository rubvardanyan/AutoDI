﻿using System;
using System.Linq;
using System.Reflection;

namespace AutoDI.AssemblyGenerator
{
    public static class AssemblyMixins
    {
        public static object InvokeStatic<T>(this Assembly assembly, string methodName, params object[] parameters) where T : class
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            Type type = assembly.GetType(typeof(T).FullName);
            if (type == null)
                throw new AssemblyInvocationExcetion($"Could not find '{typeof(T).FullName}' in '{assembly.FullName}'");

            MethodInfo method = type.GetMethod(methodName);
            if (method == null)
                throw new AssemblyInvocationExcetion($"Could not find method '{methodName}' on type '{type.FullName}'");

            if (!method.IsStatic)
                throw new AssemblyInvocationExcetion($"Method '{type.FullName}.{methodName}' is not static");

            return method.Invoke(null, parameters);
        }

        public static object InvokeGeneric<TGeneric>(this Assembly assembly, object target, string methodName, params object[] parameters)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            Type genericType = assembly.GetType(typeof(TGeneric).FullName);
            if (genericType == null)
                throw new AssemblyInvocationExcetion($"Could not find generic parameter type '{typeof(TGeneric).FullName}' in '{assembly.FullName}'");

            Type targetType = target.GetType();

            MethodInfo method = targetType.GetMethod(methodName);
            if (method == null)
            {
                foreach (var @interface in targetType.GetInterfaces())
                {
                    method = @interface.GetMethod(methodName);
                    if (method != null) break;
                }
            }
            if (method == null)
                throw new AssemblyInvocationExcetion($"Could not find method '{methodName}' on type '{targetType.FullName}'");

            if (method.IsStatic)
                throw new AssemblyInvocationExcetion($"Method '{genericType.FullName}.{methodName}' is static");

            MethodInfo genericMethod = method.MakeGenericMethod(genericType);

            return genericMethod.Invoke(target, parameters);
        }

        public static object CreateInstance<T>(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type type = assembly.GetType(typeof(T).FullName);
            if (type == null)
                throw new AssemblyCreateInstanceException($"Could not find '{typeof(T).FullName}' in '{assembly.FullName}'");

            foreach (ConstructorInfo ctor in type.GetConstructors().OrderBy(c => c.GetParameters().Length))
            {
                var parameters = ctor.GetParameters();
                if (parameters.All(pi => pi.HasDefaultValue))
                    return ctor.Invoke(parameters.Select(x => x.DefaultValue).ToArray());
            }
            throw new AssemblyCreateInstanceException($"Could not find valid constructor for '{typeof(T).FullName}'");
        }

        public static object Resolve<T>(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type resolverType = assembly.GetType("AutoDI.AutoDIContainer");
            if (resolverType == null)
                throw new InvalidOperationException("Could not find AutoDIContainer");

            var resolver = Activator.CreateInstance(resolverType) as IDependencyResolver;

            if (resolver == null)
                throw new InvalidOperationException($"Failed to create resolver '{resolverType.FullName}'");


            return assembly.InvokeGeneric<T>(resolver, nameof(IDependencyResolver.Resolve), (object) new object[0]);
        }
    }
}