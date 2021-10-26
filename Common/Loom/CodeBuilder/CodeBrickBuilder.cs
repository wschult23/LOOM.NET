// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;

using Loom.JoinPoints;

namespace Loom.CodeBuilder
{

    /// <summary>
    /// Erzeugt einen CodeBrick der den Aufruf einer Aspektmethode Repräsentiert.
    /// Der CodebrickBuilder wird in durch die Methoden DefineAspectCall und DefineInitialisation erzeugt
    /// Anschlieäend müssen die Parameter für den Aufruf in derselben Reihenfolge aufgezählt werden, wie in der Aspektmethode definiert sind.
    /// Zum Abschluss muss die Methode <see cref="CreateCodeBrick"/> aufgrufen werden.
    /// </summary>
    internal abstract class CodeBrickBuilder
    {
        /// <summary>
        /// Erzeugt intern einen ParameterCodeBrick, der einen Parameter der Zielmethode in einen Parameter der Aspectmethode übergibt
        /// Dieser Codebrick ist ein Singleton und benötigt keinen Initialisierungscode
        /// </summary>
        /// <param name="pi">Der Parameter in der Aspektmethode</param>
        /// <returns></returns>
        public abstract void AddSimpleParameter(ParameterInfo pi);
        /// <summary>
        /// Erzeugt intern einen ParameterCodeBrick, der einen generischen Parameter der Zielmethode konkretisiert und an einen Parameter der Aspectmethode übergibt
        /// Dieser Codebrick ist ein Singleton und benötigt keinen Initialisierungscode
        /// </summary>
        /// <param name="pi">der Parameter der Aspektmethode</param>
        /// <returns></returns>
        public abstract void AddGenericParameter(ParameterInfo pi);
        /// <summary>
        /// Erzeugt intern einen ParameterCodeBrick, der einen Aspectparameter an die Aspectmethode übergibt
        /// Dieser CodeBrick benötigt Initialisierungscode für bestimmte Aspectparametertypen 
        /// Instanzen werden in der Factory gespeichert
        /// </summary>
        /// <param name="pi">Der Parameter der Aspektmethode</param>
        /// <param name="apt">Der Typ (ParamInfo ist hier nicht nätig)</param>
        /// <param name="scope">Der Scope einer JP Variablen</param>
        /// <returns></returns>
        public abstract void AddAspectParameter(ParameterInfo pi, ParamType apt, Scope scope);
        /// <summary>
        /// Erzeugt intern einen ParameterCodeBrick, der die Parameter der Zielmethode in einem Array der Aspectmethode übergibt
        /// Dieser CodeBrick benötigt Initialisierungscode für jedes (Containertyp,position) Tupel
        /// Instanzen werden in der Factory gespeichert
        /// </summary>
        /// <param name="container">Der Container der Aspektmethode</param>
        /// <returns></returns>
        public abstract void AddParameterContainer(ParameterInfo container);
        /// <summary>
        /// Schliesst die Erzeugung des Codebricks ab und rendert alle Teile zusammen
        /// </summary>
        public abstract void CreateCodeBrick();
    }
}
