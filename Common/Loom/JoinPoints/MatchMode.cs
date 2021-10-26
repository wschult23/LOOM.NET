// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Loom.JoinPoints
{
    /// <summary>
    /// Defines the matching behavior on an aspect-method.
    /// </summary>
    public enum Match
    {
        /// <summary>
        /// At least one "IncludeXXX" and no "ExcludeXXX"  attribute matches. (This is the default behavior)
        /// </summary>
        One,
        /// <summary>
        /// All "IncludeXXX" attributes and no "ExcludeXXX" attribute matches.
        /// </summary>
        All
    }

    /// <summary>
    /// Use this attribute to define the matching behavior on an aspect-method.
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MatchModeAttribute : Attribute
    {
        Match match;

        internal Match Mode
        {
            get { return match; }
        }

        /// <summary>
        /// Defines the matching behavior on an aspect-method.
        /// </summary>
        /// <param name="match">defines the <see cref="Match"/></param>
        public MatchModeAttribute(Match match)
        {
            this.match = match;
        }
    }
}
