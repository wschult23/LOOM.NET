// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder;


namespace Loom.AspectModel
{
    /// <summary>
    /// Return values for the match function
    /// </summary>
    internal enum AspectMemberMatchResult { Match, NoMatch, NotYet }

    /// <summary>
    /// a delagate declaring the Match function
    /// </summary>
    /// <param name="cpweaver"></param>
    /// <param name="jp"></param>
    /// <param name="pass"></param>
    /// <param name="interwovenmethods"></param>
    /// <returns></returns>
    internal delegate AspectMemberMatchResult IsJoinPointMatchDelegate(IList<IJoinPointSelector> cpweaver, JoinPoint jp, int pass, AspectMemberCollection interwovenmethods);

    /// <summary>
    /// This meta class is representing an aspect method
    /// </summary>
    internal abstract class AspectMember
    {
        /// <summary>
        /// the aspect method's MethodInfo
        /// </summary>
        protected MethodInfo aspectmethod;

        /// <summary>
        /// the constructor
        /// </summary>
        /// <param name="aspectmethod"></param>
        protected AspectMember(MethodInfo aspectmethod)
        {
            this.aspectmethod = aspectmethod;
            object[] attrs = aspectmethod.GetCustomAttributes(typeof(MatchModeAttribute), true);

            Match matchmode;
            if (attrs.Length == 1)
            {
                matchmode = ((MatchModeAttribute)attrs[0]).Mode;
            }
            else
            {
                matchmode = Match.One;
            }
            if (matchmode == Match.One)
            {
                IsJoinPointMatch = new IsJoinPointMatchDelegate(IsJoinPointMatchOne);
            }
            else
            {
                IsJoinPointMatch = new IsJoinPointMatchDelegate(IsJoinPointMatchAll);
            }
        }


        /// <summary>
        /// returns the aspect method
        /// </summary>
        public MethodInfo Method
        {
            get { return aspectmethod; }
        }


        /// <summary>
        /// Creates a list of all joinpoint attributes
        /// </summary>
        /// <returns>a list of joinpoint attributes</returns>
        protected IList<IJoinPointSelector> CreateConnectionPointMatchList()
        {
            List<IJoinPointSelector> cpweaver = new List<IJoinPointSelector>();

            foreach (Attribute attr in aspectmethod.GetCustomAttributes(false))
            {
                IJoinPointSelector cpmatch = attr as IJoinPointSelector;
                if (cpmatch != null) cpweaver.Add(cpmatch);
            }

            return cpweaver;
        }

        /// <summary>
        /// Delegate Method, that checks if THE method will match a join point
        /// </summary>
        protected readonly IsJoinPointMatchDelegate IsJoinPointMatch;

        /// <summary>
        /// A implementation of a joinpoijnt check method
        /// </summary>
        /// <param name="cpweaver"></param>
        /// <param name="jp"></param>
        /// <param name="interwovenmethods"></param>
        /// <param name="pass"></param>
        /// <returns>NoMatch, if one selector returns NoMatch or no selector returns Match, NotYet if one selector returns NotYet, Match otherwise</returns>
        private AspectMemberMatchResult IsJoinPointMatchOne(IList<IJoinPointSelector> cpweaver, JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            AspectMemberMatchResult amRes = AspectMemberMatchResult.NoMatch;
            foreach (IJoinPointSelector cpm in cpweaver)
            {
                switch (cpm.IsMatch(jp, pass, interwovenmethods))
                {
                    case JoinPointSelectorResult.NoMatch:
                        return AspectMemberMatchResult.NoMatch;
                    case JoinPointSelectorResult.Match:
                        amRes = AspectMemberMatchResult.Match;
                        break;
                    case JoinPointSelectorResult.NotYet:
                        return AspectMemberMatchResult.NotYet;
                }
            }
            return amRes;
        }

