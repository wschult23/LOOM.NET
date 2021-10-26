// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Loom.JoinPoints.Implementation
{
    /// <summary>
    /// Wenn ein Join Point diese Interface implementiert, bezieht er sich auf eine 
    /// Interfacemethode
    /// </summary>
    internal interface IInterfaceJoinPoint
    {
        /// <summary>
        /// Das Interface aus dem der JoinPoint kommt
        /// </summary>
        Type Interface { get; }
    }

    internal interface IBindInterfaceMethod
    {
        /// <summary>
        /// Erzeugt einen neuen Joinpoint, der das Interface "implementiert"
        /// </summary>
        MethodInfoJoinPoint BindToImplementation(MethodInfo miimplementation, AspectCoverageInfo[] aspects);
    }

    /// <summary>
    /// Diese Interface wird von den Code-Bricks verwendet, um die Sprungadresse für die Basisimplementation zu ermitteln
    /// </summary>
    internal interface IBaseCall
    {
        /// <summary>
        /// Die Methode, die aufgerufen werden muss, um die Basisimplementation zu erreichen
        /// </summary>
        MethodInfo BaseCall { get; }
    }

    /// <summary>
    /// Dieses Interface wird verwendet um den Basiskonstruktor zu ermitteln
    /// </summary>
    internal interface IBaseCtor
    {
        /// <summary>
        /// Zeigt auf den Basiskonstruktor
        /// </summary>
        ConstructorInfo BaseCtor { get; }
        /// <summary>
        /// Zeigt an, dass das erste Argument ein Array der Aspekte ist
        /// </summary>
        bool HasAspectParameter { get; }
    }

    internal class ProxyInterfaceMethodJoinPoint : MethodJoinPoint, IManageJoinPoint, IInterfaceJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="miinterface">Methodendeklaration im Interface</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        public ProxyInterfaceMethodJoinPoint(Type declaringtype, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, miinterface, aspects)
        {
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return miimplementation.GetBaseDefinition(); }
        }

        #endregion

        #region IInterfaceJoinPoint Members

        public Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion
    }

    internal class ProxyInterfacePropertyJoinPoint : PropertyJoinPoint, IManageJoinPoint, IInterfaceJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="piproperty">Propertydeklaration im Interface</param>
        /// <param name="miinterface">Methodendeklaration im Interface</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        public ProxyInterfacePropertyJoinPoint(Type declaringtype, PropertyInfo piproperty, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, piproperty, miimplementation, miinterface, aspects)
        {
        }


        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return miimplementation.GetBaseDefinition(); }
        }

        #endregion

        #region IInterfaceJoinPoint Members


        public Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfacePropertyJoinPoint(this.declaringtype, this.pipropertyinfo, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion
    }

    internal class ProxyInterfaceEventJoinPoint : EventJoinPoint, IManageJoinPoint, IInterfaceJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="miinterface">Methodendeklaration im Interface</param>
        /// <param name="aspects"></param>
        /// <param name="eieventinfo"></param>
        /// <param name="miimplementation"></param>
        public ProxyInterfaceEventJoinPoint(Type declaringtype, EventInfo eieventinfo, MethodInfo miimplementation, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, eieventinfo, miimplementation, miinterface, aspects)
        {
        }


        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return miimplementation.GetBaseDefinition(); }
        }

        #endregion

        #region IInterfaceJoinPoint Members


        public Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceEventJoinPoint(this.declaringtype, this.eieventinfo, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion
    }


    internal class ProxyVirtualMethodJoinPoint : MethodJoinPoint, IManageJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// die urspruengliche Definition der Methode (NewSlot)
        /// </summary>
        MethodInfo mibasedefinition;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        public ProxyVirtualMethodJoinPoint(Type declaringtype, MethodInfo miimplementation, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, null, aspects)
        {
            this.mibasedefinition = miimplementation.GetBaseDefinition();
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return mibasedefinition; }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            System.Diagnostics.Debug.Assert(mibasecallimplementation == null);
            return new GeneratedProxyMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion
    }

    internal class ProxyPropertyJoinPoint : PropertyJoinPoint, IManageJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// die urspruengliche Definition der Methode (NewSlot)
        /// </summary>
        MethodInfo mibasedefinition;

        /// <summary>
        /// Definiert einen Joinpoint für eine Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        /// <param name="piproperty"></param>
        public ProxyPropertyJoinPoint(Type declaringtype, PropertyInfo piproperty, MethodInfo miimplementation, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, piproperty, miimplementation, null, aspects)
        {
            this.mibasedefinition = miimplementation.GetBaseDefinition();
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return mibasedefinition; }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            System.Diagnostics.Debug.Assert(mibasecallimplementation == null);
            return new GeneratedProxyPropertyJoinPoint(this.declaringtype, this.pipropertyinfo, this.miimplementation, miimplementation, this.Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion
    }


    internal class ProxyEventJoinPoint : EventJoinPoint, IManageJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// die urspruengliche Definition der Methode (NewSlot)
        /// </summary>
        MethodInfo mibasedefinition;

        /// <summary>
        /// Definiert einen Joinpoint für eine Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        /// <param name="evevent"></param>
        public ProxyEventJoinPoint(Type declaringtype, EventInfo evevent, MethodInfo miimplementation, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, evevent, miimplementation, null, aspects)
        {
            this.mibasedefinition = miimplementation.GetBaseDefinition();
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return mibasedefinition; }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            System.Diagnostics.Debug.Assert(mibasecallimplementation == null);
            return new GeneratedProxyEventJoinPoint(this.declaringtype, this.eieventinfo, this.miimplementation, miimplementation, this.Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion
    }


    internal class ProxyConstructorJoinPoint : ConstructorJoinPoint, IManageJoinPoint, IBaseCall, IBaseCtor, IBindConstructor
    {
        /// <summary>
        /// Die Activatormethode, welche die Adviceaufrufe enthält
        /// </summary>
        MethodBuilder micreateai;

        public MethodBuilder CreateAIMethod
        {
            get { return micreateai; }
        }
        /// <summary>
        /// Die Activatormethode, welche das Objekt erzeugt
        /// </summary>
        MethodBuilder micreateto;

        public MethodBuilder CreateTOMethod
        {
            get { return micreateto; }
        }

        public void SetCreateMethods(MethodBuilder micreateai, MethodBuilder micreateto)
        {
            this.micreateai = micreateai;
            this.micreateto = micreateto;
        }

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="ciconstructor">der Konstructor</param>
        /// <param name="aspects"></param>
        public ProxyConstructorJoinPoint(Type declaringtype, ConstructorInfo ciconstructor, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, ciconstructor, aspects)
        {
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return ciconstructor; }
        }

        #endregion

        #region IBindConstructor Members

        ConstructorJoinPoint IBindConstructor.BindToType(ConstructorBuilder miimplementation, MethodBuilder micreateai, MethodBuilder micreateto)
        {
            return new GeneratedProxyConstructorJoinPoint(this.declaringtype, this.ciconstructor, miimplementation, micreateai, micreateto, Aspects);
        }

        #endregion

        #region IBaseCall Members

        public MethodInfo BaseCall
        {
            get { return micreateai; }
        }

        #endregion

        #region IBaseCtor Members

        ConstructorInfo IBaseCtor.BaseCtor
        {
            get { return ciconstructor; }
        }

        bool IBaseCtor.HasAspectParameter
        {
            get { return false; }
        }

        #endregion
    }

    internal class ProxyDestroyMethodJoinPoint : DestroyMethodJoinPoint, IManageJoinPoint, IBaseCall, IBindMethod
    {
        /// <summary>
        /// die ursprüngliche Definition der Methode (NewSlot)
        /// </summary>
        MethodInfo mibasedefinition;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        public ProxyDestroyMethodJoinPoint(Type declaringtype, MethodInfo miimplementation, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, aspects)
        {
            this.mibasedefinition = miimplementation.GetBaseDefinition();
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return mibasedefinition; }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            System.Diagnostics.Debug.Assert(mibasecallimplementation == null);
            return new GeneratedProxyDestroyMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion
    }

    /// <summary>
    /// Diese Klasse Repraesentiert alle übrigen JoinPoints die nicht verwoben werden können (statisch oder nicht-virtuelle Klassenmethoden, die nicht über ein Interface erreichbar sind)
    /// </summary>
    internal class ProxyMethodJoinPoint : MethodJoinPoint, IManageJoinPoint, IBaseCall
    {
        /// <summary>
        /// die urspruengliche Definition der Methode (NewSlot)
        /// </summary>
        MethodInfo mibasedefinition;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="declaringtype">Type, in dem das Interface declariert wurde</param>
        /// <param name="aspects"></param>
        /// <param name="miimplementation"></param>
        public ProxyMethodJoinPoint(Type declaringtype, MethodInfo miimplementation, AspectCoverageInfo[] aspects)
            :
            base(declaringtype, miimplementation, null, aspects)
        {
            this.mibasedefinition = GetBaseDefinition(declaringtype, miimplementation.Name, Common.Conversion.ToTypeArray(miimplementation.GetParameters()));
            System.Diagnostics.Debug.Assert(mibasedefinition != null);
        }

        /// <summary>
        /// Sucht von einer Methode die erste Implementierung in der Vererbungshirarchie
        /// </summary>
        /// <param name="declaringtype"></param>
        /// <param name="name"></param>
        /// <param name="paramtypes"></param>
        /// <returns></returns>
        private MethodInfo GetBaseDefinition(Type declaringtype, string name, Type[] paramtypes)
        {
            MethodInfo mi;
            if (declaringtype.BaseType != null)
            {
                mi = GetBaseDefinition(declaringtype.BaseType, name, paramtypes);
            }
            else mi = null;

            if (mi == null)
            {
                mi = declaringtype.GetMethod(
                    name,
                    BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly,
                    null,
                    paramtypes,
                    null);
            }

            return mi;
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return mibasedefinition; }
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miimplementation; }
        }

        #endregion
    }
}
