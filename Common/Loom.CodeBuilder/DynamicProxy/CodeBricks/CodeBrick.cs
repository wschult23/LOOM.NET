// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;

using Loom.JoinPoints;
using Loom.CodeBuilder.DynamicProxy;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    /// <summary>
    /// Ein Codebrick ist ein Stück Code, welcher in Methoden eingewoben wird
    /// Ein Codebrick kann dabei ein Objekt als Rückgabewert auf dem Stack hinterlassen
    /// Die unterliegende Logik entscheidet dann ob dieses Objekt direkt oder konvertiert
    /// weitergegeben oder ob es gar verworfen wird.
    /// Weiterhin kann ein Codebrick Requirements an seinen Code definieren.
    /// </summary>
    internal abstract class CodeBrick
    {
        /// <summary>
        /// Code erzeugen, der durch diesen Brick Repräsentiert wird
        /// </summary>
        /// <param name="wmeb"></param>
        /// <returns>Typ des Objektes, welches nach Ausführung des generierten Codes oben auf dem Stack liegt oder typeof(void)</returns>
        public abstract Type Emit(ProxyMemberBuilder wmeb);
    }
}
