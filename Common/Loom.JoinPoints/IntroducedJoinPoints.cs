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


namespace Loom.JoinPoints.Implementation
{
    internal class IntroducedMethodJoinPoint : MethodJoinPoint, IInterfaceJoinPoint, IBindInterfaceMethod, IBindMethod
    {
        public IntroducedMethodJoinPoint(MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(miinterface.DeclaringType, null, miinterface, aspects)
        {
        }

        public override MethodBase Method
        {
            get
            {
                return miinterface;
            }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(miinterface);
        }

        #region IInterfaceJoinPoint Members

        public Type Interface
        {
            get { return miinterface.DeclaringType; }
        }

        public MethodInfoJoinPoint BindToImplementation(MethodInfo miimplementation, AspectCoverageInfo[] aspects)
        {
            return new ProxyInterfaceMethodJoinPoint(miimplementation.DeclaringType, miimplementation, miinterface, aspects);
        }

        #endregion

        #region IBindMethod Members

        public MethodInfoJoinPoint BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceMethodJoinPoint(this.declaringtype, this.miinterface, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion
    }

    internal class IntroducedPropertyJoinPoint : PropertyJoinPoint, IInterfaceJoinPoint, IBindInterfaceMethod, IBindMethod
    {
        public IntroducedPropertyJoinPoint(PropertyInfo pipropertyinfo, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(miinterface.DeclaringType, pipropertyinfo, null, miinterface, aspects)
        {
        }

        public override MethodBase Method
        {
            get
            {
                return miinterface;
            }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(miinterface);
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return miinterface; }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfacePropertyJoinPoint(this.declaringtype, this.pipropertyinfo, this.miinterface, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion

        #region IInterfaceJoinPoint Members

        Type IInterfaceJoinPoint.Interface
        {
            get { return miinterface.DeclaringType; }
        }

        public MethodInfoJoinPoint BindToImplementation(MethodInfo miimplementation, AspectCoverageInfo[] aspects)
        {
            // pipropertyinfo ist das aus dem Interface, koennte problematisch sein
            return new ProxyInterfacePropertyJoinPoint(miimplementation.DeclaringType, pipropertyinfo, miimplementation, miinterface, aspects);
        }



        #endregion
    }


    internal class IntroducedEventJoinPoint : EventJoinPoint, IInterfaceJoinPoint, IBindInterfaceMethod, IBindMethod
    {
        public IntroducedEventJoinPoint(EventInfo eventinfo, MethodInfo miinterface, AspectCoverageInfo[] aspects)
            :
            base(miinterface.DeclaringType, eventinfo, null, miinterface, aspects)
        {
        }

        public override MethodBase Method
        {
            get
            {
                return miinterface;
            }
        }

        public override string ToString()
        {
            return Common.Convert.ToString(miinterface);
        }

        #region IManageJoinPoint Members

        public MethodBase KeyMethod
        {
            get { return miinterface; }
        }

        #endregion

        #region IBindMethod Members

        MethodInfoJoinPoint IBindMethod.BindToType(MethodBuilder miimplementation, MethodBuilder mibasecallimplementation)
        {
            return new GeneratedProxyInterfaceEventJoinPoint(this.declaringtype, this.eieventinfo, this.miinterface, miimplementation, this.miinterface, mibasecallimplementation, Aspects);
        }

        #endregion

        #region IInterfaceJoinPoint Members

        Type IInterfaceJoinPoint.Interface
        {
            get { return miinterface.DeclaringType; }
        }

        public MethodInfoJoinPoint BindToImplementation(MethodInfo miimplementation, AspectCoverageInfo[] aspects)
        {
            // eieventinfo ist das aus dem Interface, evtl. problematisch
            return new ProxyInterfaceEventJoinPoint(miimplementation.DeclaringType, eieventinfo, miimplementation, miinterface, aspects);
        }



        #endregion
    }
}
