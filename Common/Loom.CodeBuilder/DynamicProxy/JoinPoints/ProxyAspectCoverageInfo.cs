// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.AspectProperties;
using Loom.AspectModel;
using System.Reflection;
using Loom.Configuration;
using System.Reflection.Emit;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using System.Collections;
using Loom.Common;

namespace Loom.CodeBuilder.DynamicProxy
{

    interface IBindToAspectFieldDeclaration
    {
        FieldInfo AspectFieldInfo { get; }
        void Bind(FieldInfo fi);
    }

    abstract class ProxyAspectCoverageInfo : AspectCoverageInfo, IBindToAspectFieldDeclaration
    {
        protected FieldInfo field;

        public FieldInfo AspectFieldInfo { get { return field; } }

        public void Bind(FieldInfo field)
        {
            this.field = field;
        }

        protected ProxyAspectCoverageInfo(AspectClass aspectclass) :
            base(aspectclass)
        {
        }

        public virtual AspectField CreateAspectField()
        {
            switch (AspectClass.AspectInstantiation)
            {
                case Per.AspectClass:
                    return new ContainerAspectField((IAspectInstantiationCodeBrick)this, (IBindToAspectFieldDeclaration)AspectClass);
                case Per.Annotation:
                    return new ContainerAspectField((IAspectInstantiationCodeBrick)this, this);
                case Per.Class:
                case Per.ClassAndAnnotation:
                    return new StaticAspectField((IAspectInstantiationCodeBrick)this);
                case Per.Instance:
                case Per.InstanceAndAnnotation:
                    return new InstanceAspectField((IAspectInstantiationCodeBrick)this);
                default:
                    System.Diagnostics.Debug.Fail("undefined");
                    break;
            }
            return null;
        }

        public abstract void CheckEmptyConstructor();

        protected void ThrowError1014()
        {
            throw new AspectWeaverException(1014, Resources.CodeBuilderErrors.ERR_1014, this.AspectClass.ToString(), String.Format("{0}({1}.{2})",typeof(CreateAspectAttribute).Name, typeof(Per).Name, this.AspectClass.AspectInstantiation.ToString()));
        }
    }

    internal class CustomAttributeAspectCoverageInfo : ProxyAspectCoverageInfo, IAspectInstantiationCodeBrick
    {
        private CustomAttributeData cad;

        public CustomAttributeData CustomAttributeData
        {
            get { return cad; }
        }

        public CustomAttributeAspectCoverageInfo(AspectClass ac, CustomAttributeData cad) :
            base(ac)
        {
            this.cad = cad;
        }

        public override void CheckEmptyConstructor()
        {
            if(cad.ConstructorArguments.Count!=0 || cad.NamedArguments.Count!=0)
            {
                ThrowError1014();
            }
        }

        public void Emit(ILGenerator ilgen)
        {
            foreach (var carg in cad.ConstructorArguments)
            {
                if (carg.ArgumentType.IsArray)
                {
                    var arr = (ICollection<CustomAttributeTypedArgument>)carg.Value;
                    var elemtype=carg.ArgumentType.GetElementType();
                    CommonCode.EmitLdConstArray(ilgen, arr, elemtype, arg => arg.ArgumentType, arg => arg.Value);
                }
                else
                {
                    CommonCode.EmitLdConst(ilgen, carg.ArgumentType, carg.Value);
                }
            }

            ilgen.Emit(OpCodes.Newobj, cad.Constructor);
            foreach (var narg in cad.NamedArguments)
            {
                ilgen.Emit(OpCodes.Dup);
                CommonCode.EmitLdConst(ilgen, narg.TypedValue.ArgumentType, narg.TypedValue.Value);
                switch (narg.MemberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        ilgen.Emit(OpCodes.Stfld, (FieldInfo)narg.MemberInfo);
                        break;
                    case MemberTypes.Property:
                        MethodInfo mi = ((PropertyInfo)narg.MemberInfo).GetSetMethod();
                        System.Diagnostics.Debug.Assert(mi != null);
                        ilgen.Emit(OpCodes.Call, mi);
                        break;
                    default:
                        System.Diagnostics.Debug.Fail("invalid member");
                        break;
                }
            }
        }

        

    }

    internal class ConfigAspectCoverageInfo : ProxyAspectCoverageInfo, IAspectInstantiationCodeBrick
    {
        private AspectConfig config;
        private TypedValue[] ctorparams;
        private Type[] ctortypes;

        public ConfigAspectCoverageInfo(AspectClass ac, AspectConfig config) :
            base(ac)
        {
            this.config = config;
            this.ctorparams = config.parameter == null ? new TypedValue[0] : config.parameter.Select(pc => GetParameterValue(pc, ac.AspectType.Assembly, typeof(string))).ToArray();
            this.ctortypes = this.ctorparams.Select(pval => pval.Type).ToArray();
        }

       
        public void Emit(ILGenerator ilgen)
        {
            foreach (var param in ctorparams)
            {
                if (param.Type.IsArray)
                {
                    CommonCode.EmitLdConstArray(ilgen, (ICollection<TypedValue>)param.Value, param.Type.GetElementType(), tv => tv.Type, tv => tv.Value);
                }
                else
                {
                    CommonCode.EmitLdConst(ilgen, param.Type, param.Value);
                }
            }
            ConstructorInfo ci = AspectClass.AspectType.GetConstructor(ctortypes);
            if (ci == null)
            {
                throw new AspectWeaverException(1015, Resources.CodeBuilderErrors.ERR_1015, AspectClass.AspectType.FullName, config.parameter.Length.ToString(), Common.Convert.ToString(ctortypes));
            }
            ilgen.Emit(OpCodes.Newobj, ci);
        }

