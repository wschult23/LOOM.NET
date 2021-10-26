// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using Loom.Configuration;

namespace Loom.JoinPoints.Implementation
{
    /// <summary>
    /// Wird zum aufbauen der JoinPointstrukturen verwendet, um z.B. überladungen zu erkennen
    /// </summary>
    internal interface IManageJoinPoint
    {
        /// <summary>
        /// Liefert ein Methodbase-Objekt, über welches entschieden werden kann, ob
        /// ein Joinpoint auf einen anderen basiert (überladen ist) oder nicht. Bei Interfaces ist
        /// es die Methode des Interfaces, bei Virtuellen Methoden die Basisimplementierung,
        /// Bei Konstruktoren wird dies nicht benötigt
        /// </summary>
        MethodBase KeyMethod { get; }
    }

    /// <summary>
    /// Wird verwendet um einen existierenden JoinPoint an eine neue Implementation zu binden. Der JoinPoint
    /// wird kopiert und sieht für das Matching gleich aus, nur sind für den Weber in der Kopie zusätzliche
    /// Informationen abgespeichert
    /// </summary>
    internal interface IBindMethod
    {
        /// <summary>
        /// Erzeugt einen neuen JoinPoint, der an einen generierten Typen gebunden ist
        /// </summary>
        /// <param name="miimplementation">Die Implementation der Methode auf die der Joinpoint zeigt</param>
        /// <param name="mibasecallimplementation">ggf. eine Stellvertretermethode, die Aufgerufen wird um die Implementation zu erreichen</param>
        /// <returns></returns>
        MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation);
    }

    /// <summary>
    /// Siehe <see cref="IBindMethod"/>
    /// </summary>
    internal interface IBindConstructor
    {
        /// <summary>
        /// Erzeugt einen neuen JoinPoint, der an einen generierten Typen gebunden ist
        /// </summary>
        /// <param name="miimplementation">Die Implementation des Construktors auf die der Joinpoint zeigt</param>
        /// <param name="micreateai">Die Implementation der Advices, diese Methode wird gerufen um den Typen zu Erzeugen</param>
        /// <param name="micreateto">Die Implementation der Objekterzeugung, diese Ruft dann den Konstruktor</param>
        /// <returns></returns>
        ConstructorJoinPoint BindToType(ConstructorBuilder miimplementation, MethodBuilder micreateai, MethodBuilder micreateto);
    }

    /// <summary>
    /// Fasst die Joinpoints eines Interfaces zusammen
    /// </summary>
    internal struct InterfaceJoinPoints
    {
        /// <summary>
        /// Dieses Flag ist true, wenn <see cref="JoinPoints"/> einen JoinPoint mit einer virtuellen Methode enthält.
        /// Das bedeutet, dass der JoinPoint doppelt vorkommt (Noch einmal in VTableJoinPoints) es aber in der Zielklasse nur
        /// eine Methode gibt, die beide JP's abbildet. Daher muss beim erstmaligen Ableiten die Methode nach dem äblichen 
        /// Verfahren aufgespalten werden, das Interface also in jedem Fall implementiert werden, auch wenn es kein Match gibt
        /// </summary>
        public bool bContainsVirtualMethods;
        /// <summary>
        /// Das zugehärige Interface
        /// </summary>
        public InterfaceJoinPointCollection Interface;
        /// <summary>
        /// Die JoinPoints der Implementierung
        /// </summary>
        public MethodInfoJoinPoint[] JoinPoints;
    }

    /// <summary>
    /// Kollektion von Aspekten
    /// </summary>
    internal struct AssemblyAspectInfo
    {
        /// <summary>
        /// Cookie um herauszubekommen, ob der jp in einer session schon gesehen wurde. Wird von <see cref="JoinPointCollection"/> hochgezählt.
        /// </summary>
        public static int c_magiccookie = 0;
        /// <summary>
        /// Zeigt an, ob diese Aspekte während einer Sammlung schon angefasst wurden
        /// </summary>
        public int magiccookie;
        /// <summary>
        /// Die Aspekte, die direkt aus der Assembly kommen 
        /// </summary>
        public AspectCoverageInfo[] aspects;
        /// <summary>
        /// Liste der auf dieser Assembly Konfigurierten Typen
        /// </summary>
        public Dictionary<Type, TargetTypeConfig> typeconfiguration;
    }

    /// <summary>
    /// Verbindet einen JoinPoint mit einem Aspect
    /// </summary>
    internal struct JoinPointAspectInfo
    {
        public JoinPointAspectInfo(AspectCoverageInfo aspect, int ifcindex, int jpindex)
        {
            this.aspect = aspect;
            this.ifcindex = ifcindex;
            this.jpindex = jpindex;
        }
        /// <summary>
        /// Der Aspekt
        /// </summary>
        public AspectCoverageInfo aspect;
        /// <summary>
        /// Der Index in der InterfaceCollection oder -1
        /// </summary>
        public int ifcindex;
        /// <summary>
        /// Der Index in der JoinPointCollection
        /// </summary>
        public int jpindex;
    }

    internal class JoinPointAspectInfoList
    {
        public JoinPointAspectInfoList(List<JoinPointAspectInfo> list)
        {
            this.list = list;
            this.firstIndexOfVtblJoinPointAspectInfo = 0;
            this.firstIndexOfIfcJoinPointAspectInfo = 0;
        }

        /// <summary>
        /// Die Liste der JoinPointAspectInfos;
        /// </summary>
        public List<JoinPointAspectInfo> list;
        /// <summary>
        /// Der Index des letzten Ctor JopinPointAspectInfo
        /// </summary>
        public int firstIndexOfVtblJoinPointAspectInfo;
        /// <summary>
        /// Der Index der letzten VtblAspectInfo
        /// </summary>
        public int firstIndexOfIfcJoinPointAspectInfo;
    }

    /// <summary>
    /// Die JoinPoints eines Interfaces
    /// </summary>
    internal class InterfaceJoinPointCollection
    {
        /// <summary>
        /// s.u., wird von <see cref="IJoinPointEnumerator"/> und <see cref="JoinPointCollection.GetJoinPoints"/> hochgezählt
        /// </summary>
        public static volatile int c_magiccookie = 0;
        /// <summary>
        /// Wird verwendet um zu erkennen, ob dieses Interface bereits angefasst wurde
        /// </summary>
        public int magiccookie;
        /// <summary>
        /// Der Typ des Interfaces
        /// </summary>
        public Type Interface;
        /// <summary>
        /// Die Basisinterfaces
        /// </summary>
        public InterfaceJoinPointCollection[] BaseInterfaces;
        /// <summary>
        /// DIe JoinPoints des Interfaces
        /// </summary>
        public MethodInfoJoinPoint[] JoinPoints;
        /// <summary>
        /// Auf dem Interface verwobene Aspekte
        /// </summary>
        public AspectCoverageInfo[] aspects;
    }
}
