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

namespace Loom.CodeBuilder.Gripper
{
    public class SymbolDictionary : MarshalByRefObject
    {
        public class ClassDictionary : Dictionary<string, bool> { }

        private Dictionary<string, ClassDictionary> dictClasses;
        private Dictionary<string, object> dictModules;

        public SymbolDictionary()
        {
            this.dictClasses = new Dictionary<string, ClassDictionary>();
            this.dictModules = new Dictionary<string, object>();
        }

        public bool NeedInstrumentation(string assemblysymbol, string classsymbol)
        {
            ClassDictionary cld;
            if (!dictClasses.TryGetValue(assemblysymbol, out cld))
            {
                return false;
            }
            return cld.ContainsKey(classsymbol) ? cld[classsymbol] : false;
        }


        public void AddSymbol(string assemblysymbol, string classsymbol, bool val)
        {
            ClassDictionary cld;
            if (!dictClasses.TryGetValue(assemblysymbol, out cld))
            {
                cld = new ClassDictionary();
                dictClasses.Add(assemblysymbol, cld);
            }
            cld.Add(classsymbol, val);
        }

        public void AddModule(string assemblysymbol, object module)
        {
            dictModules.Add(assemblysymbol, module);
        }

        public object GetWeavingModule(string assemblysymbol)
        {
            object retval;
            if(dictModules.TryGetValue(assemblysymbol, out retval))
            {
                return retval;
            }
            return null;
        }
    }
}
