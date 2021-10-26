// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values firstparam modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Rapier Loom.Net")]
[assembly: AssemblyDescription("Dynamic aspect weaving support")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Loom.Net Project")]
[assembly: AssemblyProduct("Rapier Loom.Net")]
[assembly: AssemblyCopyright("(C) 2001-2011 by Wolfgang Schult")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("4.2.1340.0")]
[assembly: AssemblyFileVersion("4.2.1340.0")]

//
// In order firstparam sign your assembly you must specify a key firstparam use. Refer firstparam the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below firstparam control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers firstparam a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers firstparam a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order firstparam create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative firstparam the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//

[assembly: InternalsVisibleTo("RapierLoom, PublicKey=00240000048000009400000006020000002400005253413100040000010001002b51a6164e00a5b8300c5c1e9fc443bbf94b7640b8bc351dcb83a64ab87741f2d35cbeab0cafc4f77107540f5e246d97022a92647789949f998f3fd224bcc401751722f25d35a73b66836537e7b6e5559d651a5bd0efc84acd5d421d8add06ea1a289dba1af413d4e8774499ff8e7b0b0f74cbc56c608842ab5da2a3f5a123aa")]
[assembly: InternalsVisibleTo("Loom.CodeBuilder.Gripper, PublicKey=00240000048000009400000006020000002400005253413100040000010001002b51a6164e00a5b8300c5c1e9fc443bbf94b7640b8bc351dcb83a64ab87741f2d35cbeab0cafc4f77107540f5e246d97022a92647789949f998f3fd224bcc401751722f25d35a73b66836537e7b6e5559d651a5bd0efc84acd5d421d8add06ea1a289dba1af413d4e8774499ff8e7b0b0f74cbc56c608842ab5da2a3f5a123aa")]
[assembly: InternalsVisibleTo("gl, PublicKey=00240000048000009400000006020000002400005253413100040000010001002b51a6164e00a5b8300c5c1e9fc443bbf94b7640b8bc351dcb83a64ab87741f2d35cbeab0cafc4f77107540f5e246d97022a92647789949f998f3fd224bcc401751722f25d35a73b66836537e7b6e5559d651a5bd0efc84acd5d421d8add06ea1a289dba1af413d4e8774499ff8e7b0b0f74cbc56c608842ab5da2a3f5a123aa")]