        /// <summary>
        /// Übersetzt die Konfigurationsparameter in echte Parameterwerte
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static TypedValue GetParameterValue(ParameterWithValueConfig pval, Assembly assemblyscope, Type defaulttype)
        {
            string typestr;
            if (pval.type == null)
            {
                if (defaulttype == null)
                {
                    throw new ArgumentException(Resources.CodeBuilderErrors.ERRMSG_NoTypeParam);
                }
                typestr = defaulttype.FullName;
            }
            else
            {
                typestr = pval.type.Trim();
            }
            Type elemtype = null;
            switch (typestr)
            {
                case "bool[]": elemtype = typeof(bool); break;
                case "byte[]": elemtype = typeof(byte); break;
                case "sbyte[]": elemtype = typeof(sbyte); break;
                case "char[]": elemtype = typeof(char); break;
                case "double[]": elemtype = typeof(double); break;
                case "int[]": elemtype = typeof(int); break;
                case "uint[]": elemtype = typeof(uint); break;
                case "short[]": elemtype = typeof(short); break;
                case "ushort[]": elemtype = typeof(ushort); break;
                case "long[]": elemtype = typeof(long); break;
                case "ulong[]": elemtype = typeof(ulong); break;
                case "string[]": elemtype = typeof(string); break;
                case "System.Boolean":
                case "bool": return new TypedValue(bool.Parse(pval.value));
                case "System.Byte":
                case "byte": return new TypedValue(byte.Parse(pval.value));
                case "System.SByte":
                case "sbyte": return new TypedValue(sbyte.Parse(pval.value));
                case "System.Char":
                case "char": return new TypedValue(char.Parse(pval.value));
                case "System.Double":
                case "double": return new TypedValue(double.Parse(pval.value));
                case "System.Single":
                case "float": return new TypedValue(float.Parse(pval.value));
                case "System.Int16":
                case "short": return new TypedValue(short.Parse(pval.value));
                case "System.UInt16":
                case "ushort": return new TypedValue(ushort.Parse(pval.value));
                case "System.Int32":
                case "int": return new TypedValue(int.Parse(pval.value));
                case "System.UInt32":
                case "uint": return new TypedValue(uint.Parse(pval.value));
                case "System.Int64":
                case "long": return new TypedValue(long.Parse(pval.value));
                case "System.UInt64":
                case "ulong": return new TypedValue(ulong.Parse(pval.value));
                case "System.String":
                case "string": return _T<string>.Value(pval.value);
                case "System.Type": return new RTTypedValue(typeof(Type),Common.Resolver.ResolveType(pval.value, assemblyscope)); ;
                default:
                    {
                        Type type = Common.Resolver.ResolveType(typestr, assemblyscope);
                        if (type == null)
                        {
                            throw new ArgumentException(string.Format(Loom.Common.Properties.Resources.ERRMSG_TypeNotFound, typestr));
                        }
                        if (type.IsEnum)
                        {
                            return new TypedValue(Enum.Parse(type, pval.value));
                        }
                        else if (type.IsArray)
                        {
                            elemtype = type;
                        }
                        else if (type.IsPrimitive)
                        {
                            return new RTTypedValue(type, Common.Conversion.ToObject(type, pval.value));
                        }
                    }
                    break;
            }
            if (elemtype!=null)
            {
                if (pval.value != null)
                {
                    throw new ArgumentException(Resources.CodeBuilderErrors.ERRMSG_ParamValue);
                }
                var arrtype = elemtype.MakeArrayType();
             
                if (pval.arrayelements != null)
                {
                    var array=pval.arrayelements.Select(pc => GetParameterValue(pc, assemblyscope, elemtype)).ToArray();
                    foreach (var tpval in array)
                    {
                        if(tpval.Type!=elemtype && !tpval.Type.IsSubclassOf(elemtype))
                        {
                            throw new ArgumentException(string.Format(Resources.CodeBuilderErrors.ERRMSG_IncompatibleTypes,tpval.Type.FullName,arrtype.FullName));
                        }
                    }
                    return new RTTypedValue(arrtype, array);
                }
                else
                {
                    return new RTTypedValue(arrtype, new TypedValue[0]);
                }
            }
            throw new ArgumentException(string.Format(Loom.Common.Properties.Resources.ERRMSG_InvalidType, typestr));
        }

        public override void CheckEmptyConstructor()
        {
            if (config.parameter.Length != 0)
            {
                ThrowError1014();
            }
        }
    }
}
