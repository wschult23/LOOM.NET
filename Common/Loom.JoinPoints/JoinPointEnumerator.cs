// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Loom.AspectModel;

namespace Loom.JoinPoints.Implementation
{
    internal abstract class JoinPointCollectionEnumeratorBase
    {
        protected JoinPointCollectionEnumeratorBase(AspectClass ac)
        {
            this.ac = ac;
        }

        protected AspectClass ac;

        #region IJoinPointEnumerator Members

        public Type AspectType
        {
            get { return ac.AspectType; }
        }

        #endregion

        /// <summary>
        /// Enumeriert neu hinzugekommene Interfaces
        /// </summary>
        /// <param name="acceptor"></param>
        /// <param name="aspectindex">Index des Aspektes</param>
        protected void EnumIntroducedJoinPoints(IAcceptJoinPoint acceptor, int aspectindex)
        {
            // Introductions
            // Alle Interfaces suchen, die noch nicht in der Targetclass implementiert sind
            ICollection<Type> ifctypes = ac.GetIntroducedInterfaces();
            List<InterfaceJoinPointCollection> ifccol = new List<InterfaceJoinPointCollection>(ifctypes.Count);
            foreach (Type ifc in ifctypes)
            {
                InterfaceJoinPointCollection ifcjp = JoinPointCollection.GetInterfaceJoinPoints(ifc);
                if (ifcjp.magiccookie != InterfaceJoinPointCollection.c_magiccookie)
                {
                    ifccol.Add(ifcjp);
                }
            }
            // Die neuen Interfaces "anmelden" und dann die Joinpoints durchgehen
            acceptor.AcceptInterface(ifccol);
            for (int ifcindex = 0; ifcindex < ifccol.Count; ifcindex++)
            {
                InterfaceJoinPointCollection ifjpcol = ifccol[ifcindex];
                for (int jpindex = 0; jpindex < ifjpcol.JoinPoints.Length; jpindex++)
                {
                    MethodInfoJoinPoint jp = ifjpcol.JoinPoints[jpindex];
                    AspectMemberCollection matches = ac.GetIntroduction(jp);
                    acceptor.AcceptIntroductionJoinPoint(aspectindex, ifcindex, jpindex, jp, matches);
                }
            }
        }

        protected void EnumCtorJoinPoints(JoinPointCollection jpcol, IAcceptJoinPoint acceptor)
        {
            // Konstruktoren
            for (int jpindex = 0; jpindex < jpcol.CtorJoinPoints.Length; jpindex++)
            {
                ConstructorJoinPoint jp = jpcol.CtorJoinPoints[jpindex];
                AspectMemberCollection matches = ac.GetAdvices(jp);
                acceptor.AcceptCtorJoinPoint(0, jpindex, jp, matches);
            }
        }

        /// <summary>
        /// EnumJoinPoints welches von <see cref="JoinPointCollectionEnumerator"/> und <see cref="DynamicJoinPointCollectionEnumerator"/> verwendet wird
        /// </summary>
        /// <param name="acceptor"></param>
        /// <param name="jpcol"></param>
        public void EnumJoinPoints(IAcceptJoinPoint acceptor, JoinPointCollection jpcol)
        {
            // Cookie für diesen Aufruf setzen
            InterfaceJoinPointCollection.c_magiccookie++;
            int jpindex, ifcindex;
            // Alle Joinpoints durchgehen und bei einem Match die entsprechenden Acceptoren aufrufen
            // VTable
            for (jpindex = 0; jpindex < jpcol.VTableJoinPoints.Length; jpindex++)
            {
                MethodInfoJoinPoint jp = jpcol.VTableJoinPoints[jpindex];
                AspectMemberCollection matches = ac.GetAdvices(jp);
                if (matches != null)
                {
                    acceptor.AcceptVTableJoinPoint(0, jpindex, jp, matches);
                }
            }

            // Interfaces
            for (ifcindex = 0; ifcindex < jpcol.InterfaceJoinPoints.Length; ifcindex++)
            {
                InterfaceJoinPoints ifcjp = jpcol.InterfaceJoinPoints[ifcindex];
                // Interface als "besucht" markieren
                ifcjp.Interface.magiccookie = InterfaceJoinPointCollection.c_magiccookie;
                // Wenn keine virtuellen Methoden das Interface implementieren, muss nur im Falle eines Matches eine
                // Verwebung vorgenommen werden
                jpindex = 0;
                if (!ifcjp.bContainsVirtualMethods)
                {
                    for (; jpindex < ifcjp.JoinPoints.Length; jpindex++)
                    {
                        MethodInfoJoinPoint jp = ifcjp.JoinPoints[jpindex];
                        AspectMemberCollection matches = ac.GetAdvices(jp);
                        if (matches != null)
                        {
                            // Alle davor verweben
                            for (int jpindex2 = 0; jpindex2 < jpindex; jpindex2++)
                            {
                                acceptor.AcceptInterfaceJoinPoint(0, ifcindex, jpindex2, ifcjp.JoinPoints[jpindex2], null);
                            }
                            acceptor.AcceptInterfaceJoinPoint(0, ifcindex, jpindex, jp, matches);
                            jpindex++;
                            break;
                        }
                    }
                }
                for (; jpindex < ifcjp.JoinPoints.Length; jpindex++)
                {
                    MethodInfoJoinPoint jp = ifcjp.JoinPoints[jpindex];
                    AspectMemberCollection matches = ac.GetAdvices(jp);
                    acceptor.AcceptInterfaceJoinPoint(0, ifcindex, jpindex, jp, matches);
                }
            }

            // Konstruktoren und Introductions
            EnumCtorJoinPoints(jpcol, acceptor);
            EnumIntroducedJoinPoints(acceptor, 0);
        }

    }

