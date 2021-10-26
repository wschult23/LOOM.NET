// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Loom.JoinPoints;

namespace Loom.Runtime
{
    /// <summary>
    /// This class is for internal purposes only! Do never create an instance of this class directly from your code!
    /// </summary>
    [Serializable]
    public class MethodContext : Context, ISerializable
    {
        /// <summary>
        /// 
        /// </summary>
        protected object instance;
        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public MethodContext(object instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// This property is for internal purposes only! Do never access this property directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override object Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override Type[] GetParameterTypes()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// This property is for internal purposes only! Do never access this property directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override MethodBase CurrentMethod
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// This property is for internal purposes only! Do never access this property directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override Type ReturnType
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override Type TargetClass
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override Type InterwovenClass
        {
            get { return instance.GetType(); }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!<br />
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override object ReCall(params object[] args)
        {
            return ReCallOn(instance, args);
        }

        #region ISerializable Members

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!<br />
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(MethodContextSurrogate));
            info.AddValue("instance", instance);
            info.AddValue("tag", tag);
            info.AddValue("ctxclass", GetType().Name);
        }

        #endregion
    }

    [Serializable]
    internal class MethodContextSurrogate : IObjectReference, ISerializable
    {
        private MethodContext theObject;

        protected MethodContextSurrogate(SerializationInfo info, StreamingContext context)
        {
            object instance = info.GetValue("instance", typeof(object));
            string ctxclassname = info.GetString("ctxclass");
            Type ctxclass = null;
            for (Type insttype = instance.GetType(); insttype != null; insttype = insttype.BaseType)
            {
                ctxclass = insttype.GetNestedType(ctxclassname);
                if (ctxclass != null) goto found;
            }
            throw new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000);

        found:
            this.theObject = (MethodContext)Activator.CreateInstance(ctxclass, instance);
            this.theObject.Tag = info.GetValue("tag", typeof(object));
        }

        #region IObjectReference Members

        public object GetRealObject(StreamingContext context)
        {
            return theObject;
        }

        #endregion


        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    /// <summary>
    /// Represents a context within a advice method
    /// </summary>
    [Serializable]
    public class CallContext : MethodContext
    {
        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        public CallContext(object instance)
            :
            base(instance)
        {
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <returns>true</returns>
        public override bool HasJoinPoint()
        {
            return true;
        }
    }

    /// <summary>
    /// Represents a context within a advice method
    /// </summary>
    [Serializable]
    public abstract class IntroductionContext : MethodContext
    {
        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public IntroductionContext(object instance) :
            base(instance)
        {
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override object Invoke(object[] args)
        {
            throw new MissingTargetMethodException(this, args);
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <seealso cref="Loom.JoinPoints.Context"/>
        public override object InvokeOn(object target, object[] args)
        {
            throw new MissingTargetMethodException(this, args);
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// See <see cref="Loom.JoinPoints.Context"/> for documentation.
        /// </summary>
        /// <returns>false</returns>
        public override bool HasJoinPoint()
        {
            return false;
        }
    }

}
