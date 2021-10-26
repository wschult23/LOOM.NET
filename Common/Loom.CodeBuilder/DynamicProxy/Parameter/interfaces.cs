// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Loom.CodeBuilder.DynamicProxy.Parameter
{
    internal class GenericDictionary : Dictionary<Type, Type> { }

    /// <summary>
    /// Wird verwendet um die generischen Typen einer Aspektmethode zu binden
    /// Wird von Klassen die von <see cref="ParameterCodeBrick"/> ableiten implementiert.
    /// </summary>
    internal interface IGenericParameterCodeBrick
    {
        /// Bindet die generischen Parameter des Ziels an die Typen des Aufrufers
        /// Ist der Generische Parameter als Key schon im Dictionary, passiert nichts
        /// (es ist durch das Matching schon sichergestellt, das in diesem Fall das Value
        /// bereits den richtigen Typen hat) ansonsten muss er hinzugefügt werden.
        void AddGenericParameter(GenericDictionary dictionary);
    }
}
