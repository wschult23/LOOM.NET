// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom;
using Loom.JoinPoints;

namespace VariablesExample
{
    public class Aspect1 : AspectAttribute
    {
        /// <summary>
        /// This aspect method declares two jp variables using the JPVariable attribute. Using the ref keyword, it's possible 
        /// to modify the variable.
        /// </summary>
        /// <param name="hidden">(private) this jp variable is accessible only by members of this aspect class</param>
        /// <param name="visible">(protected) this jp variable is accessible by all interweaved aspects and target classes</param>
        [Call(Advice.Before)]
        public void Test([JPVariable(Scope.Private)] ref int hidden, [JPVariable(Scope.Protected)] ref int visible)
        {
            Console.WriteLine("Aspect1: hidden={0}, visible={1}", hidden, visible);  // both 0
            hidden++; visible++; // both 1 (but hidden is private to this aspect class!!!)
        }
    }

    public class Aspect2 : AspectAttribute
    {
        /// <summary>
        /// This aspect method declares two jp variables using the JPVariable attribute. Using the ref keyword, it's possible 
        /// to modify the variable.
        /// </summary>
        /// <param name="hidden">(private) this jp variable is accessible only by members of this aspect class</param>
        /// <param name="visible">(protected) this jp variable is accessible by all interweaved aspects and target classes</param>
        [Call(Advice.Before)]
        public void Test([JPVariable(Scope.Private)] ref int hidden, [JPVariable(Scope.Protected)] ref int visible)
        {
            Console.WriteLine("Aspect2: hidden={0}, visible={1}", hidden, visible); // hidden=0, visible is already 1, set by aspect1
            hidden++; visible++; // hidden=1, visible=2
        }
    }
}
