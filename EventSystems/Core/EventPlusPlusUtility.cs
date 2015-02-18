using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public static class EventPlusPlusUtility
{

    public static bool IsAcceptedType(Type t)
    {
        return t == typeof(float) || t == typeof(int) || t == typeof(Color) || t == typeof(Vector3);
    }

    public static List<MemberInfo> BuildList(Component c)
    {
        List<Type> types;
        return BuildList(c, out types);
    }

    public static List<MemberInfo> BuildList(Component c, out List<Type> types)
    {
        types = new List<Type>();
        FieldInfo[] fieldInfo = c.GetType().GetFields();
        PropertyInfo[] propInfo = c.GetType().GetProperties();

        List<MemberInfo> memberInfo = new List<MemberInfo>();

        for (int i = 0; i < fieldInfo.Length; i++)
        {
            if (IsAcceptedType(fieldInfo[i].FieldType))
            {
                memberInfo.Add(fieldInfo[i]);
                types.Add(fieldInfo[i].FieldType);
            }
        }

        for (int i = 0; i < propInfo.Length; i++)
        {
            if (IsAcceptedType(propInfo[i].PropertyType))
            {
                memberInfo.Add(propInfo[i]);
                types.Add(propInfo[i].PropertyType);

            }
        }
        return memberInfo;
    }

}
