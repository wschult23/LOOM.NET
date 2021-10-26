// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom.AspectModel;

namespace Loom.JoinPoints.Implementation
{
    /// <summary>
    /// Eintrittspunkt zum parsen der JoinPoints
    /// </summary>
    interface IJoinPointEnumerator
    {
        /// <summary>
        /// Die Aspekte 
        /// </summary>
        ICollection<AspectCoverageInfo> Aspects { get; }
        /// <summary>
        /// zählt die Advices für den gegebenen Aspekttypen auf Muss als erstes aufgerufen werden
        /// </summary>
        /// <param name="acceptor"></param>
        /// <param name="jpcol">Die Collection, auf der enumeriert wird</param>
        void EnumJoinPoints(IAcceptJoinPoint acceptor, JoinPointCollection jpcol);
    }

    /// <summary>
    /// Rückrufinterface zum parsen einer JoinPointCollection
    /// </summary>
    /// <remarks>
    /// Aspecttype
    /// |-VTableJoinPoint
    /// |-CtorJoinPoint
    /// |-Interface
    ///   |-InterfaceJoinPoint
    /// </remarks>
    internal interface IAcceptJoinPoint
    {
        /// <summary>
        /// Eine virtuelle Methode, die verwoben werden soll
        /// </summary>
        /// <param name="aspectindex">Die Aspektinstanz, auf die sich der Aspekt bezieht</param>
        /// <param name="jp">Der JoinPoint</param>
        /// <param name="matches">Die Matches</param>
        /// <param name="jpindex"></param>
        void AcceptVTableJoinPoint(int aspectindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches);
        /// <summary>
        /// Ein KostruktorJoinPoint
        /// </summary>
        /// <param name="aspectindex">Die Aspektinstanz, auf die sich der Aspekt bezieht</param>
        /// <param name="jpindex"></param>
        /// <param name="jp">Der JoinPoint</param>
        /// <param name="matches">Die Matches</param>
        void AcceptCtorJoinPoint(int aspectindex, int jpindex, ConstructorJoinPoint jp, AspectMemberCollection matches);
        /// <summary>
        /// Ein InterfaceJoinPoint
        /// </summary>
        /// <param name="aspectindex">Die Aspektinstanz, auf die sich der Aspekt bezieht oder -1, wenn keine Aspektinstanz zugeordnet ist</param>
        /// <param name="ifcindex">Der Index des Interfaces in der entsprechenden Collection</param>
        /// <param name="jpindex"> Der Index des JoinPoints in der entsprechenden Collection</param>
        /// <param name="jp">Der JoinPoint</param>
        /// <param name="matches">Die Matches oder null, wenn kein Match</param>
        /// <remarks>
        /// Enumeriert alle verwobenen oder neu eingeführten Interface-Join-Points
        /// Kommen neue Interfaces hinzu, erfolgt erst ein Aufruf von <see cref="AcceptInterface"/>, der die Liste der Interfaces
        /// und damit die möglichen Indizies (ifcindex) entsprechend erweitert.
        /// </remarks>
        void AcceptInterfaceJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches);

        /// <summary>
        /// Ein InterfaceJoinPoint aus einer Introduction
        /// </summary>
        /// <param name="aspectindex">Die Aspektinstanz, auf die sich der Aspekt bezieht oder -1, wenn keine Aspektinstanz zugeordnet ist</param>
        /// <param name="ifcindex">Der Index des Interfaces in der entsprechenden Collection</param>
        /// <param name="jpindex"> Der Index des JoinPoints in der entsprechenden Collection</param>
        /// <param name="jp">Der JoinPoint</param>
        /// <param name="matches">Die Matches oder null, wenn kein Match</param>
        /// <remarks>
        /// Kommen neue Interfaces hinzu, erfolgt erst ein Aufruf von <see cref="AcceptInterface"/>, der die Liste der Interfaces
        /// erweitert.
        /// </remarks>
        void AcceptIntroductionJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches);

        /// <summary>
        /// Listet die neu hinzugekommenen Interfaces auf
        /// </summary>
        /// <param name="ifcol">Eine Liste neuer Interfaces</param>
        /// <remarks>
        /// Versetzt den Acceptor in einen neuen Zustand, es folgen nur noch Aufrufe zu <see cref="AcceptInterfaceJoinPoint"/>
        /// Der Interfaceindex bei diesen Aufrufen bezieht sich dann auf die übergebene Collection
        /// </remarks>
        void AcceptInterface(ICollection<InterfaceJoinPointCollection> ifcol);
    }
}