    /// <summary>
    /// Implementiert Teile des <see cref="IJoinPointEnumerator"/> Interfaces für Enumeratoren, die ausschlieslich eine <see cref="AspectCoverageInfo"/> Instanz haben
    /// </summary>
    internal class SingleAspectJoinPointCollection : JoinPointCollectionEnumeratorBase, ICollection<AspectCoverageInfo>
    {
        protected AspectCoverageInfo aspect;

        public SingleAspectJoinPointCollection(AspectCoverageInfo aspect) :
            base(aspect.AspectClass)
        {
            this.aspect = aspect;
        }

        #region IJoinPointEnumerator Members

        public ICollection<AspectCoverageInfo> Aspects
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region ICollection<AspectInfo> Members

        public void Add(AspectCoverageInfo item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(AspectCoverageInfo item)
        {
            return (aspect == item);
        }

        public void CopyTo(AspectCoverageInfo[] array, int arrayIndex)
        {
            array[arrayIndex] = aspect;
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(AspectCoverageInfo item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<AspectInfo> Members

        public IEnumerator<AspectCoverageInfo> GetEnumerator()
        {
            yield return aspect;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            yield return aspect;
        }

        #endregion


    }

    /// <summary>
    /// Enumeriert die JoinPoints einer Klasse
    /// </summary>
    internal class JoinPointCollectionEnumerator : SingleAspectJoinPointCollection, IJoinPointEnumerator
    {
        public JoinPointCollectionEnumerator(AspectCoverageInfo aspect) :
            base(aspect)
        {
        }
    }

    /// <summary>
    /// Enumeriert Joinpoints einer Klasse für einen dynamischen ASpekt
    /// </summary>
    internal class DynamicJoinPointCollectionEnumerator : JoinPointCollectionEnumeratorBase, IJoinPointEnumerator
    {
        public DynamicJoinPointCollectionEnumerator(AspectClass ac)
            :
            base(ac)
        {
        }


        #region IJoinPointEnumerator Members

        ICollection<AspectCoverageInfo> IJoinPointEnumerator.Aspects
        {
            get { return null; }
        }


        #endregion
    }

    /// <summary>
    /// zählt die Joinpoints für ein Aspekt auf einem Interface auf
    /// </summary>
    internal class JoinPointInterfaceCollectionEnumerator : SingleAspectJoinPointCollection, IJoinPointEnumerator
    {
        private InterfaceJoinPoints ifcjp;
        private int ifcindex;

        public JoinPointInterfaceCollectionEnumerator(AspectCoverageInfo aspect, InterfaceJoinPoints ifcjp, int ifcindex) :
            base(aspect)
        {
            this.ifcjp = ifcjp;
            this.ifcindex = ifcindex;
        }

        #region IJoinPointEnumerator Members

        public new void EnumJoinPoints(IAcceptJoinPoint acceptor, JoinPointCollection jpcol)
        {
            // Cookie für diesen Aufruf setzen
            InterfaceJoinPointCollection.c_magiccookie++;
            EnumJoinPoints(ifcjp, ifcindex, acceptor, jpcol);
            base.EnumCtorJoinPoints(jpcol, acceptor);
            base.EnumIntroducedJoinPoints(acceptor, jpcol.InterfaceJoinPoints.Length);
        }

        #endregion

        /// <summary>
        /// zählt die JoinPoints für das Interface und alle Basisinterfaces auf
        /// </summary>
        /// <param name="curifcjp"></param>
        /// <param name="curifcindex"></param>
        /// <param name="acceptor"></param>
        /// <param name="jpcol"></param>
        private void EnumJoinPoints(InterfaceJoinPoints curifcjp, int curifcindex, IAcceptJoinPoint acceptor, JoinPointCollection jpcol)
        {
            // Alle Basisinterfaces durchgehen
            foreach (InterfaceJoinPointCollection baseifcjpcol in curifcjp.Interface.BaseInterfaces)
            {
                // Implementierung in der JoinPoinCollection suchen
                int baseifcindex = jpcol.InterfaceJoinPoints.Length;
                // Die Basisinterfaces kommen in der Liste meistens hinten, also räckwärts zählen
                while (0 != baseifcindex--)
                {
                    InterfaceJoinPoints baseifcjp = jpcol.InterfaceJoinPoints[baseifcindex];
                    if (baseifcjp.Interface == baseifcjpcol)
                    {
                        EnumJoinPoints(baseifcjp, baseifcindex, acceptor, jpcol);
                        break;
                    }
                }
                System.Diagnostics.Debug.Assert(baseifcindex >= 0); // Implementierung nicht gefunden
            }
            // Das aktuelle Interface behandeln
            curifcjp.Interface.magiccookie = InterfaceJoinPointCollection.c_magiccookie;
            for (int jpindex = 0; jpindex < curifcjp.JoinPoints.Length; jpindex++)
            {
                MethodInfoJoinPoint jp = curifcjp.JoinPoints[jpindex];
                AspectMemberCollection matches = ac.GetAdvices(jp);
                acceptor.AcceptInterfaceJoinPoint(0, curifcindex, jpindex, jp, matches);
            }
        }
    }


    /// <summary>
    /// Wird verwendet um JoinPoints aufzuzählen, die direkt mit einem Aspekt annotiert sind
    /// </summary>
    internal class JoinPointMethodEnumerator : JoinPointCollectionEnumeratorBase, IJoinPointEnumerator, ICollection<AspectCoverageInfo>
    {
        /// <summary>
        /// Eine Liste die alle JoinPoints und die zugehörigen Aspektinmstanzen der Aspektklasse enthält
        /// </summary>
        JoinPointAspectInfoList jplist;
        /// <summary>
        /// Die Aspektklasse
        /// </summary>

        public JoinPointMethodEnumerator(AspectClass ac, JoinPointAspectInfoList jplist)
            : base(ac)
        {
            this.jplist = jplist;
        }

        #region IJoinPointEnumerator Members

        /// <summary>
        /// Enumeriert die Joinpoints in der liste
        /// </summary>
        /// <param name="acceptor"></param>
        /// <param name="jpcol"></param>
        public new void EnumJoinPoints(IAcceptJoinPoint acceptor, JoinPointCollection jpcol)
        {
            // Cookie für diesen Aufruf setzen
            InterfaceJoinPointCollection.c_magiccookie++;

            int jpindex;
            // Wir gehen vorwärts durch die Liste, sie ist sortiert nach Konstruktoren, wie in CtorJoinPoints,
            // virtuellen Methoden wie in VTBLJoinPoints und dann in Interfaces
            int listindex = 0;
            // Konstruktoren müssen in jedem Fall alle verwoben werden
            for (jpindex = 0; jpindex < jpcol.CtorJoinPoints.Length; jpindex++)
            {
                // Aspektindex erstmal auf -1 setzen, d.h. es kann noch keine Aspektinstanz zugewiesen werden
                int aspectindex = -1;
                AspectMemberCollection matches = null;
                ConstructorJoinPoint jp = jpcol.CtorJoinPoints[jpindex];

                // ist der Konstruktor in der Liste? Dann die Matches holen
                if (listindex < jplist.firstIndexOfVtblJoinPointAspectInfo && jpindex == jplist.list[listindex].jpindex)
                {
                    matches = ac.GetAdvices(jp);
                    // Der Aspektindex entspricht dem Index in der Liste
                    aspectindex = listindex;
                    listindex++;
                }

                acceptor.AcceptCtorJoinPoint(aspectindex, jpindex, jp, matches);
            }

            // Es folgen die VTBLJoinPoints
            if (jplist.firstIndexOfIfcJoinPointAspectInfo > 0)
            {
                for (; listindex < jplist.list.Count && jplist.firstIndexOfIfcJoinPointAspectInfo != listindex; listindex++)
                {
                    JoinPointAspectInfo jpai = jplist.list[listindex];

                    MethodInfoJoinPoint jp = jpcol.VTableJoinPoints[jpai.jpindex];
                    AspectMemberCollection matches = ac.GetAdvices(jp);
                    acceptor.AcceptVTableJoinPoint(listindex, jpai.jpindex, jp, matches);
                }
            }

            // nun die verbleibenden InterfaceJoinPoints
            for (int ifcindex = 0; ifcindex < jpcol.InterfaceJoinPoints.Length; ifcindex++)
            {
                InterfaceJoinPoints ifjp = jpcol.InterfaceJoinPoints[ifcindex];
                // Das Interface markieren
                ifjp.Interface.magiccookie = InterfaceJoinPointCollection.c_magiccookie;

                // Wenn das nächste Element in der Liste zum Interface gehört oder das Interface mit
                // virtuellen Methoden implementiert wurde, muss es komplett neu implementiert werden
                if (ifjp.bContainsVirtualMethods || (listindex < jplist.list.Count && jplist.list[listindex].ifcindex == ifcindex))
                {
                    for (jpindex = 0; jpindex < ifjp.JoinPoints.Length; jpindex++)
                    {
                        // Aspektindex erstmal auf -1 setzen, d.h. es kann noch keine Aspektinstanz zugewiesen werden
                        int aspectindex = -1;
                        AspectMemberCollection matches = null;
                        MethodInfoJoinPoint jp = ifjp.JoinPoints[jpindex];

                        // Ist die Methode in der Liste?
                        if (listindex < jplist.list.Count && jpindex == jplist.list[listindex].jpindex)
                        {
                            matches = ac.GetAdvices(jp);
                            // Der Aspektindex entspricht dem Index in der Liste-2 (durch die Kontrollelemente)
                            aspectindex = listindex;
                            listindex++;
                        }

                        acceptor.AcceptInterfaceJoinPoint(aspectindex, ifcindex, jpindex, jp, matches);
                    }
                }
                else
                {
                    // Hier mögliche Introductions checken, die bestehende Interfaces verweben
                    for (jpindex = 0; jpindex < ifjp.JoinPoints.Length; jpindex++)
                    {
                        AspectMemberCollection matches = null;
                        MethodInfoJoinPoint jp = ifjp.JoinPoints[jpindex];

                        matches = ac.GetAdvices(jp);
                        acceptor.AcceptInterfaceJoinPoint(-1, ifcindex, jpindex, jp, matches);
                    }
                }
               
            }

            System.Diagnostics.Debug.Assert(listindex == jplist.list.Count);

            // Jetzt noch die Introductions
            base.EnumIntroducedJoinPoints(acceptor, -1);
        }

        public ICollection<AspectCoverageInfo> Aspects
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region ICollection<AspectInfo> Members

        public void Add(AspectCoverageInfo item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(AspectCoverageInfo item)
        {
            foreach (var jpai in jplist.list)
            {
                if (jpai.aspect == item) return true;
            }
            return false;
        }

        public void CopyTo(AspectCoverageInfo[] array, int arrayIndex)
        {
            for (int iPos = 0; iPos < jplist.list.Count; iPos++)
            {
                array[arrayIndex + iPos] = jplist.list[iPos].aspect;
            }

        }

        public int Count
        {
            get { return jplist.list.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(AspectCoverageInfo item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<AspectInfo> Members

        public IEnumerator<AspectCoverageInfo> GetEnumerator()
        {
            return jplist.list.Select(ai => ai.aspect).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
