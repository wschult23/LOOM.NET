// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Text;

using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.Common;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{

    /// <summary>
    /// CodeBrick zum Aufruf einer Aspektmethode.
    /// </summary>
    internal class AspectCallCodeBrick : CodeBrick
    {
        protected MethodInfo aspectmethod;
        protected ParameterCodeBrick[] param;
        protected DataSlot aspect;

        public MethodInfo AspectMethod
        {
            get { return aspectmethod; }
        }

        /// <summary>
        /// ASpektdataslot wird über den wmeb ermittelt
        /// </summary>
        /// <param name="param"></param>
        /// <param name="aspectmethod"></param>
        public AspectCallCodeBrick(ParameterCodeBrick[] param, MethodInfo aspectmethod)
        {
            this.param = param;
            this.aspectmethod = aspectmethod;
        }

        /// <summary>
        /// Aspektdataslot wird explizit angegeben, da er sich nicht über den wmeb ermitteln lässt
        /// </summary>
        /// <param name="param"></param>
        /// <param name="aspectmethod"></param>
        /// <param name="aspect"></param>
        public AspectCallCodeBrick(ParameterCodeBrick[] param, MethodInfo aspectmethod, DataSlot aspect)
        {
            this.param = param;
            this.aspectmethod = aspectmethod;
            this.aspect = aspect;
        }

        /// <summary>
        /// Erzeugt Code zum Aufruf einer Aspektmethode.
        /// Die Methode wird von ProxyIntroductionMethodBuilder.Emit() aufgerufen.
        /// </summary>
        /// <param name="wmeb"></param>
        /// <returns></returns>
        public override Type Emit(ProxyMemberBuilder wmeb)
        {
            IILGenerator iilgen = ((IILGenerator)wmeb);
            ILGenerator ilgen = iilgen.ILGenerator;
            if (!aspectmethod.IsStatic)
            {
                if (aspect != null)
                {
                    aspect.EmitLoadValue(wmeb, aspectmethod.DeclaringType);
                }
                else
                {
                    if (wmeb.DataSlots.Aspect == null)
                    {
                        throw new AspectWeaverException(1013, Resources.CodeBuilderErrors.ERR_1013, Common.Convert.ToString(aspectmethod));
                    }
                    // Aspektfeld laden in aktuellem Objekt
                    wmeb.DataSlots.Aspect.EmitLoadValue(wmeb, aspectmethod.DeclaringType);
                }
            }
            // Parameter auf den Stack
            int iPos = 0;
            for (; iPos < param.Length; iPos++)
            {
                param[iPos].EmitPrepare(wmeb);
            }
            // Methode rufen
            ilgen.EmitCall(OpCodes.Call, aspectmethod, null);
            // Parameter zurückschreiben
            for (iPos--; iPos >= 0; iPos--)
            {
                param[iPos].EmitPostprocessing(wmeb);
            }
            return aspectmethod.ReturnType;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name);
            sb.Append(" \"call ");
            sb.Append(Common.Convert.ToString(aspectmethod));
            sb.Append("\"");
            return sb.ToString();
        }
    }

    internal class GenericAspectCallCodeBrick : AspectCallCodeBrick
    {
        public GenericAspectCallCodeBrick(ParameterCodeBrick[] param, MethodInfo aspectmethod) :
            base(param, aspectmethod)
        {
        }

        public override Type Emit(ProxyMemberBuilder wmeb)
        {
            // Alle generics suchen und schauen, wie sie in der Zielmethode gesetzt sind
            Type[] genpa = aspectmethod.GetGenericArguments();
            GenericDictionary generics = new GenericDictionary();
            foreach (ParameterCodeBrick pcb in param)
            {
                IGenericParameterCodeBrick gpcb = pcb as IGenericParameterCodeBrick;
                if (gpcb != null)
                {
                    gpcb.AddGenericParameter(generics);
                }
            }
            Type rettype = aspectmethod.ReturnParameter.ParameterType;
            if (rettype.IsGenericParameter)
            {
                if (!generics.ContainsKey(rettype))
                {
                    // Returntyp ermitteln: Methode = ReturnTyp, Konstruktor = TargetType
                    MethodInfo mi = wmeb.JoinPoint.Method as MethodInfo;
                    Type targetrettype = mi != null ? mi.ReturnType : wmeb.DeclaringBuilder.TargetClass;
                    // Wenn void-Methode, dann wird dies auf object gemappt, da void als generic nicht möglich ist
                    if (targetrettype == typeof(void))
                    {
                        targetrettype = typeof(Loom.JoinPoints._Void);
                    }

                    // Constraints anpassen
                    // Es kann sein, dass die Aspektmethode  strengere Constraints für den Rückgabetypen hat
                    // Dies kann aber aufgrund der Covarianz passieren. 
                    targetrettype = TypeCheck.GetCommonType(rettype, targetrettype);

                    generics.Add(rettype, targetrettype);
                }
                //else
                //{
                //    var targettype = TypeCheck.GetCommonType(rettype, generics[rettype]);
                //    generics[rettype] = targettype;
                //}
            }

            // Konkrete Parameter setzen und neue Methode erzeugen
            for (int iPos = 0; iPos < genpa.Length; iPos++)
            {
                Type t;
                if (!generics.TryGetValue(genpa[iPos], out t))
                {
                    throw new AspectWeaverException(1008, Loom.Resources.CodeBuilderErrors.ERR_1008, Common.Convert.ToString(aspectmethod), genpa[iPos].ToString());
                }
                else
                {
                    genpa[iPos] = t;
                }
            }
            aspectmethod = aspectmethod.MakeGenericMethod(genpa);

            return base.Emit(wmeb);
        }

        
    }
}
