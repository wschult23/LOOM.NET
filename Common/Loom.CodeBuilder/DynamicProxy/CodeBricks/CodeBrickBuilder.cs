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

using Loom.JoinPoints;
using Loom.CodeBuilder;
using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Declarations;

namespace Loom.CodeBuilder.DynamicProxy.CodeBricks
{
    /// <summary>
    /// Abstrakte Basisimplementierung für einen <see cref="CodeBrickBuilder"/>
    /// </summary>
    internal abstract class ProxyCodeBrickBuilderBase : CodeBrickBuilder
    {
        /// <summary>
        /// Die Aspektmethode
        /// </summary>
        protected MethodInfo aspectmethod;
        /// <summary>
        /// die ParametercodeBricks
        /// </summary>
        protected List<ParameterCodeBrick> paramcodebricks;


        protected ProxyCodeBrickBuilderBase(MethodInfo aspectmethod)
        {
            this.aspectmethod = aspectmethod;
            this.paramcodebricks = new List<ParameterCodeBrick>();
        }

        /// <summary>
        /// Baut aus den eingesammelten Parametercodebricks den richtigen Aspektcallcodebrick zusammen
        /// </summary>
        /// <returns></returns>
        protected CodeBrick Create()
        {
            if (aspectmethod.IsGenericMethod)
            {
                return new GenericAspectCallCodeBrick(paramcodebricks.ToArray(), aspectmethod);
            }
            else
            {
                return new AspectCallCodeBrick(paramcodebricks.ToArray(), aspectmethod);
            }
        }
        /// <summary>
        /// Zeigt auf den nächsten Parameter der Zielmethode, der durch den Builder erzeugt wird.
        /// Er muss bei den entsprechenden Add* -Methoden weitergesetzt werden
        /// </summary>
        protected IEnumerator currentparameter;


        public override void AddSimpleParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            paramcodebricks.Add(new SimpleParameterCodeBrick(((ParameterInfo)currentparameter.Current).Position + 1));
        }