        /// <summary>
        /// A implementation of a joinpoijnt check method
        /// </summary>
        /// <param name="cpweaver"></param>
        /// <param name="jp"></param>
        /// <param name="interwovenmethods"></param>
        /// <param name="pass"></param>
        /// <returns>NoMatch, if one selector returns NoMatch or Undecided or no selector returns Match, NotYet if one selector returns NotYet, Match otherwise</returns>
        private AspectMemberMatchResult IsJoinPointMatchAll(IList<IJoinPointSelector> cpweaver, JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            AspectMemberMatchResult amRes = AspectMemberMatchResult.NoMatch;
            foreach (IJoinPointSelector cpm in cpweaver)
            {
                switch (cpm.IsMatch(jp, pass, interwovenmethods))
                {
                    case JoinPointSelectorResult.NoMatch:
                    case JoinPointSelectorResult.Undecided:
                        return AspectMemberMatchResult.NoMatch;
                    case JoinPointSelectorResult.Match:
                        amRes = AspectMemberMatchResult.Match;
                        break;
                    case JoinPointSelectorResult.NotYet:
                        return AspectMemberMatchResult.NotYet;
                }
            }
            return amRes;
        }

        /// <summary>
        /// Checks parameter compatibility
        /// </summary>
        /// <param name="cpparameter"></param>
        /// <param name="cpresult"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool IsParameterMatch(IList<IParameterMatch> cpparameter, IResultTypeMatch cpresult, MethodBase target)
        {
            // Generics?
            Dictionary<Type, Type> genericargs = null;
            if (aspectmethod.IsGenericMethodDefinition)
            {
                genericargs = new Dictionary<Type, Type>();
                foreach (Type t in aspectmethod.GetGenericArguments())
                {
                    genericargs.Add(t, null);
                }
            }

            // Ceck if identical paremeters
            ParameterInfo[] param = target.GetParameters();
            IEnumerator piarr = param.GetEnumerator();

            for (IEnumerator<IParameterMatch> epm = cpparameter.GetEnumerator(); epm.MoveNext(); )
            {
                IParameterMatch pm = epm.Current;

                if (!piarr.MoveNext())
                {
                    // no more parameters, but it could be that it matches anyway
                    do
                    {
                        if (!epm.Current.IsMatch(null)) return false;
                    } while (epm.MoveNext());

                    goto checkretval;
                }

                // Check if generic methodnamepattern matches
                IGenericParameter igen = pm as IGenericParameter;
                if (igen != null)
                {
                    ParameterInfo pi = (ParameterInfo)piarr.Current;
                    Type tbound = genericargs[igen.GenericParameterType];
                    if (tbound == null)
                    {
                        genericargs[igen.GenericParameterType] = pi.ParameterType;
                    }
                    else if (tbound != pi.ParameterType)
                    {
                        return false;
                    }
                }

                if (!pm.IsMatch(piarr)) return false;
            }

            // are there further unchecked parameters?
            if (piarr.MoveNext()) return false;


        checkretval:
            // return type
            if (cpresult != null)
            {
                Type restype;
                MethodInfo mi = target as MethodInfo;
                if (mi != null)
                {
                    restype = mi.ReturnType;
                }
                else
                {
                    // check constructor
                    restype = target.DeclaringType;
                }

                if (!cpresult.IsMatch(restype)) return false;
            }

            return true;
        }

        /// <summary>
        /// checks if abstract MethodAccessException should be interwoven
        /// </summary>
        /// <returns></returns>
        public abstract AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods);

        /// <summary>
        /// Interweaves a method by calling <see cref="MethodCodeBuilder.DefineAspectCall" />.
        /// </summary>
        /// <param name="meb"></param>
        public abstract void Interweave(MethodCodeBuilder meb);

        /// <summary>
        /// Generates a code brick to call an aspect method
        /// </summary>
        /// <param name="mcb">the method builde to be used to call the aspect method</param>
        /// <param name="cpparameter">the parameter match objects to convert the aspect call</param>
        /// <param name="advice">the kind of advice</param>
        /// <returns></returns>
        protected void CreateAspectCallCodeBrick(MethodCodeBuilder mcb, IList<IParameterMatch> cpparameter, Advice advice)
        {
            // define parameter conversions

            // Create CodeBricks , 
            // Important: count foreward, the factory is not stateless and the code bricks have to be created in the correct order
            CodeBrickBuilder cbb = mcb.DefineAspectCall(Method, advice);

            foreach (IParameterMatch cpp in cpparameter)
            {
                cpp.AddParameter(cbb);
            }

            cbb.CreateCodeBrick();
        }

