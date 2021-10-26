// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

using Loom.AspectModel;

namespace Loom.JoinPoints
{
    /// <summary>
    /// Specifies how the aspect weaver should interweave aspect class and target class.
    /// </summary>
    public enum Advice
    {
        /// <summary>
        /// <para>The aspect method will be invoked before the join point is executed.</para>
        /// </summary>
        Before,
        /// <summary>
        /// <para>The aspect method will be invoked after the join-point has been execuded.</para>
        /// </summary>
        After,
        /// <summary>
        /// <para>The aspect method will be invoked after the execution of the join-point has been returned.</para>
        /// </summary>
        AfterReturning,
        /// <summary>
        /// <para>The aspect method will be invoked after the join-point has been thrown an exception.</para>
        /// </summary>
        AfterThrowing,
        /// <summary>
        /// <para>The join-point will not be executed - but can be called from inside the aspect method (see <see cref="Context"/>).</para>
        /// </summary>
        Around,
        /// <summary>
        /// <para>Every time an interwoven object of the specified target class is created by the weaver the aspect method will be invoked for every matching join-point</para>
        /// <para>Only <see cref="JoinPointParameterAttribute">join-point parameters</see> are valid when the aspect method is called. All other parameters are set to their default value.</para>
        /// </summary>
        Initialize
    }

    /// <summary>
    /// Specifies the type of the interweaving target.
    /// </summary>
    /// <seealso cref="ErrorAttribute"/>
    /// <seealso cref="WarningAttribute"/>
    public enum TargetTypes
    {
        /// <summary>
        /// Specifies that the target is the object creation.
        /// </summary>
        Create,
        /// <summary>
        /// Specifies that the target is a method call.
        /// </summary>
        Call,
        /// <summary>
        /// Specifies that the target is an access to a property.
        /// </summary>
        Access,
        /// <summary>
        /// Specifies that the target is the destructor.
        /// </summary>
        Finalize
    }

    /// <summary>
    /// The base class for all pointcut attributes.
    /// </summary>
    public abstract class AspectMethodAttribute : Attribute
    {
        /// <summary>
        /// Advice enum
        /// </summary>
        protected Advice invoke;

        /// <summary>
        /// Mit dieser Methode definiert das Attribut, welche Auswirkungen es auf den Verwebungsprozess hat.
        /// </summary>
        /// <param name="aspectclass"></param>
        /// <param name="aspectmember"></param>
        internal abstract void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmember);

        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="invokeOrder">describes at which point an interwoven method will be invoked</param>
        /// <seealso cref="Advice"/>
        public AspectMethodAttribute(Advice invokeOrder)
        {
            this.invoke = invokeOrder;
        }

        /// <summary>
        /// Returns the invoke order of this join-point.
        /// </summary>
        /// <seealso cref="Advice"/>
        internal Advice InvokeOrder
        {
            get
            {
                return invoke;
            }
        }

        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="ei">Internal use.</param>
        /// <returns>Internal use.</returns>
        protected static IEnumerable<MethodInfo> GetEventMethods(EventInfo ei)
        {
            MethodInfo mi;
            mi = ei.GetAddMethod();
            if (mi != null) yield return mi;
            mi = ei.GetRaiseMethod();
            if (mi != null) yield return mi;
            mi = ei.GetRemoveMethod();
            if (mi != null) yield return mi;
            foreach (MethodInfo miother in ei.GetOtherMethods())
            {
                yield return mi;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object. 
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("[{0}({1}.{2})]", GetType().Name.Replace("Attribute", ""), invoke.GetType().Name, invoke);
            return sb.ToString();
        }
    }

  

   



   
}
