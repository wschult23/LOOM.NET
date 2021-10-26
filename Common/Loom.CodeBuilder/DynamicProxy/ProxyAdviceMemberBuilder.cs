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

using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;

namespace Loom.CodeBuilder.DynamicProxy
{

    /// <summary>
    /// Basisklasse zum erzeugen von Methoden und Konstruktoren.
    /// Diese Klasse wird von den <see cref="CodeBrick"/> Klassen verwendet.
    /// </summary>
    internal abstract class ProxyAdviceMemberBuilder : ProxyMemberBuilder
    {
        /// <summary>
        /// code bricks
        /// </summary>
        protected List<CodeBrick> invokeAfterThrowing;
        protected List<CodeBrick> invokeAfterReturning;
        protected List<CodeBrick> invokeBefore;
        protected List<CodeBrick> invokeAfter;
        protected CodeBrick methodcode;



        public ProxyAdviceMemberBuilder(ProxyTypeBuilderBase tb, Loom.JoinPoints.Implementation.MethodBaseJoinPoint jp)
            :
            base(tb, jp)
        {
        }

        protected static void Add(ref List<CodeBrick> list, CodeBrick cb)
        {
            if (list == null) list = new List<CodeBrick>();
            list.Add(cb);
        }

        #region CodeBrick Access
        public void AddBefore(CodeBrick cb)
        {
            Add(ref invokeBefore, cb);
        }

        public void AddAfter(CodeBrick cb)
        {
            Add(ref invokeAfter, cb);
        }

        public void AddAfterThrowing(CodeBrick cb)
        {
            Add(ref invokeAfterThrowing, cb);
        }

        public void AddAfterReturning(CodeBrick cb)
        {
            Add(ref invokeAfterReturning, cb);
        }

        public void DefineMethodCode(CodeBrick cb)
        {
            if (methodcode != null)
            {
                throw new AspectWeaverException(1002, Loom.Resources.CodeBuilderErrors.ERR_1002, joinpoint.ToString(), DeclaringBuilder.TargetClass.ToString());
            }
            methodcode = cb;
        }
        #endregion

        /// <summary>
        /// Wenn Enumeration nicht leer, alle Codebricks emitieren
        /// </summary>
        /// <param name="en"></param>
        /// <param name="ilgen"></param>
        protected void EmitCodeBrickEnumeration(IEnumerable<CodeBrick> en, ILGenerator ilgen)
        {
            if (en != null)
            {
                foreach (CodeBrick cb in en)
                {
                    CommonCode.EmitAdjustReturnValue(ilgen, cb.Emit(this), typeof(void));
                }
            }
        }

        /// <summary>
        /// Erzeugt den Methodencode
        /// </summary>
        /* Diese Methode erzeugt folgenden Code
        // 
         * Initialization
         * 
         * try *1
         * {
         * 	    [invokebefore]
        //		[methodcode]
        //		store retval  *2
        //		[invokeafterreturning]
        //		leave tryblock *1
         * }
         * catch(Exception e) *3
         * {
         *      store e *5
         *      [invokeafterthrowing]
         * }
         * finally *4
         * {
         *      [invokeafter]
         * }
         * return
         * 
         * 
         * (*1) (*3) || (*4)
         * (*2) Locals.Retval!=null
         * (*3) invokeafterthrowing !=null
         * (*4) invokeafter != null
         * (*5) Locals.Exception!=null
         */
        protected void EmitMethod(MethodBuilder meb)
        {
            bool bNeedTry = invokeAfterThrowing != null || invokeAfter != null;
            // wird eine lokale Variable zum speichern des Rückgabewerts benötigt?
            // bei invokeafter und nach dem try wir ohne nochmalige Prüfung der wert dort gespeichert
            if (DataSlots.RetVal == null && (bNeedTry || invokeAfterReturning != null) && meb.ReturnType != typeof(void))
            {
                DataSlots.RetVal = new LocalObject(meb.ReturnType);
            }

            // Initialisierungscode rendern
            this.ilgen = meb.GetILGenerator();
            foreach (DataSlot lv in DataSlots)
            {
                lv.EmitInitialization(this);
            }


            // try
            if (bNeedTry)
            {
                ilgen.BeginExceptionBlock();
            }

            // [invokeBefore]
            EmitCodeBrickEnumeration(invokeBefore, ilgen);

            // [methodcode]
            if (methodcode != null)
            {
                CommonCode.EmitAdjustReturnValue(ilgen, methodcode.Emit(this), meb.ReturnType);

                // store retval
                if (DataSlots.RetVal != null)
                {
                    DataSlots.RetVal.EmitStoreValue(this);
                }
            }

            // [invokeafterreturning] (lbimplicitparam)
            EmitCodeBrickEnumeration(invokeAfterReturning, ilgen);

            // leave tryblock

            // (*5)
            if (invokeAfterThrowing != null)
            {
                // catch(Exception e)
                ilgen.BeginCatchBlock(typeof(Exception));
                if (DataSlots.Exception != null)
                {
                    DataSlots.Exception.EmitStoreValue(this);
                }
                else
                {
                    ilgen.Emit(OpCodes.Pop);
                }

                // [invokeafterreturning] 
                if (meb.ReturnType != typeof(void))
                {
                    // beim InvokeafterThrowing kann der Rückgabewert gesetzt werden!
                    foreach (CodeBrick cb in invokeAfterThrowing)
                    {
                        CommonCode.EmitAdjustReturnValue(ilgen, cb.Emit(this), meb.ReturnType);
                        DataSlots.RetVal.EmitStoreValue(this);
                    }
                }
                else
                {
                    EmitCodeBrickEnumeration(invokeAfterThrowing, ilgen);

                }
            }

            if (invokeAfter != null)
            {
                // finally
                ilgen.BeginFinallyBlock();

                // [ivokeafter]
                EmitCodeBrickEnumeration(invokeAfter, ilgen);
            }

            //EndExceptionblock 1
            if (bNeedTry)
            {
                ilgen.EndExceptionBlock();
            }

            if (DataSlots.RetVal != null)
            {
                DataSlots.RetVal.EmitLoadValue(this, meb.ReturnType);
            }
            ilgen.Emit(OpCodes.Ret);

            this.ilgen = null;
        }
    }
}
