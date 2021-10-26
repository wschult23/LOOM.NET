// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Loom.AspectModel
{
    /// <summary>
    /// Eine Collection von AspectMember
    /// </summary>
    internal abstract class AspectMemberCollection : IList<AspectMember>
    {

        #region IList<AspectMember> Members

        public abstract int IndexOf(AspectMember item);

        public void Insert(int index, AspectMember item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public virtual AspectMember this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<AspectMember> Members

        public abstract void Add(AspectMember item);

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(AspectMember item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(AspectMember[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public abstract int Count
        {
            get;
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(AspectMember item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<AspectMember> Members

        public abstract IEnumerator<AspectMember> GetEnumerator();

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (object obj in this)
            {
                sb.Append("\n");
                sb.Append(obj.ToString());
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Eine Collection von AspectMember mit mehr als einem Eintrag
    /// </summary>
    internal class MultiItemAspectMemberCollection : AspectMemberCollection
    {
        List<AspectMember> aspectmember;
        static IEnumerable<AspectMember> c_empty = new AspectMember[0];

        public AspectMember[] ToArray()
        {
            if (aspectmember == null) return new AspectMember[0];
            return aspectmember.ToArray();
        }

        public void AddRange(MultiItemAspectMemberCollection am)
        {
            if (am == null) return;
            if (aspectmember == null)
            {
                aspectmember = am.aspectmember;
            }
            else
            {
                aspectmember.AddRange(am);
            }
        }

        public override int IndexOf(AspectMember item)
        {
            if (aspectmember == null) return -1;
            return aspectmember.IndexOf(item);
        }


        public override AspectMember this[int index]
        {
            get
            {
                if (aspectmember == null) throw new ArgumentOutOfRangeException();
                return aspectmember[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Add(AspectMember item)
        {
            if (aspectmember == null) aspectmember = new List<AspectMember>();
            aspectmember.Add(item);
        }

        public override IEnumerator<AspectMember> GetEnumerator()
        {
            if (aspectmember == null)
            {
                return c_empty.GetEnumerator();
            }
            return aspectmember.GetEnumerator();
        }

        public override int Count
        {
            get
            {
                if (aspectmember == null) return 0;
                return aspectmember.Count;
            }
        }
    }

    /// <summary>
    /// Eine Collection von Aspektmember mit maximal einem Eintrag
    /// </summary>
    internal class SingleItemAspectMemberCollection : AspectMemberCollection
    {
        public class InvalidAddOperationException : InvalidOperationException
        {
            AspectMember item;

            internal AspectMember Item
            {
                get { return item; }
            }

            public InvalidAddOperationException(AspectMember item)
                :
                base()
            {
                this.item = item;
            }
        }

        AspectMember item;

        public AspectMember Item
        {
            get
            {
                return item;
            }
        }

        public override int IndexOf(AspectMember item)
        {
            return (this.item == item) ? 0 : -1;
        }

        public override void Add(AspectMember item)
        {
            if (this.item != null) throw new InvalidAddOperationException(item);
            this.item = item;
        }

        public override int Count
        {
            get { return item == null ? 0 : 1; }
        }

        public override IEnumerator<AspectMember> GetEnumerator()
        {
            if (item != null) yield return item;
        }
    }

}
