// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Loom
{
    /// <summary>
    /// This aspect weaver exception occurs when you are trying to create an instance of an abstract class, 
    /// which has no interwoven aspect methods to cover and implement all abstract methods.
    /// </summary>
    public class AbstractClassException : AspectWeaverException
    {
        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="classobject">Internal use.</param>
        public AbstractClassException(Runtime.ClassObject classobject)
            : base(1006, Resources.CodeBuilderErrors.ERR_1006, classobject.Type.FullName)
        {
        }
    }
}
