using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Test.Mocks;

public sealed class MockServiceCollection : IServiceCollection
{
    public Action<ServiceDescriptor> _addAction;
    public Func<int> _countFunc;
    public Func<IEnumerator<ServiceDescriptor>> _getEnumeratorFunc;
    public Func<int, ServiceDescriptor> _itemFunc;

    int ICollection<ServiceDescriptor>.Count => this._countFunc();

    bool ICollection<ServiceDescriptor>.IsReadOnly =>
        throw new NotImplementedException();

    ServiceDescriptor IList<ServiceDescriptor>.this[int index]
    {
        get => this._itemFunc(index);
        set => throw new NotImplementedException();
    }

    public MockServiceCollection()
    {
        this._addAction = _ => throw new NotImplementedException();
        this._countFunc = () => throw new NotImplementedException();
        this._getEnumeratorFunc = () => throw new NotImplementedException();
        this._itemFunc = _ => throw new NotImplementedException();
    }

    void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item) =>
        this._addAction(item);

    void ICollection<ServiceDescriptor>.Clear() =>
        throw new NotImplementedException();

    bool ICollection<ServiceDescriptor>.Contains(ServiceDescriptor item) =>
        throw new NotImplementedException();

    void ICollection<ServiceDescriptor>.CopyTo(
        ServiceDescriptor[] array,
        int arrayIndex
    ) =>
        throw new NotImplementedException();

    bool ICollection<ServiceDescriptor>.Remove(ServiceDescriptor item) =>
        throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() =>
        throw new NotImplementedException();

    IEnumerator<ServiceDescriptor> IEnumerable<ServiceDescriptor>.GetEnumerator() =>
        this._getEnumeratorFunc();

    int IList<ServiceDescriptor>.IndexOf(ServiceDescriptor item) =>
        throw new NotImplementedException();

    void IList<ServiceDescriptor>.Insert(int index, ServiceDescriptor item) =>
        throw new NotImplementedException();

    void IList<ServiceDescriptor>.RemoveAt(int index) =>
        throw new NotImplementedException();
}
