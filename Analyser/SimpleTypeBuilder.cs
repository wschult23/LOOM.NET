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
using System.Linq;

using Loom.CodeBuilder;
using Loom.AspectModel;
using Loom.JoinPoints.Implementation;
using Example;

namespace Loom.Analyzer
{
    internal class SimpleTypeBuilder:InterwovenTypeBuilder, IAcceptJoinPoint
    {
        Type targetclass;
        StringBuilder sb;
        string indent;
        JoinPointCollection jpcol;

        InterfaceJoinPointCollection[] introduced;

        public SimpleTypeBuilder(Type targetclass)
        {
            this.targetclass = targetclass;
            jpcol = JoinPointCollection.GetJoinPoints(targetclass);
        }

        public override bool Interweave(Aspect aspect)
        {
            
            return true;
        }

        public override bool Interweave()
        {
            sb = new StringBuilder();

            sb.Append(targetclass.FullName);
            sb.Append("\n{\n");
            foreach(IJoinPointEnumerator jpenum in jpcol.GetAspects())
            {
                Accept(jpenum);
            }
            sb.Append("}\n\n");

            Console.WriteLine();
            Console.WriteLine(sb.ToString());
            return true;
        }




  
        public void Accept(IJoinPointEnumerator enumerator)
        {
            introduced = null;

            sb.Append("  ");
            sb.Append(enumerator.Aspects.First().AspectClass.AspectType.Name);
            sb.Append("[");
            sb.Append(enumerator.Aspects.Count);
            sb.Append("]");
            sb.Append("\n  {\n");
            indent = "    ";
            enumerator.EnumJoinPoints(this, jpcol);
            sb.Append("  }\n\n");

            //int intrpos = jpcol.InterfaceJoinPoints.Length;
            //int ifcpos;
            //InterfaceJoinPoints[] ifcjpts = new InterfaceJoinPoints[intrpos + introduced.Length];
            //for (ifcpos = 0; ifcpos < intrpos; ifcpos++)
            //{
            //    InterfaceJoinPoints oldifcjp=jpcol.InterfaceJoinPoints[ifcpos];
            //    InterfaceJoinPoints ifcjp = new InterfaceJoinPoints();
                
            //    ifcjp.bContainsVirtualMethods = false;
            //    ifcjp.Interface = oldifcjp.Interface;
            //    ifcjp.JoinPoints = new MethodInfoJoinPoint[oldifcjp.JoinPoints.Length];

            //    for (int jppos = 0; jppos < ifcjp.JoinPoints.Length; jppos++)
            //    {
            //        ifcjp.JoinPoints[jppos] = (MethodInfoJoinPoint)oldifcjp.JoinPoints[jppos].Clone(targetclass);
            //    }

            //    ifcjpts[ifcpos] = ifcjp;
            //}
            //for (; ifcpos < ifcjpts.Length; ifcpos++)
            //{
            //    InterfaceJoinPoints ifcjp = new InterfaceJoinPoints();
            //    InterfaceJoinPointCollection ifccol = introduced[ifcpos-intrpos];

            //    ifcjp.bContainsVirtualMethods = false;
            //    ifcjp.Interface = ifccol;
            //    ifcjp.JoinPoints = new MethodInfoJoinPoint[ifccol.JoinPoints.Length];

            //    for (int jppos = 0; jppos < ifcjp.JoinPoints.Length; jppos++)
            //    {
            //        ifcjp.JoinPoints[jppos] = ((IBindInterfaceMethod)ifccol.JoinPoints[jppos]).BindToImplementation(ifccol.JoinPoints[jppos].MethodInterface,new AspectAttribute[0]);
            //    }

            //    ifcjpts[ifcpos] = ifcjp;
            //}
        }


        private void PrintWeavingPlan(JoinPoint jp, AspectMemberCollection matches)
        {
            sb.Append(indent);
            sb.Append(jp.ToString());
            sb.Append("\n");
            sb.Append(indent);
            sb.Append("{\n");
            MethodCodeBuilder mcb = new SimpleMethodBuilder(sb,indent+"  ");
            mcb.Interweave(matches);
            sb.Append(indent);
            sb.Append("}\n\n");

            mcb.Emit();
        }

        #region IAcceptJoinPoint Members

        void IAcceptJoinPoint.AcceptVTableJoinPoint(int aspectindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            PrintWeavingPlan(jp, matches);
        }

       
        void IAcceptJoinPoint.AcceptCtorJoinPoint(int aspectindex, int jpindex, ConstructorJoinPoint jp, AspectMemberCollection matches)
        {
            PrintWeavingPlan(jp, matches);
        }



        void IAcceptJoinPoint.AcceptInterfaceJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            PrintInterfaceWeavingPlan(jpcol.InterfaceJoinPoints[ifcindex].Interface, jpindex, jp, matches);
        }

        private void PrintInterfaceWeavingPlan(InterfaceJoinPointCollection ifjp, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            if (jpindex == 0)
            {
                sb.Append(indent);
                sb.Append(jp.MethodInterface.DeclaringType.FullName);
                sb.Append("\n");
                sb.Append(indent);
                sb.Append("{\n");
                indent += "  ";
            }

            PrintWeavingPlan(jp, matches);

            if (jpindex == ifjp.JoinPoints.Length - 1)
            {
                indent = indent.Substring(2);
                sb.Append(indent);
                sb.Append("}\n\n");
            }
        }

        void IAcceptJoinPoint.AcceptIntroductionJoinPoint(int aspectindex, int ifcindex, int jpindex, MethodInfoJoinPoint jp, AspectMemberCollection matches)
        {
            PrintInterfaceWeavingPlan(introduced[ifcindex], jpindex, jp, matches);
        }

        void IAcceptJoinPoint.AcceptInterface(ICollection<InterfaceJoinPointCollection> ifcol)
        {
            introduced = new InterfaceJoinPointCollection[ifcol.Count];
            ifcol.CopyTo(introduced, 0);
        }

        #endregion
    }
}
