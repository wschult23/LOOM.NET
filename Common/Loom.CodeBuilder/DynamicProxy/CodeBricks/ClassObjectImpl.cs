// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

using Loom.Common;
using Loom.Runtime;
using Loom.JoinPoints.Implementation;
using System.Collections;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{

    internal struct ParameterContainer : IComparable<ParameterContainer>
    {
        private Type[] parameters;

        private ParameterContainer(Type[] parameters)
        {
            this.parameters = parameters;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ParameterContainer)) return false;
            ParameterContainer other = (ParameterContainer)obj;
            if (other.parameters.Length != parameters.Length) return false;
            for (int iPos = 0; iPos < parameters.Length; iPos++)
            {
                if (other.parameters[iPos] != parameters[iPos]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return parameters.GetHashCode();
        }

        public static implicit operator Type[](ParameterContainer pc)
        {
            return pc.parameters;
        }

        public static implicit operator ParameterContainer(Type[] types)
        {
            return new ParameterContainer(types);
        }

        #region IComparable<ParameterContainer> Members
        public int CompareTo(ParameterContainer other)
        {
            // Längendifferenz ?
            int diff = parameters.Length - other.parameters.Length;
            if (diff != 0) return diff;
            // ersten unterschiedlichen Parameter suchen
            if (parameters.Length == 0) return 0;
            Type tx = null;
            Type ty = null;
            for (int iPos = 0; iPos < parameters.Length; iPos++)
            {
                tx = parameters[iPos];
                ty = other.parameters[iPos];
                if (tx != ty) break;
            }
            // ist einer spezieller als der andere?
            if (tx.IsSubclassOf(ty)) return -1;
            if (ty.IsSubclassOf(tx)) return 1;

            // Generics nach hinten
            diff = System.Convert.ToInt32(tx.IsGenericParameter);
            diff -= System.Convert.ToInt32(ty.IsGenericParameter);
            // TODO: wenn diff = 0 mässte man jetzt das Spielchen mit den Constraints der Generics weitermachen
            if (diff != 0) return diff;

            return GetHashCode() - other.GetHashCode();
        }
        #endregion
    }

    internal class ConstructorInfoDictionary : Dictionary<ParameterContainer, ConstructorInfo> { };

    internal class ClassObjectImpl
    {
        public static string c_CreateTo_Name = "__CreateTO";
        public static string c_CreateAi_Name = "__CreateAI";

        /// <summary>
        /// Baut ein Klassenobjekt für einen unverwobenen Basistypen (ohne Aspektparameter)
        /// </summary>
        /// <param name="targetmodule"></param>
        /// <param name="name"></param>
        /// <param name="basetype"></param>
        /// <param name="ctors">alle Konstruktorjoinpoints für diesen Typ</param>
        /// <param name="abstractimpl"></param>
        /// <returns></returns>
        public static TypeBuilder DefineClassObject(ModuleBuilder targetmodule, string name, Type basetype, MethodBaseJoinPoint[] ctors, bool abstractimpl)
        {
            TypeAttributes tattr = TypeAttributes.Public | TypeAttributes.Serializable;
            if (abstractimpl) tattr |= TypeAttributes.Abstract;
            TypeBuilder tb = targetmodule.DefineType(name, tattr, typeof(ClassObject));
            Type[] generics = null;
            if (basetype.IsGenericTypeDefinition)
            {
                System.Diagnostics.Debug.Fail("Generics still not implemented");
                var gen=basetype.GetGenericArguments();
                string[] genp=gen.Select(t=>t.Name).ToArray();
                generics=tb.DefineGenericParameters(genp);
            }

            // Type und Targetclass implementieren. Targetclass wird nur hier implementiert, während Type bei jeder weiteren
            // überladung neu ersetzt wird.
            CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_ClassObject_getTargetClass, basetype);
            if (!abstractimpl)
            {
                CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_ClassObject_getType, basetype);
            }

            foreach (ProxyConstructorJoinPoint jp in ctors)
            {
                Type[] ctorargs = Common.Conversion.ToTypeArray(jp.ConstructorImplementation.GetParameters());
                Type[] parameters = GetCreateMethodArgTypes(ctorargs);

                MethodBuilder mito;
                if (abstractimpl)
                {
                    mito = Impl__CreateTO_Abstract(tb, parameters);
                }
                else if (basetype.IsAbstract)
                {
                    mito = Impl__CreateTO_AbstractClass(tb, parameters);
                }
                else
                {
                    mito = Impl__CreateTO(tb, jp.ConstructorImplementation, parameters, false, true);
                }
                MethodBuilder miai = Impl__CreateAI(tb, mito, parameters);

                jp.SetCreateMethods(miai, mito);
            }


            return tb;
        }

        /// <summary>
        /// Baut ein Klassenobjekt für einen unverwobenen Basistypen (ohne Aspektparameter)
        /// </summary>
        /// <param name="targetmodule"></param>
        /// <param name="name"></param>
        /// <param name="basetype"></param>
        /// <param name="ctors">alle Konstruktorjoinpoints für diesen Typ</param>
        /// <param name="abstractimpl"></param>
        /// <returns></returns>
        public static TypeBuilder DefineClassObjectWithDynamicDispatch(ModuleBuilder targetmodule, string name, Type basetype, MethodBaseJoinPoint[] ctors, bool abstractimpl)
        {
            var tb = DefineClassObject(targetmodule, name, basetype, ctors, abstractimpl);
            CommonMethodsImpl.SortedJoinPoints ctorlist = new CommonMethodsImpl.SortedJoinPoints();
            foreach (ProxyConstructorJoinPoint jp in ctors)
            {
                Type[] ctorargs = Common.Conversion.ToTypeArray(jp.ConstructorImplementation.GetParameters());
                ctorlist.Add(ctorargs, jp.CreateAIMethod);
            }
            Impl_CreateInstanceInternal(tb, ctorlist);

            return tb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetmodule"></param>
        /// <param name="name"></param>
        /// <param name="baseclasstype">Basisklassenobjket</param>
        /// <param name="newtype">neuer typ</param>
        /// <param name="abstractimpl"></param>
        /// <returns></returns>
        public static TypeBuilder DefineClassObject(ModuleBuilder targetmodule, string name, Type baseclasstype, Type newtype, bool abstractimpl)
        {
            TypeBuilder tb;
            if (abstractimpl)
            {
                tb = targetmodule.DefineType(name, TypeAttributes.Public | TypeAttributes.Serializable | TypeAttributes.Abstract, baseclasstype);
            }
            else
            {
                tb = targetmodule.DefineType(name, TypeAttributes.Public | TypeAttributes.Serializable, baseclasstype);
                CommonMethodsImpl.Impl_get_TypeProperty(tb, ReflectionObjects.c_mi_ClassObject_getType, newtype);
            }

            return tb;
        }

        private static MethodInfo Impl_CreateInstanceInternal(TypeBuilder tb, CommonMethodsImpl.SortedJoinPoints ctors)
        {
            MethodAttributes mattr = ReflectionObjects.c_mi_ClassObject_CreateInstanceInternal.Attributes;
            mattr -= MethodAttributes.NewSlot;

            MethodBuilder mbcreate = tb.DefineMethod(ReflectionObjects.c_mi_ClassObject_CreateInstanceInternal.Name, mattr, typeof(object), ReflectionObjects.c_param_AspectArray_ObjectArray);
            ILGenerator ilgen = mbcreate.GetILGenerator();

            CommonMethodsImpl.ImplDynamicDispatch(ilgen, ctors, il => il.Emit(OpCodes.Ldarg_2), il => 
            { 
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);  
            });

            return mbcreate;
        }


        /// <summary>
        /// Baut Code um das verwobene Objekt zu erzeugen
        /// </summary>
        /// <param name="tb">Das ClassObject</param>
        /// <param name="cicreate">Der Konstruktor des verwobenen Objetes</param>
        /// <param name="paramtypes">Die Parameter der CreateTO Methode</param>
        /// <param name="bOverride">Basisimplementierung oder überladung</param>
        /// <param name="bistargetclass"></param>
        /// <returns></returns>
        public static MethodBuilder Impl__CreateTO(TypeBuilder tb, ConstructorInfo cicreate, Type[] paramtypes, bool bOverride, bool bistargetclass)
        {
            MethodAttributes mattr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            if (!bOverride) mattr |= MethodAttributes.NewSlot;

            MethodBuilder mbcreate = tb.DefineMethod(c_CreateTo_Name, mattr, typeof(object), paramtypes);
            ILGenerator ilgen = mbcreate.GetILGenerator();
            int count = bistargetclass ? paramtypes.Length - 1 : paramtypes.Length;
            CommonCode.EmitLdArgList(ilgen, 1, count);
            ilgen.Emit(OpCodes.Newobj, cicreate);
            ilgen.Emit(OpCodes.Ret);

            return mbcreate;
        }

        /// <summary>
        /// Baut Code um abstrakte Objekte zu bauen
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static MethodBuilder Impl__CreateTO_AbstractClass(TypeBuilder tb, Type[] parameters)
        {
            MethodAttributes mattr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            MethodBuilder meb = tb.DefineMethod(c_CreateTo_Name, mattr, typeof(object), parameters);
            ILGenerator ilgen = meb.GetILGenerator();
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, ReflectionObjects.c_ci_AbstractClassException_);
            ilgen.Emit(OpCodes.Throw);

            return meb;
        }

        /// <summary>
        /// Baut Code um abstrakte Objekte zu bauen
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static MethodBuilder Impl__CreateTO_Abstract(TypeBuilder tb, Type[] parameters)
        {
            MethodAttributes mattr = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual;
            MethodBuilder meb = tb.DefineMethod(c_CreateTo_Name, mattr, typeof(object), parameters);
            meb.GetILGenerator();
            return meb;
        }


        /// <summary>
        /// Baut Code, in dem die Aspekte eingewoben werden
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="micreateto"></param>
        /// <param name="paramtypes"></param>
        /// <returns></returns>
        private static MethodBuilder Impl__CreateAI(TypeBuilder tb, MethodInfo micreateto, Type[] paramtypes)
        {
            MethodAttributes mattr = MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot;

            MethodBuilder mbcreate = tb.DefineMethod(c_CreateAi_Name, mattr, typeof(object), paramtypes);
            ILGenerator ilgen = mbcreate.GetILGenerator();
            CommonCode.EmitLdArgList(ilgen, 0, paramtypes.Length + 1);
            ilgen.Emit(OpCodes.Tailcall);
            ilgen.Emit(OpCodes.Callvirt, micreateto);
            ilgen.Emit(OpCodes.Ret);

            return mbcreate;
        }


        /// <summary>
        /// Ermittelt aus dem Zielklassenkonstruktor die Signatur für die CreateTO und CreateAI Methoden
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        public static Type[] GetCreateMethodArgTypes(ConstructorInfo ci)
        {
            ParameterInfo[] parameters = ci.GetParameters();
            Type[] argtypes = new Type[parameters.Length + 1];
            argtypes[0] = typeof(Aspect[]);
            for (int iPos = 0; iPos < parameters.Length; iPos++)
            {
                argtypes[iPos + 1] = parameters[iPos].ParameterType;
            }
            return argtypes;
        }

        /// <summary>
        /// Ermittelt aus dem Zielklassenkonstruktor-Signatur die Signatur für die CreateTO und CreateAI Methoden
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Type[] GetCreateMethodArgTypes(Type[] parameters)
        {
            Type[] argtypes = new Type[parameters.Length + 1];
            argtypes[0] = typeof(Aspect[]);
            parameters.CopyTo(argtypes, 1);
            return argtypes;
        }


        public static FieldInfo Imp_StaticCtor(TypeBuilder tb)
        {
            ConstructorInfo ci = tb.DefineDefaultConstructor(MethodAttributes.Private);
            FieldInfo fi = tb.DefineField("_obj", tb, FieldAttributes.Static | FieldAttributes.Private);
            ConstructorBuilder cb = tb.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, Common.ReflectionObjects.c_param_);
            ILGenerator ilgen = cb.GetILGenerator();
            ilgen.Emit(OpCodes.Newobj, ci);
            ilgen.Emit(OpCodes.Stsfld, fi);
            ilgen.Emit(OpCodes.Ret);

            return fi;
        }

        public static void Impl_Create(TypeBuilder tb, FieldInfo fi, MethodInfo micreateai, Type targetclass, Type[] paramtypes)
        {
            MethodBuilder mb = tb.DefineMethod("Create", MethodAttributes.Public | MethodAttributes.Static, targetclass, paramtypes);
            ILGenerator ilgen = mb.GetILGenerator();

            ilgen.Emit(OpCodes.Ldsfld, fi);
            ilgen.Emit(OpCodes.Dup);
            ilgen.Emit(OpCodes.Ldc_I4_0);
            ilgen.Emit(OpCodes.Callvirt, Common.ReflectionObjects.c_mi_ClassObject_GetAspectArray);
            CommonCode.EmitLdArgList(ilgen, 0, paramtypes.Length);
            ilgen.Emit(OpCodes.Callvirt, micreateai);
            ilgen.Emit(OpCodes.Castclass, targetclass);
            ilgen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Erzeugt die Methode, die die Aspecte liefert
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="aspects"></param>
        /// <returns></returns>
        static internal void Impl_GetAspects(TypeBuilder tb, ICollection<AspectField> aspects)
        {
            //Methode anlegen
            MethodAttributes mattr = ReflectionObjects.c_mi_ClassObject_GetAspectArray.Attributes;
            mattr -= MethodAttributes.NewSlot;

            MethodBuilder mbcreate = tb.DefineMethod(ReflectionObjects.c_mi_ClassObject_GetAspectArray.Name, mattr, typeof(Aspect[]), ReflectionObjects.c_param_bool);
            ILGenerator ilgen = mbcreate.GetILGenerator();

            /// Array anlegen, in dem die Aspekte gespeichert werden
            CommonCode.EmitLdci4(ilgen, aspects.Count);
            ilgen.Emit(OpCodes.Newarr, typeof(Aspect));
            
            // Array initialisieren
            Label lb=default(Label);
            bool bFirst = true;

            foreach(var af in aspects.Where(asp=>!asp.IsReferencedAspectStatic))
            {
                if (bFirst)
                {
                    lb = ilgen.DefineLabel();
                    ilgen.Emit(OpCodes.Ldarg_1);
                    ilgen.Emit(OpCodes.Brtrue, lb);
                    bFirst = false;
                }
                StoreAspectToArray(ilgen, af);
            }
            
            if (!bFirst)
            {
                ilgen.MarkLabel(lb);
            }

            foreach(var af in aspects.Where(asp=>asp.IsReferencedAspectStatic))
            {
                StoreAspectToArray(ilgen, af);
            }

            ilgen.Emit(OpCodes.Ret);
        }

        private static void StoreAspectToArray(ILGenerator ilgen, AspectField af)
        {
            ilgen.Emit(OpCodes.Dup);
            CommonCode.EmitLdci4(ilgen, af.ArrayPosition);
            af.EmitInitialize(ilgen);
            ilgen.Emit(OpCodes.Stelem_Ref);
        }

    }
}
