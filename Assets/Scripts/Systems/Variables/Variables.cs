using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Primitives

[CreateAssetMenu(fileName = "Bool", menuName = "Variable/Primitive/Bool")]
public class BoolVariable : Variable<bool> { }

[CreateAssetMenu(fileName = "Float", menuName = "Variable/Primitive/Float")]
public class FloatVariable : Variable<float>
{
    public static FloatVariable operator ++(FloatVariable a)
    {
        a.Value++;
        return a;
    }
    public static FloatVariable operator --(FloatVariable a)
    {
        a.Value--;
        return a;
    }
}

[CreateAssetMenu(fileName = "Int", menuName = "Variable/Primitive/Int")]
public class IntVariable : Variable<int>
{
    public static IntVariable operator ++(IntVariable a)
    {
        a.Value++;
        return a;
    }
    public static IntVariable operator --(IntVariable a)
    {
        a.Value--;
        return a;
    }
}

[CreateAssetMenu(fileName = "String", menuName = "Variable/Primitive/String")]
public class StringVariable : Variable<string> { }

[CreateAssetMenu(fileName = "Ulong", menuName = "Variable/Primitive/Ulong")]
public class UlongVariable : Variable<ulong>
{
    public static UlongVariable operator ++(UlongVariable a)
    {
        a.Value++;
        return a;
    }
    public static UlongVariable operator --(UlongVariable a)
    {
        a.Value--;
        return a;
    }
}

#endregion

#region Classes

[CreateAssetMenu(fileName = "HoL Player", menuName = "Variable/Classes/HoL Player")]
public class HoLPlayerVariable : Variable<HoLPlayer> { }

[CreateAssetMenu(fileName = "Mission", menuName = "Variable/Classes/Mission")]
public class MissionVariable : Variable<Mission> { }

[CreateAssetMenu(fileName = "Mission", menuName = "Variable/Classes/Role")]
public class RoleVariable : Variable<Role> { }

#endregion

#region Enums

[CreateAssetMenu(fileName = "Mission Result", menuName = "Variable/Enums/MissionResult")]
public class MissionResultVariable : Variable<MissionResult> { }

[CreateAssetMenu(fileName = "Team", menuName = "Variable/Enums/Team")]
public class TeamVariable : Variable<Team> { }

#endregion