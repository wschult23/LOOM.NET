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

using Loom.JoinPoints.Implementation;
using System.Text;

namespace Loom.JoinPoints.Implementation
{
    /// <summary>
    /// Generierte JoinPoints sehen für das Matching so aus, als würden sie auf der Zielklasse liegen
    /// Insbesondere wird der DeclaringType und Method nicht mehr nachgezogen und bleiben auf der TargetClass
    /// Des weiteren werden MethodInfos für die verwebung erzeugt
    /// Hier werden allerdings die MethodBuilder-Objekte verwendet, die Korresbondieren aber nicht mit den 
    /// Tatsächlichen Methodinfo-Objekten und durfen daher auch nur zur Codeerzeugung verwendet und nicht auf den 
    /// JoinPoint-Interfaces sichtbar werden
    /// </summary>
    internal class GeneratedProxyInterfaceMethodJoinPoint : ProxyInterfaceMethodJoinPoint, IBindMethod, IBaseCall
    {
        /// <summary>
        /// Methode zum Aufruf der Basisimplementierung
        /// </summary>
        MethodBuilder mibasecallimplementation;
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="targetclass">Die Zielklasse</param>
        /// <param name="mibaseimplementation">die Implementation in der Zielklasse oder Interface</param>
        /// <param name="mibasecallimplementation">Methode, die aufgerufen werden muss um die Basisimplementierung zu erreichen</param>
        /// <param name="aspects"></param>
        /// <param name="miinterface"></param>
        /// <param name="miproxyimplementation"></param>
        public GeneratedProxyInterfaceMethodJoinPoint(Type targetclass, MethodInfo mibaseimplementation, MethodBuilder miproxyimplementation, MethodInfo miinterface, MethodBuilder mibasecallimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, mibaseimplementation, miinterface, aspects)
        {
            this.mibasecallimplementation = mibasecallimplementation;
            this.miproxyimplementation = miproxyimplementation;
        }