        /// <summary>
        /// Creates the match object for the return value
        /// </summary>
        /// <returns></returns>
        protected IResultTypeMatch CreateResultTypeMatch(IList<IParameterMatch> cpparams, Advice advice, bool bSimpleMatch)
        {
            IResultTypeMatch restm=null;
            switch (advice)
            {
                case Advice.After:
                case Advice.AfterReturning:
                case Advice.Before:
                case Advice.Initialize:
                    if (aspectmethod.ReturnType != typeof(void))
                    {
                        throw new AspectWeaverException(11, Resources.Errors.ERR_0011, Common.Convert.ToString(aspectmethod));
                    }
                    break;
                case Advice.AfterThrowing:
                case Advice.Around:
                    if (bSimpleMatch)
                    {
                        if (aspectmethod.ReturnType.IsGenericParameter)
                        {
                            restm=new SimpleGenericResultType(aspectmethod);
                        }
                        else
                        {
                            restm=new SimpleResultType(aspectmethod);
                        }
                    }
                    else
                    {
                        if (!aspectmethod.ReturnType.IsGenericParameter)
                        {
                            throw new AspectWeaverException(12, Resources.Errors.ERR_0012, Common.Convert.ToString(aspectmethod), Common.Convert.ToString(aspectmethod.ReturnType), Common.Convert.ToString(advice));
                        }
                        restm = new AdvancedGenericResultType(aspectmethod);
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }


            // Is there a joinpoint parameter that needs to be assigned to the result value?
            foreach (object obj in cpparams)
            {
                var rtpara = obj as IResultTypeMatch;
                if (restm == null)
                {
                    restm = rtpara;
                }
                else
                {
                    if (rtpara != null && restm.ResultType!=rtpara.ResultType)
                    {
                        throw new AspectWeaverException(22, Resources.Errors.ERR_0022, Common.Convert.ToString(aspectmethod));
                    }
                }
            }
            return restm;
        }


        /// <summary>
        /// Creates a parameter match object for each parameter
        /// if a parameter has a proprirate attribute, then the match object is the attribute itself
        /// </summary>
        /// <param name="bSimpleMatch"></param>
        /// <returns></returns>
        protected IList<IParameterMatch> CreateParameterMatchList(bool bSimpleMatch)
        {
            ParameterInfo[] piarr = aspectmethod.GetParameters();
            IParameterMatch[] cpparameter = new IParameterMatch[piarr.Length];

            for (int iPos = 0; iPos < piarr.Length; iPos++)
            {
                ParameterInfo pi = piarr[iPos];
                // ICreateParameterMatch-Attribute on this parameter?
                IParameterMatch cpparam = null;

                object[] cpsig = pi.GetCustomAttributes(false);
                foreach (Attribute cpattr in cpsig)
                {
                    ICreateParameterMatch newpam = cpattr as ICreateParameterMatch;
                    if (newpam != null)
                    {
                        if (cpparam != null)
                        {
                            AspectWeaver.WriteWeaverMessage(WeaverMessages.MessageType.Warning, 2, Loom.Resources.Warnings.WARN_0202, pi.Name, Common.Convert.ToString(aspectmethod), newpam.ToString());
                        }
                        else
                        {
                            cpparam = newpam.Create(pi);
                        }
                    }
                    else if (cpattr is ParamArrayAttribute && !bSimpleMatch)
                    {
                        cpparam = new WildcardParameter(pi);
                    }
                }
                // simple types
                if (cpparam == null)
                {
                    Type t = pi.ParameterType;
                    if (t.IsByRef)
                    {
                        t = t.GetElementType();
                    }
                    if (t.IsGenericParameter)
                    {
                        if (bSimpleMatch)
                        {
                            cpparam = new GenericParameter(pi);
                        }
                        else
                        {
                            cpparam = new GenericWildcardParameter(pi);
                        }
                    }
                    else
                    {
                        cpparam = new SimpleParameter(pi);
                    }
                }
                cpparameter[iPos] = cpparam;
            }

            return cpparameter;
        }

        /// <summary>
        /// Returns the string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Common.Convert.ToString(aspectmethod);
        }

    }
}
