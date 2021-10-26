// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Reflection;
using System.Text;

using Loom.CodeBuilder;

namespace Loom.AspectModel
{
    /// <summary>
    /// über dieses Interface wird gepräft, ob ein Parameter einer Aspektmethode zur Zielmethode passt. Die konkreten
    /// Implementierungen sind abhängig von der Art der Parameter.
    /// </summary>
    internal interface IParameterMatch
    {
        /// <summary>
        /// Die Methode bekommt einen Enumerator auf die Zielmethodenparameter und präft, ob der Aspektparameter
        /// auf einen oder mehrere Parameter passt. Werden mehrere Parameter vom Aspektparameter abgedekt, muss der Enumerator auf
        /// den letzten gematchten Parameter weitergeschoben werden. Werden alle restlichen Parameter gematcht, kann der Enumerator 
        /// anstelle des letzten auch hinter das letzte Element geschoben werden.
        /// Beim Aufruf der Methode ist IEnumerator.Current auf jeden Fall valid, ein Zugriff wird niemals eine Exception erzeugen.
        /// Die Typkompatibilität muss an dieser stelle gepräft werden. Im Fehlerfall ist eine <see cref="AspectWeaverException"/>
        /// geworfen werden.
        /// Nach dem Aufruf von IsMatch erfolgt bei Rückgabe von true die Anforderung des passenden CodeBrick's.
        /// </summary>
        /// <param name="epi">Enumeratorobjekt auf den/die nächsten zu präfenden Parameter oder null bei leerer Parameterliste</param>
        /// 
        /// <returns>true, wenn der Parameter der Aspektmethode eine verwebung gestattet, false sonst</returns>
        bool IsMatch(IEnumerator epi);

        /// <summary>
        /// Liefert für einen Parameter-Match den passenden Code Brick
        /// </summary>
        /// <param name="ppf">die Factory</param>
        /// <returns></returns>
        void AddParameter(CodeBrickBuilder ppf);
    }

    /// <summary>
    /// is used by ConnectionPointSignatureAttribute attributes to create a parameter match object
    /// </summary>
    internal interface ICreateParameterMatch
    {
        IParameterMatch Create(ParameterInfo pi);
    }

    /// <summary>
    /// Provides functions to compare aspect method return types with target method parameters.
    /// </summary>
    internal interface IResultTypeMatch
    {
        /// <summary>
        /// Parameter matches
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsMatch(Type type);

        /// <summary>
        /// The result type
        /// </summary>
        Type ResultType { get; }
    }

    /// <summary>
    /// is used by ConnectionPointSignatureAttribute attributes to create a result type match object
    /// </summary>
    internal interface ICreateResultTypeMatch
    {
        IResultTypeMatch Create(MethodInfo type);
    }

    /// <summary>
    /// Wird von Matchobjekten implementiert, wenn sie einen Generischen Parameter repräsentieren
    /// </summary>
    interface IGenericParameter
    {
        /// <summary>
        /// Der Generische Typ
        /// </summary>
        /// <returns></returns>
        Type GenericParameterType { get; }
    }
}
