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

using Loom.CodeBuilder;
using Loom.AspectModel;

namespace Loom.JoinPoints.Implementation
{
    internal enum JoinPointSelectorResult { Match, NoMatch, Undecided, NotYet };

    /// <summary>
    /// You have to implement this interface if you want to specify programatically whether a method
    /// should become interwoven or not
    /// </summary>
    internal interface IJoinPointSelector
    {
        /// <summary>
        /// Specifies whether or not a aspect method should become interwoven with a target class method.
        /// </summary>
        /// <remarks>
        /// If the parameter signature of the target class method does not match with the aspect method,
        /// this method will not be called.
        /// </remarks>
        /// <param name="jp">the <see cref="JoinPoint"/></param>
        /// <param name="pass">the actual match pass</param>
        /// <param name="interwovenmethods">interwoven methods so far</param>
        /// <returns>
        /// <see cref="JoinPointSelectorResult.Match"/> if the join point matches
        /// <see cref="JoinPointSelectorResult.NoMatch"/> if the join point doesn't match
        /// <see cref="JoinPointSelectorResult.Undecided"/> if the selector can't decide if the join point matches
        /// <see cref="JoinPointSelectorResult.NotYet"/> if the selector need's another pass to decide if the join point matches
        /// </returns>
        JoinPointSelectorResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods);
    }

    /// <summary>
    /// You have to implement this interface if you want to specify programatically whether a method
    /// should become interwoven or not
    /// </summary>
    internal interface IJoinPointParameterMatch
    {
        /// <summary>
        /// Specifies whether or not a aspect method with a certain parameter should become interwoven with a target class method.
        /// </summary>
        /// <param name="aspectparameter"></param>
        /// <param name="methodparameter"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        JoinPointSelectorResult IsMatch(ParameterInfo aspectparameter, ParameterInfo methodparameter, WeavingContext ctx);
    }

    /// <summary>
    /// You have to implement this interface if you want to specify programatically whether a method
    /// should become interwoven or not
    /// </summary>
    internal interface IJoinPointReturnTypeMatch
    {
        /// <summary>
        /// Specifies whether or not a aspect method with a certain parameter should become interwoven with a target class method.
        /// </summary>
        /// <param name="aspectreturntype"></param>
        /// <param name="methodreturntype"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        JoinPointSelectorResult IsMatch(Type aspectreturntype, Type methodreturntype, WeavingContext ctx);
    }

}