        /// <summary>
        /// Zwei Fälle für Generics:
        /// 1. Zielmethode ist generic und aspectmethode auch, 
        /// 2. Zielmethode ist konkret, Aspektmethode generisch, 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public override void AddGenericParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            ParameterInfo pitargetmethod = (ParameterInfo)currentparameter.Current;
            System.Diagnostics.Debug.Assert(pi.ParameterType.IsGenericParameter || (pi.ParameterType.IsByRef && pi.ParameterType.GetElementType().IsGenericParameter));
            paramcodebricks.Add(new GenericParameterCodeBrick(pitargetmethod.Position + 1, pitargetmethod, pi));
        }
    }

    internal abstract class ProxyCodeBrickBuilder<T> : ProxyCodeBrickBuilderBase where T : ProxyMemberBuilder
    {
        /// <summary>
        /// Der Memberbuilder
        /// </summary>
        protected T meb;

        /// <summary>
        /// Baut die factory
        /// </summary>
        /// <param name="meb"></param>
        /// <param name="aspectmethod"></param>
        protected ProxyCodeBrickBuilder(T meb, MethodInfo aspectmethod) :
            base(aspectmethod)
        {
            this.meb = meb;
            this.currentparameter = meb.JoinPoint.Method.GetParameters().GetEnumerator();
        }

        public override void AddParameterContainer(ParameterInfo container)
        {
            int count = 0;
            int first = 0;
            if (currentparameter.MoveNext())
            {
                first = ((ParameterInfo)currentparameter.Current).Position;
                while (currentparameter.MoveNext()) count++;
            }

            // Array holen & Codebrick bauen;
            LocalArgArray argarray = GetContainerVariable<ProxyMethodLocalArgArray>(first, count);
            paramcodebricks.Add(new ContainerParameterCodeBrick(argarray, container.ParameterType));
        }

        public override void AddAspectParameter(ParameterInfo pi, Loom.JoinPoints.ParamType apt, Scope scope)
        {
            // Hier därfen keine Parametercodebricks vorkommen, die auf Dataslots zugreifen!
            switch (apt)
            {
                case Loom.JoinPoints.ParamType.InterwovenClass:
                    paramcodebricks.Add(DynamicTypeAspectParameterCodeBrick.GetObject());
                    break;
                case Loom.JoinPoints.ParamType.TargetClass:
                    paramcodebricks.Add(TargetClassTypeAspectParameterCodeBrick.GetObject());
                    break;
                case Loom.JoinPoints.ParamType.Target:
                    paramcodebricks.Add(TargetAspectParameterCodeBrick.GetObject());
                    break;
                default:
                    throw new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000);
            }
        }
        /// <summary>
        /// Holt einen Containerinitialisierer für ein Argumentenarray
        /// Annahme, gleiche paramcount Werte bedeuten gleiche firstparam Werte
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="paramstart"></param>
        /// <param name="paramcount">Anzahl der Elemente im Argumentenarray</param>
        /// <returns>den Container</returns>
        internal LocalArgArray GetContainerVariable<A>(int paramstart, int paramcount) where A : LocalArgArray, new()
        {
            LocalArgArray lvarg = null;
            ArgArrayDictionary argarrays = meb.DataSlots.ArgArrays;
            if (argarrays == null)
            {
                argarrays = new ArgArrayDictionary();
                meb.DataSlots.ArgArrays = argarrays;
            }
            else
            {
                argarrays.TryGetValue(paramcount, out lvarg);
            }
            if (lvarg == null)
            {
                lvarg = new A();
                lvarg.Start = paramstart;
                lvarg.Count = paramcount;
                argarrays.Add(paramcount, lvarg);
            }

            return lvarg;
        }

        /// <summary>
        /// Baut ein ParameterCodeBrick für eine JoinPointvariable
        /// </summary>
        /// <param name="pi">Der Parameter</param>
        /// <param name="scope">Der Scope für die Variable</param>
        /// <remarks>
        ///             TargetClass (static)    | TargetClass  | JoinPoint | JoinPoint (static)
        /// public      1                         1              x           x
        /// protected   1                         1              x           x
        /// private     2                         2              3           3
        /// 
        /// 
        /// 1 introduces a new (static) public/protectd member variable to the target class. 
        /// 3 introduces a new variable on the joinpoin, that will be visible for all advices on this join point
        /// 
        /// </remarks>
        /// <returns></returns>
        protected ParameterCodeBrick CreateJPVariableCodeBrick(ParameterInfo pi, Scope scope)
        {
            DataSlot dsvar = null;
            Type dstype = pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
            string name = pi.Name;
            if ((scope & Scope.Local) == Scope.Local)
            {
                name = string.Format("__{0}{1}", name, meb.JoinPoint.DeclaringMember.MetadataToken);
            }

            // Bei Public und Protected kann es sein, dass die Variable bereits existiert
            if ((scope & Scope.Private) == 0)
            {
                FieldInfo fi = meb.DeclaringBuilder.BaseType.GetField(pi.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (fi != null && !fi.IsPrivate)
                {
                    if (fi.IsPublic && (scope & Scope.Public) == 0)
                    {
                        throw new AspectWeaverException(16, Loom.Resources.Errors.ERR_0016, pi.Name, Common.Convert.ToString(pi.Member), scope.ToString());
                    }
                    if (fi.FieldType != dstype)
                    {
                        throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0020, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(fi.FieldType), Common.Convert.ToString(fi.DeclaringType));
                    }
                    dsvar = new FieldAccessDataSlot(fi);
                }
                // Override aber nicht definiert
                if ((scope & Scope.Override) != 0 && dsvar == null)
                {
                    throw new AspectWeaverException(17, Loom.Resources.Errors.ERR_0017, pi.Name, Common.Convert.ToString(pi.Member));
                }
            }

            if (dsvar == null)
            {
                JPVariableDeclaration decl;

                if ((scope & Scope.Local) != 0)
                {
                    if (meb.LocalSharedVariables.TryGetValue(pi.Name, out decl))
                    {
                        if (decl.Scope != scope)
                        {
                            throw new AspectWeaverException(18, Loom.Resources.Errors.ERR_0018, pi.Name, Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                        if (decl.DeclarationType != dstype)
                        {
                            throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0020, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(decl.FieldInfo.FieldType), Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                        dsvar = new FieldAccessDataSlot(decl);
                    }
                    else
                    {
                        decl = new JPVariableDeclaration(pi, scope);
                        meb.LocalSharedVariables.Add(pi.Name, decl);
                        dsvar = new FieldAccessDataSlot(decl);
                    }
                }
                else if ((scope & Scope.Call) == Scope.Call)
                {
                    if (meb.CallSharedVariables.TryGetValue(pi.Name, out dsvar))
                    {
                        if (dsvar.DataSlotType != dstype)
                        {
                            throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0018, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(dsvar.DataSlotType), meb.JoinPoint.ToString());
                        }
                    }
                    else
                    {
                        if ((scope & Scope.Static) == Scope.Static)
                        {
                            dsvar = new LocalObject(dstype);
                        }
                        meb.CallSharedVariables.Add(pi.Name, dsvar);
                    }
                }
                else
                {
                    if (meb.Declarations.SharedVariables.TryGetValue(pi.Name, out decl))
                    {
                        if (decl.Scope != scope)
                        {
                            throw new AspectWeaverException(18, Loom.Resources.Errors.ERR_0018, pi.Name, Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                        Type dt = decl.ParameterInfo.ParameterType;
                        if (dt.IsByRef)
                        {
                            dt = dt.GetElementType();
                        }
                        if (dt != dstype)
                        {
                            throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0020, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(dt), Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                    }
                    else
                    {
                        decl = new JPVariableDeclaration(pi, scope);
                        meb.Declarations.SharedVariables.Add(pi.Name, decl);
                    }
                    dsvar = new FieldAccessDataSlot(decl);
                }
            }

            meb.DataSlots.Variables.Add(dsvar);

            if (pi.ParameterType.IsByRef)
            {
                return new DataSlotRefParameterCodeBrick(dsvar, pi);
            }
            else
            {
                return new DataSlotParameterCodeBrick(dsvar, pi);
            }
        }
    }

    internal class ProxyAdviceMethodCodeBrickBuilder : ProxyCodeBrickBuilder<ProxyAdviceMethodBuilder>
    {
        /// <summary>
        /// Wo eingewoben
        /// </summary>
        Advice advice;

        internal ProxyAdviceMethodCodeBrickBuilder(ProxyAdviceMethodBuilder pamb, MethodInfo aspectmethod, Advice advice)
            : base(pamb, aspectmethod)
        {
            this.advice = advice;
        }

        /// <summary>
        /// Bei Methoden kann auch der Return-Wert im Context auftreten.
        /// Wird von ProxyAdviceMethodBuilder verwendet
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="apt"></param>
        /// <param name="scope"></param>
        public override void AddAspectParameter(ParameterInfo pi, Loom.JoinPoints.ParamType apt, Scope scope)
        {
            switch (apt)
            {
                case Loom.JoinPoints.ParamType.RetVal:
                    MethodInfo mi = meb.JoinPoint.MethodImplementation;
                    if (mi.ReturnType != typeof(void))
                    {
                        if (meb.DataSlots.RetVal == null)
                        {
                            meb.DataSlots.RetVal = new LocalObject(Loom.Common.TypeCheck.GetCommonType(pi.ParameterType, mi.ReturnType));
                        }
                        if (pi.ParameterType.IsByRef)
                        {
                            paramcodebricks.Add(new DataSlotRefParameterCodeBrick(meb.DataSlots.RetVal, pi));
                        }
                        else
                        {
                            paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.RetVal, pi));
                        }
                        break;
                    }
                    else
                    {
                        paramcodebricks.Add(NullParameterCodeBrick.GetObject());
                    }
                    break;
                case Loom.JoinPoints.ParamType.Context:
                    if (meb.DataSlots.Context == null)
                    {
                        meb.DataSlots.Context = new LocalContextVariable(meb.ContextClassDeclaration);
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Context, pi));
                    break;
                case Loom.JoinPoints.ParamType.Exception:
                    if (meb.DataSlots.Exception == null)
                    {
                        meb.DataSlots.Exception = new LocalObject(typeof(Exception));
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Exception, pi));
                    break;
                case Loom.JoinPoints.ParamType.Variable:
                    paramcodebricks.Add(CreateJPVariableCodeBrick(pi, scope));
                    break;
                default:
                    base.AddAspectParameter(pi, apt, scope);
                    break;
            }
        }


        /// <summary>
        /// BAut den CodeBrick und setzt ihn an richtiger Stelle in den MCB ein
        /// </summary>
        public override void CreateCodeBrick()
        {
            switch (advice)
            {
                case Advice.Before:
                    meb.AddBefore(Create());
                    break;
                case Advice.After:
                    meb.AddAfter(Create());
                    break;
                case Advice.AfterReturning:
                    meb.AddAfterReturning(Create());
                    break;
                case Advice.AfterThrowing:
                    meb.AddAfterThrowing(Create());
                    break;
                case Advice.Around:
                    meb.DefineMethodCode(Create());
                    break;
            }
        }

    }

    /// <summary>
    /// Bei Methoden, die auf Konstruktoren mappen
    /// Wird von ProxyConstructorBuilder verwendet
    /// </summary>
    internal class ProxyAdviceConstructorCodeBrickBuilder : ProxyCodeBrickBuilder<ProxyAdviceConstructorBuilder>
    {
        /// <summary>
        /// Wo eingewoben
        /// </summary>
        Advice advice;

        internal ProxyAdviceConstructorCodeBrickBuilder(ProxyAdviceConstructorBuilder pacb, MethodInfo aspectmethod, Advice advice) :
            base(pacb, aspectmethod)
        {
            this.advice = advice;
        }

        /// <summary>
        /// Baut ein ParameterCodeBrick für eine JoinPointvariable
        /// </summary>
        /// <param name="pi">Der Parameter</param>
        /// <param name="scope">Der Scope für die Variable</param>
        /// <returns></returns>
        protected new ParameterCodeBrick CreateJPVariableCodeBrick(ParameterInfo pi, Scope scope)
        {
            DataSlot dsvar = null;
            Type dstype = pi.ParameterType.GetElementType();

            // Bei Public und Protected kann es sein, dass die Variable bereits existiert
            if ((scope & Scope.Private) == 0)
            {
                FieldInfo fi = meb.DeclaringBuilder.TargetClass.GetField(pi.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (fi != null && !fi.IsPrivate)
                {
                    if (fi.IsPublic && (scope & Scope.Public) == 0)
                    {
                        throw new AspectWeaverException(16, Loom.Resources.Errors.ERR_0016, pi.Name, Common.Convert.ToString(pi.Member), scope.ToString());
                    }
                    if (fi.FieldType != dstype)
                    {
                        throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0020, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(fi.FieldType), Common.Convert.ToString(fi.DeclaringType));
                    }
                    // sonst kann der Fieldaccess nicht zugreifen!
                    if (meb.DataSlots.RetVal == null) meb.DataSlots.RetVal = new LocalObject(meb.DeclaringBuilder.TargetClass);
                    dsvar = new ExternFieldAccessDataSlot(fi);
                }
                // Override aber nicht definiert
                if ((scope & Scope.Override) != 0 && dsvar == null)
                {
                    throw new AspectWeaverException(17, Loom.Resources.Errors.ERR_0017, pi.Name, Common.Convert.ToString(pi.Member));
                }
            }

            if (dsvar == null)
            {
                JPVariableDeclaration decl;

                if ((scope & (Scope.Local | Scope.Call)) != 0)
                {
                    throw new AspectWeaverException(21, Loom.Resources.Errors.ERR_0021, pi.Name, meb.JoinPoint.ToString());
                }
                else
                {
                    if (meb.DeclaringBuilder.SharedVariables.TryGetValue(pi.Name, out decl))
                    {
                        if (decl.Scope != scope)
                        {
                            throw new AspectWeaverException(18, Loom.Resources.Errors.ERR_0018, pi.Name, Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                        if (decl.DeclarationType != dstype)
                        {
                            throw new AspectWeaverException(20, Loom.Resources.Errors.ERR_0020, pi.Name, Common.Convert.ToString(pi.Member), Common.Convert.ToString(decl.FieldInfo.FieldType), Common.Convert.ToString(decl.ParameterInfo.Member));
                        }
                    }
                    else
                    {
                        decl = new JPVariableDeclaration(pi, scope);
                        meb.DeclaringBuilder.SharedVariables.Add(pi.Name, decl);
                    }
                    // sonst kann der Fieldaccess nicht zugreifen!
                    if (meb.DataSlots.RetVal == null) meb.DataSlots.RetVal = new LocalObject(meb.DeclaringBuilder.TargetClass);
                    dsvar = new ExternFieldAccessDataSlot(decl);
                }
            }

            meb.DataSlots.Variables.Add(dsvar);

            if (pi.ParameterType.IsByRef)
            {
                return new DataSlotRefParameterCodeBrick(dsvar, pi);
            }
            else
            {
                return new DataSlotParameterCodeBrick(dsvar, pi);
            }
        }


        public override void AddAspectParameter(ParameterInfo pi, Loom.JoinPoints.ParamType apt, Scope scope)
        {
            switch (apt)
            {
                case Loom.JoinPoints.ParamType.Context:
                    if (meb.DataSlots.Context == null)
                    {
                        meb.DataSlots.Context = new LocalCreateContextVariable((CreateContextClassDeclaration)meb.ContextClassDeclaration);
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Context, pi));
                    break;
                case Loom.JoinPoints.ParamType.Target:
                case Loom.JoinPoints.ParamType.RetVal:
                    if (meb.DataSlots.RetVal == null) meb.DataSlots.RetVal = new LocalObject(meb.DeclaringBuilder.TargetClass);
                    if (pi.ParameterType.IsByRef)
                    {
                        paramcodebricks.Add(new DataSlotRefParameterCodeBrick(meb.DataSlots.RetVal, pi));
                    }
                    else
                    {
                        paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.RetVal, pi));
                    }
                    break;
                case Loom.JoinPoints.ParamType.Exception:
                    if (meb.DataSlots.Exception == null)
                    {
                        meb.DataSlots.Exception = new LocalObject(typeof(Exception));
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Exception, pi));
                    break;
                case Loom.JoinPoints.ParamType.InterwovenClass:
                    paramcodebricks.Add(ActivatorTypeAspectParameterCodeBrick.GetObject());
                    break;
                case Loom.JoinPoints.ParamType.TargetClass:
                    paramcodebricks.Add(TargetClassTypeAspectParameterCodeBrick.GetObject());
                    break;
                case Loom.JoinPoints.ParamType.Variable:
                    if ((scope & Scope.Static) != 0)
                    {
                        throw new AspectWeaverException(9000, "Variable scope must be static in Create");
                    }
                    paramcodebricks.Add(CreateJPVariableCodeBrick(pi, scope));
                    break;
                default:
                    throw new AspectWeaverException(9000, Loom.Resources.Errors.ERR_9000);
            }
        }

        public override void AddParameterContainer(ParameterInfo container)
        {
            int count = 0;
            int first = 0;
            if (currentparameter.MoveNext())
            {
                first = ((ParameterInfo)currentparameter.Current).Position;
                while (currentparameter.MoveNext()) count++;
            }

            // Array holen & Codebrick bauen Achtung
            LocalArgArray argarray = GetContainerVariable<CreateMethodLocalArgArray>(first, count);
            paramcodebricks.Add(new ContainerParameterCodeBrick(argarray, container.ParameterType));

        }

        public override void AddSimpleParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            paramcodebricks.Add(new SimpleParameterCodeBrick(((ParameterInfo)currentparameter.Current).Position + 2));
        }

        /// <summary>
        /// Zwei Fälle für Generics:
        /// 1. Zielmethode ist generic und aspectmethode auch, 
        /// 2. Zielmethode ist konkret, Aspektmethode generisch, 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public override void AddGenericParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            ParameterInfo pitargetmethod = (ParameterInfo)currentparameter.Current;
            System.Diagnostics.Debug.Assert(pi.ParameterType.IsGenericParameter);
            paramcodebricks.Add(new GenericParameterCodeBrick(pitargetmethod.Position + 2, pitargetmethod, pi));
        }

        public override void CreateCodeBrick()
        {
            CodeBrick cb;
            if (aspectmethod.IsGenericMethod)
            {
                cb = new GenericAspectCallCodeBrick(paramcodebricks.ToArray(), aspectmethod);
            }
            else
            {
                cb = new AspectCallCodeBrick(paramcodebricks.ToArray(), aspectmethod);
            }
            switch (advice)
            {
                case Advice.Before:
                    meb.AddBefore(cb);
                    break;
                case Advice.After:
                    meb.AddAfter(cb);
                    break;
                case Advice.AfterReturning:
                    meb.AddAfterReturning(cb);
                    break;
                case Advice.AfterThrowing:
                    meb.AddAfterThrowing(cb);
                    break;
                case Advice.Around:
                    meb.DefineMethodCode(cb);
                    break;
            }
        }
    }

    /// <summary>
    /// Bei Introductions
    /// wird von ProxyIntroductionBuilder verwendet
    /// </summary>
    internal class ProxyIntroducedMethodCodeBrickBuilder : ProxyCodeBrickBuilder<ProxyIntroducedMethodBuilder>
    {
        public ProxyIntroducedMethodCodeBrickBuilder(ProxyIntroducedMethodBuilder meb, MethodInfo aspectmethod)
            :
            base(meb, aspectmethod)
        {
        }

        public override void AddAspectParameter(ParameterInfo pi, Loom.JoinPoints.ParamType apt, Scope scope)
        {
            switch (apt)
            {
                case ParamType.Context:
                    if (meb.DataSlots.Context == null)
                    {
                        meb.DataSlots.Context = new StackContextValue(meb.ContextClassDeclaration);
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Context, pi));
                    break;
                case ParamType.Variable:
                    paramcodebricks.Add(CreateJPVariableCodeBrick(pi, scope));
                    break;
                default:
                    base.AddAspectParameter(pi, apt, scope);
                    break;
            }
        }

        public override void CreateCodeBrick()
        {
            meb.DefineMethodCode(Create());
        }
    }

    internal class ProxyInitializeMethodCodeBrickBuilder : ProxyCodeBrickBuilder<ProxyJPInitializerBuilder>
    {
        public ProxyInitializeMethodCodeBrickBuilder(ProxyMemberBuilder meb, MethodInfo aspectmethod, int aspectindex)
            :
            base(new ProxyJPInitializerBuilder(meb, aspectindex), aspectmethod)
        {
        }

        public override void AddGenericParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            // Parameter der Zielmethzode ermitteln
            ParameterInfo pitargetmethod = (ParameterInfo)currentparameter.Current;
            Type targettype = pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType;
            // Wenn der Zielmethodenparameter generisch ist, darf er nicht in der Methode selbst definiert sein, sondern muss aus einem
            // umschlieäenden ELement kommen, da sonst im Initializer keine Bindung erfolgen kann
            if (targettype.IsGenericParameter)
            {
                foreach (Type generic in meb.JoinPoint.Method.GetGenericArguments())
                {
                    if (generic == targettype)
                    {
                        throw new AspectWeaverException(1010, Loom.Resources.CodeBuilderErrors.ERR_1010, Common.Convert.ToString(aspectmethod), pi.ParameterType.Name, targettype.ToString(), meb.JoinPoint.ToString());
                    }
                }

            }

            if (pi.ParameterType.IsByRef)
            {
                paramcodebricks.Add(new NullRefParamCodeBrick(pitargetmethod.ParameterType.GetElementType(), targettype));
            }
            else
            {
                paramcodebricks.Add(new DefaultValueCodeBrick(pitargetmethod.ParameterType, targettype));
            }
        }

        public override void AddParameterContainer(ParameterInfo container)
        {
            int count = 0;
            int first = 0;
            if (currentparameter.MoveNext())
            {
                first = ((ParameterInfo)currentparameter.Current).Position;
                while (currentparameter.MoveNext()) count++;
            }

            // Array holen & Codebrick bauen;
            LocalArgArray argarray = GetContainerVariable<NullLocalArgArray>(first, count);
            paramcodebricks.Add(new ContainerParameterCodeBrick(argarray, container.ParameterType));
        }

        public override void AddSimpleParameter(ParameterInfo pi)
        {
            currentparameter.MoveNext();
            ParameterInfo pitargetmethod = (ParameterInfo)currentparameter.Current;
            if (pi.ParameterType.IsByRef)
            {
                paramcodebricks.Add(new NullRefParamCodeBrick(pitargetmethod.ParameterType.GetElementType(), pi.ParameterType.GetElementType()));
            }
            else if (pi.ParameterType.IsValueType)
            {
                paramcodebricks.Add(new DefaultValueCodeBrick(pitargetmethod.ParameterType, pi.ParameterType));
            }
            else
            {
                paramcodebricks.Add(NullParameterCodeBrick.GetObject());
            }
        }

        public override void AddAspectParameter(ParameterInfo pi, ParamType apt, Scope scope)
        {
            switch (apt)
            {
                case ParamType.Context:
                    if (meb.DataSlots.Context == null)
                    {
                        meb.DataSlots.Context = new StackContextValue(meb.ContextClassDeclaration);
                    }
                    paramcodebricks.Add(new DataSlotParameterCodeBrick(meb.DataSlots.Context, pi));
                    break;
                case Loom.JoinPoints.ParamType.Variable:
                    paramcodebricks.Add(CreateJPVariableCodeBrick(pi, scope));
                    break;
                default:
                    base.AddAspectParameter(pi, apt, scope);
                    break;
            }
        }

        public override void CreateCodeBrick()
        {
            meb.AddCodeBrick(Create());
            ((ProxyTypeBuilder)meb.DeclaringBuilder).AddInitializer(meb);
        }
    }
}
