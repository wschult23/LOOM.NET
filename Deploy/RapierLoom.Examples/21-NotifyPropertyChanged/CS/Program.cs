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
using Loom;
using System.ComponentModel;

namespace NotifyPropertyChanged
{
    public class A
    {
        [NotifyPropertyChanged]
        public virtual int i { get; set; }
        [NotifyPropertyChanged]
        public virtual string s { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var a = Weaver.Create<A>();
            var notif = a as INotifyPropertyChanged;
            notif.PropertyChanged += new PropertyChangedEventHandler(notif_PropertyChanged);

            a.i = 42;
            a.s = a.i.ToString();

            Console.ReadKey();
        }

        static void notif_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("{0} changed property {1}", sender, e.PropertyName);
        }
    }
}