        public override string ToString()
        {
            if (miimplementation.DeclaringType.IsInterface)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Common.Convert.ToString(miinterface));
                sb.Append(" introduced to ");
                sb.Append(DeclaringType.FullName);
                return sb.ToString();
            }
            return base.ToString();
        }

        #region IInterfaceJoinPoint Members

        public new Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get
            {
                return mibasecallimplementation != null ? mibasecallimplementation : miproxyimplementation;
            }
        }

        #endregion
    }

    internal class GeneratedProxyInterfacePropertyJoinPoint : ProxyInterfacePropertyJoinPoint, IBindMethod, IInterfaceJoinPoint, IBaseCall
    {
        /// <summary>
        /// Methode zum Aufruf der Basisimplementierung
        /// </summary>
        MethodBuilder mibasecallimplementation;
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="miinterface">Methodendeklaration im Interface</param>
        /// <param name="aspects"></param>
        /// <param name="mibasecallimplementation"></param>
        /// <param name="mibaseimplementation"/>
        /// <param name="miproxyimplementation"/>
        /// <param name="pibaseproperty"/>
        /// <param name="targetclass"/>
        public GeneratedProxyInterfacePropertyJoinPoint(Type targetclass, PropertyInfo pibaseproperty, MethodInfo mibaseimplementation, MethodBuilder miproxyimplementation, MethodInfo miinterface, MethodBuilder mibasecallimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, pibaseproperty, mibaseimplementation, miinterface, aspects)
        {
            this.mibasecallimplementation = mibasecallimplementation;
            this.miproxyimplementation = miproxyimplementation;
        }

        public override string ToString()
        {
            if (miimplementation.DeclaringType.IsInterface)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Common.Convert.ToString(miinterface));
                sb.Append(" introduced to ");
                sb.Append(DeclaringType.FullName);
                return sb.ToString();
            }
            return base.ToString();
        }


        #region IInterfaceJoinPoint Members


        public new Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfacePropertyJoinPoint(this.declaringtype, this.pipropertyinfo, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get
            {
                return mibasecallimplementation != null ? mibasecallimplementation : miproxyimplementation;
            }
        }

        #endregion
    }

    internal class GeneratedProxyInterfaceEventJoinPoint : ProxyInterfaceEventJoinPoint, IBindMethod, IInterfaceJoinPoint, IBaseCall
    {
        /// <summary>
        /// Methode zum Aufruf der Basisimplementierung
        /// </summary>
        MethodBuilder mibasecallimplementation;
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="miinterface">Methodendeklaration im Interface</param>
        /// <param name="aspects"></param>
        /// <param name="evbaseevent"></param>
        /// <param name="mibasecallimplementation"></param>
        /// <param name="mibaseimplementation"></param>
        /// <param name="miproxyimplementation"></param>
        /// <param name="targetclass"></param>
        public GeneratedProxyInterfaceEventJoinPoint(Type targetclass, EventInfo evbaseevent, MethodInfo mibaseimplementation, MethodBuilder miproxyimplementation, MethodInfo miinterface, MethodBuilder mibasecallimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, evbaseevent, mibaseimplementation, miinterface, aspects)
        {
            this.mibasecallimplementation = mibasecallimplementation;
            this.miproxyimplementation = miproxyimplementation;
        }

        public override string ToString()
        {
            if (miimplementation.DeclaringType.IsInterface)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Common.Convert.ToString(miinterface));
                sb.Append(" introduced to ");
                sb.Append(DeclaringType.FullName);
                return sb.ToString();
            }
            return base.ToString();
        }


        #region IInterfaceJoinPoint Members


        public new Type Interface
        {
            get
            {
                return miinterface.DeclaringType;
            }
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceEventJoinPoint(this.declaringtype, this.eieventinfo, this.miimplementation, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get
            {
                return mibasecallimplementation != null ? mibasecallimplementation : miproxyimplementation;
            }
        }

        #endregion
    }


    internal class GeneratedProxyMethodJoinPoint : ProxyVirtualMethodJoinPoint, IBindMethod, IBaseCall
    {
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="mibaseimplementation"></param>
        /// <param name="miproxyimplementation"></param>
        /// <param name="targetclass"></param>
        public GeneratedProxyMethodJoinPoint(Type targetclass, MethodInfo mibaseimplementation, MethodBuilder miproxyimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, mibaseimplementation, aspects)
        {
            this.miproxyimplementation = miproxyimplementation;
        }

        #region IBindMethod Members

        public new MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miproxyimplementation; }
        }

        #endregion
    }

    internal class GeneratedProxyPropertyJoinPoint : ProxyPropertyJoinPoint, IBindMethod, IBaseCall
    {
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;


        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="mibaseimplementation"/>
        /// <param name="miimplementation"/>
        /// <param name="pibaseproperty"/>
        /// <param name="targetclass"/>
        public GeneratedProxyPropertyJoinPoint(Type targetclass, PropertyInfo pibaseproperty, MethodInfo mibaseimplementation, MethodBuilder miimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, pibaseproperty, mibaseimplementation, aspects)
        {
            this.miproxyimplementation = miimplementation;
        }

        public new MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyPropertyJoinPoint(this.declaringtype, this.pipropertyinfo, this.miimplementation, miimplementation, Aspects);
        }

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miproxyimplementation; }
        }

        #endregion
    }

    internal class GeneratedProxyEventJoinPoint : ProxyEventJoinPoint, IBindMethod, IBaseCall
    {
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;


        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="mibaseimplementation"/>
        /// <param name="miimplementation"/>
        /// <param name="targetclass"/>
        /// <param name="evinfo"></param>
        public GeneratedProxyEventJoinPoint(Type targetclass,EventInfo evinfo, MethodInfo mibaseimplementation, MethodBuilder miimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, evinfo, mibaseimplementation, aspects)
        {
            this.miproxyimplementation = miimplementation;
        }

        public new MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyEventJoinPoint(this.declaringtype, this.eieventinfo, this.miimplementation, miimplementation, Aspects);
        }

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miproxyimplementation; }
        }

        #endregion
    }


    internal class GeneratedProxyConstructorJoinPoint : ProxyConstructorJoinPoint, IBindConstructor, IBaseCtor
    {
        /// <summary>
        /// die urspruengliche Definition der Methode (NewSlot)
        /// </summary>
        ConstructorBuilder ciproxyimplementation;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="cibaseimplementation"></param>
        /// <param name="ciproxyimplementation"></param>
        /// <param name="micreateai"></param>
        /// <param name="micreateto"></param>
        /// <param name="targetclass"></param>
        public GeneratedProxyConstructorJoinPoint(Type targetclass, ConstructorInfo cibaseimplementation, ConstructorBuilder ciproxyimplementation, MethodBuilder micreateai, MethodBuilder micreateto, AspectCoverageInfo[] aspects)
            :
            base(targetclass, cibaseimplementation, aspects)
        {
            this.ciproxyimplementation = ciproxyimplementation;
            SetCreateMethods(micreateai, micreateto);
        }

        #region IBindConstructor Members

        ConstructorJoinPoint IBindConstructor.BindToType(ConstructorBuilder cbimplementation, MethodBuilder micreateai, MethodBuilder micreateto)
        {
            return new GeneratedProxyConstructorJoinPoint(this.declaringtype, this.ciconstructor, cbimplementation, micreateai, micreateto, Aspects);
        }

        #endregion

        #region IBaseCtor Members

        ConstructorInfo IBaseCtor.BaseCtor
        {
            get { return ciproxyimplementation; }
        }

        bool IBaseCtor.HasAspectParameter
        {
            get { return true; }
        }

        #endregion
    }

    internal class GeneratedProxyDestroyMethodJoinPoint : ProxyDestroyMethodJoinPoint, IBindMethod, IBaseCall
    {
        /// <summary>
        /// Die Implementierung der Methode in der Zielklasse
        /// </summary>
        MethodBuilder miproxyimplementation;

        /// <summary>
        /// Definiert einen Joinpoint für eine Interface-Methode
        /// </summary>
        /// <param name="aspects"></param>
        /// <param name="mibaseimplementation"></param>
        /// <param name="miproxyimplementation"></param>
        /// <param name="targetclass"></param>
        public GeneratedProxyDestroyMethodJoinPoint(Type targetclass, MethodInfo mibaseimplementation, MethodBuilder miproxyimplementation, AspectCoverageInfo[] aspects)
            :
            base(targetclass, mibaseimplementation, aspects)
        {
            this.miproxyimplementation = miproxyimplementation;
        }

        #region IBindMethod Members

        public new MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyMethodJoinPoint(this.declaringtype, this.miimplementation, miimplementation, Aspects);
        }

        #endregion

        #region IBaseCall Members

        MethodInfo IBaseCall.BaseCall
        {
            get { return miproxyimplementation; }
        }

        #endregion
    }

}
