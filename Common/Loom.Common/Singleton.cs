﻿// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loom.Common
{
    internal class Singleton<T> where T : new()
    {
        private static T c_inst = new T();

        public static T GetObject()
        {
            return c_inst;
        }
    }
}
