using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;

public class EnumUtils {

    //Create new enum from arrays
    public static System.Enum CreateEnumFromArrays(List<string> list, int index)
    {

        System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
        AssemblyName aName = new AssemblyName("Enum");
        AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
        ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);
        EnumBuilder enumerator = mb.DefineEnum("Enum", TypeAttributes.Public, typeof(int));

        int i = 0;
        enumerator.DefineLiteral("None", i); //Here = enum{ None }

        foreach (string names in list)
        {
            i++;
            enumerator.DefineLiteral(names, i);
        }

        //Here = enum { None, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

        System.Type finished = enumerator.CreateType();

        return (System.Enum)System.Enum.ToObject(finished, index);
    }

}

public enum CollectableType { StrengthEvolution1, PlatformistEvolution1, AgileEvolution1, GhostEvolution1, Points, Rune, Money, Size };

