using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Quartz.Plugins.Attributes
{
    internal class QuartzJobTypeContainer : IList<Type>
    {
        private readonly List<Type> _types;

        public Type this[int index]
        {
            get => _types[index];
            set => _types[index] = value;
        }

        public int Count
        {
            get => _types.Count;
        }

        public bool IsReadOnly
        {
            get => ((IList<Type>)_types).IsReadOnly;
        }

        public QuartzJobTypeContainer()
        {
            _types = new List<Type>();
        }

        public QuartzJobTypeContainer(IEnumerable<Type> collection) 
        {
            if (collection.Any(e => !ReflectionHelper.IsQuartzJobClass(e)))
                throw new ArgumentException(nameof(collection));

            _types = new List<Type>(collection);
        }

        public void Add(Type item)
        {
            if (!ReflectionHelper.IsQuartzJobClass(item))
                throw new ArgumentException(nameof(item));

            _types.Add(item);
        }

        public void Insert(int index, Type item)
        {
            _types.Insert(index, item);
        }

        public bool Contains(Type item)
        {
            return _types.Contains(item);
        }

        public int IndexOf(Type item)
        {
            return _types.IndexOf(item);
        }

        public bool Remove(Type item)
        {
            return _types.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _types.RemoveAt(index);
        }

        public void Clear()
        {
            _types.Clear();
        }

        public void CopyTo(Type[] array, int arrayIndex)
        {
            _types.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Type>)_types).GetEnumerator();
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return ((IList<Type>)_types).GetEnumerator();
        }
    }
}
