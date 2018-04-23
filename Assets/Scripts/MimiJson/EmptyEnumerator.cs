using System;
using System.Collections;

namespace MimiJson
{
    internal class EmptyEnumerator : IEnumerator
    {
        public EmptyEnumerator() { }

        public void Reset() { }

        public object Current { get { throw new InvalidOperationException(); } }

        public bool MoveNext() { return false; }
    }
}
