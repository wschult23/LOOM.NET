// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using Loom;
using Loom.ContextSharp;


/// This example shows, how the Context Oriented Programming paradigm can be implemented with LOOM.NET
/// To learn more about COP, please visit http://www.swa.hpi.uni-potsdam.de/cop/
namespace ContextSharpExample
{
    /// <summary>
    /// Contains attributes related to a person. This class is declared as layered
    /// </summary>
    [Layered]
    public partial class Person
    {
        private string name;
        private string address;
        private Employer employer;

        public Person(string name, string address, Employer employer)
        {
            this.name = name;
            this.address = address;
            this.employer = employer;
        }

        public override string ToString()
        {
            return "Name: " + this.name;
        }
    }

    /// <summary>
    /// Contains attributes related to an employer. This class is declared as layered
    /// </summary>
    [Layered]
    public partial class Employer
    {
        private string name;
        private string address;

        public Employer(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        public override string ToString()
        {
            return "Name: " + this.name;
        }
    }

    // The Main Programm
    class Program
    {
        static void Main(string[] args)
        {
            Employer employer = new Employer("HPI", "Potsdam");
            Person person = new Person("John Doe", "Berlin", employer);

            Console.WriteLine(person); // Name: John Doe

            using (With<Address>.Do)
            {
                using (With<Employment>.Do)
                {
                    Console.WriteLine(person); // Name: John Doe; Address: Berlin; Employer: Name: HPI; Address: Potsdam
                    using (Without<Address>.Do)
                    {
                        Console.WriteLine(person); // Name: John Doe; Employer: Name: HPI
                    }
                    Console.WriteLine(person); // Name: John Doe; Address: Berlin; Employer: Name: HPI; Address: Potsdam
                }
                Console.WriteLine(person); // Name:John Doe; Address: Berlin
            }
            Console.WriteLine(person); // Name: John Doe
 
            Console.ReadKey();
        }
    }
}