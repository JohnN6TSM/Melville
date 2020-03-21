﻿using System;
using System.Collections.Generic;
using System.Linq;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies;

namespace Melville.IOC.TypeResolutionPolicy
{
    public interface ITypeResolutionPolicy
    {
        IActivationStrategy? ApplyResolutionPolicy(IBindingRequest request);
    }

    public interface ITypeResolutionPolicyList:ITypeResolutionPolicy
    {
        T GetPolicy<T>();
    }
    public class TypeResolutionPolicyList: ITypeResolutionPolicyList
    {
        public List<ITypeResolutionPolicy> Policies { get; } = new List<ITypeResolutionPolicy>();
        public IActivationStrategy? ApplyResolutionPolicy(IBindingRequest request) =>
            ObjectFactory.ForceToObjectFactory(Policies
                .Select(i => i.ApplyResolutionPolicy(request))
                .FirstOrDefault(i => i != null && i.ValidForRequest(request)));


        public T GetPolicy<T>() => Policies
                                       .Select(i=>i is MemorizeResult mr? mr.InnerPolicy:i)
                                       .OfType<T>()
                                       .FirstOrDefault()??
                                   throw new InvalidOperationException("No policy object of type: " + typeof(T).Name);
    }
}