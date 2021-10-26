// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Loom.Common;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;


namespace Loom.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class Recall
    {
        private static int c_recallcount = 0;
        private static ModuleBuilder c_targetmodule;

        static Recall()
        {
            AssemblyName an = new AssemblyName("Recall");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            c_targetmodule = assembly.DefineDynamicModule(WeavingCodeNames.c_moduleName, "Recall.dll", true);
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public static void CreateRecallDelegate(ref RecallDelegate del, MethodContext ctx, object[] args)
        {
            // Es kann sein, dass zwei ReCalls parallel eintreffen
            // Daher ist dieser Teil zu sichern und bei der Freigabe zu fragen,
            // ob der Recall bereits erzeugt wurde
            lock (c_targetmodule)
            {
                if (del != null) return;
                // Joinpoints suchen, die für Recall in Frage kommen
                CommonMethodsImpl.SortedJoinPoints micandidates = CommonMethodsImpl.FindCandidates(ctx.TargetClass, ctx.CurrentMethod.Name);

                // Methode für Recall implementieren
                TypeBuilder tb = c_targetmodule.DefineType("__Recall_" + c_recallcount, TypeAttributes.Sealed | TypeAttributes.Public);
                c_recallcount++;
                MethodBuilder mb = tb.DefineMethod("Recall", MethodAttributes.Public | MethodAttributes.Static, typeof(object), ReflectionObjects.c_param_Object_ObjectArray);
                ILGenerator ilgen = mb.GetILGenerator();
                CommonMethodsImpl.ImplDynamicDispatch(ilgen, micandidates, il=>CommonCode.EmitLdArg(il,1), il=>
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Castclass, ctx.TargetClass);
                });

                Type type = tb.CreateType();
                MethodInfo midel = (MethodInfo)c_targetmodule.ResolveMethod(mb.GetToken().Token);

                del = (RecallDelegate)Delegate.CreateDelegate(typeof(RecallDelegate), midel);
            }
        }

       

    }
}
