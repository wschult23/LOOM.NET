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

using Loom.AspectModel;
using Loom.Common;
using Loom.Runtime;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy
{


    internal class ProxyTypeActivatorBuilder : ProxyTypeBuilderBase
    {
        protected ProxyTypeBuilder proxytypebuilder;
        protected Type parentClassObjectType;

        /// <summary>
        /// Baut ein Classenobjekt für einen unverwobenen Typen
        /// </summary>
        /// <param name="targetmodule"></param>
        public ProxyTypeActivatorBuilder(ModuleBuilder targetmodule)
            :
            base(targetmodule)
        {
        }

        /// <summary>
        /// Baut ein Classenobjekt
        /// </summary>
        /// <param name="targetmodule">der Typebuilder des Proxies</param>
        /// <param name="parentclassobject">des Klassenobjekt des Parents</param>
        public ProxyTypeActivatorBuilder(ModuleBuilder targetmodule, ClassObject parentclassobject)
            :
            base(targetmodule)
        {
            parentClassObjectType = parentclassobject.GetType();
        }

        public AspectField[] AspectFields
        {
            get
            {
                return proxytypebuilder.Declarations.Aspects;
            }
        }

        public void SetProxyTypeBuilder(ProxyTypeBuilder proxytypebuilder)
        {
            this.proxytypebuilder = proxytypebuilder;
        }

        public Dictionary<string, JPVariableDeclaration> SharedVariables
        {
            get
            {
                return proxytypebuilder.Declarations.SharedVariables;
            }
        }

        public TypeBuilder InterwovenType
        {
            get { return proxytypebuilder.TypeBuilder; }
        }

        /// <summary>
        /// der Aspekttyp
        /// </summary>
        public override Type AspectType
        {
            get { return proxytypebuilder.AspectType; }
        }

        /// <summary>
        /// Basisklasse des neuen Typs
        /// </summary>
        public override Type BaseType
        {
            get { return proxytypebuilder.BaseType; }
        }

        /// <summary>
        /// Zielklasse
        /// </summary>
        public override Type TargetClass
        {
            get { return proxytypebuilder.TargetClass; }
        }

        /// <summary>
        /// Baut Code zum erzeugen des richtigen Konstruktors
        /// </summary>
        /// <param name="pcb"></param>
        /// <param name="matches"></param>
        /// <param name="aspectindex"></param>
        /// <returns></returns>
        public ProxyMemberBuilder DefineConstructor(ProxyConstructorBuilder pcb, AspectMemberCollection matches, int aspectindex)
        {
            ProxyMemberBuilder pmb;
            if (TargetClass.IsSealed)
            {
                if (matches != null)
                {
                    pmb = new ProxySealedAdviceConstructorBuilder(this, pcb.JoinPoint, aspectindex);
                    pmb.Interweave(matches);
                }
                else
                {
                    pmb = new ProxySealedNoAdviceConstructorBuilder(this, pcb.JoinPoint);
                }

            }
            else
            {
                if (matches != null)
                {
                    pmb = new ProxyAdviceConstructorBuilder(this, pcb, aspectindex);
                    pmb.Interweave(matches);
                }
                else
                {
                    pmb = new ProxyNoAdviceConstructorBuilder(this, pcb, aspectindex);
                }
            }
            return pmb;
        }

        /// <summary>
        /// Diese Methode muss vor dem Erzeugen der MethodCodeBuilder des Proxys erfolgen
        /// </summary>
        public virtual void DefineType(string typename, bool abstractimpl, ICollection<AspectField> aspects)
        {
            // Das ist das ClasObjekt, welches die Wurzel der vererbungshirarchie darstellt
            if (parentClassObjectType == null)
            {
                TypeBuilder tb = ClassObjectImpl.DefineClassObjectWithDynamicDispatch(TargetModule, WeavingCodeNames.GetClassObjectName(BaseType), TargetClass, proxytypebuilder.CurrentJoinPoints.CtorJoinPoints, true);
                parentClassObjectType = tb.CreateType();
            }

            Type newtype = proxytypebuilder.TypeBuilder;
            // wenn die Klasse sealed ist, existiert kein Typebuilder und wir nehmen die Targetclass
            if (newtype == null) newtype = TargetClass;

            this.typebuilder = ClassObjectImpl.DefineClassObject(TargetModule, typename, parentClassObjectType, newtype, abstractimpl);

            foreach (Declaration ds in Declarations)
            {
                ds.DefineMember(this);
            }

            // GetAspects implementieren
            if (aspects != null)
            {
                ClassObjectImpl.Impl_GetAspects(this.typebuilder, aspects);
            }
        }

        /// <summary>
        /// Baut das Klassenobjekt und bereitet alles für eine weitere verwebung vor.
        /// </summary>
        /// <returns></returns>
        public Type CreateClassObject()
        {
            parentClassObjectType = typebuilder.CreateType();
            foreach (Declaration decl in Declarations)
            {
                decl.CreateType();
            }
            return parentClassObjectType;
        }

        public ClassObject CreateInstance()
        {
            if (parentClassObjectType == null)
            {
                // Typ ist nicht verwoben, es wird aber trotzdem ein Klassenobjekt benötigt
                typebuilder = ClassObjectImpl.DefineClassObjectWithDynamicDispatch(TargetModule, Common.WeavingCodeNames.GetClassObjectName(proxytypebuilder.BaseType), TargetClass, proxytypebuilder.CurrentJoinPoints.CtorJoinPoints, false);
                parentClassObjectType = typebuilder.CreateType();
            }
            return (ClassObject)Activator.CreateInstance(parentClassObjectType);
        }


        public override bool Interweave(Aspect aspect)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Interweave()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override ContextClassDeclaration CreateIntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb)
        {
            throw new NotImplementedException();
        }

        public override ContextClassDeclaration CreateCallContextClassDeclaration(ProxyAdviceMethodBuilder meb)
        {
            throw new NotImplementedException();
        }
    }
}
