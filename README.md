# Scriptable-Enum-For-Unity
A Scriptable Object based id drawer which mimics enum behaviour prodiving flexibility for extending enum values without code recompilation.

Features:
 - Declare like normal enum, The values will be drawn by the values set in scriptable object.
 - No need for **code recompilation** or **loss of serialization** due to changes in enum values.
 - Drawer based workflow for easy assignment of values.
 - **ScriptableEnumsContainer** for access to all possible enumerations.
 - Cached HashCode generated based on **values** instead of actual object reference for optimum performance during HashSet and DictionaryLookups.
 - Also implements EquailtyComparers based on values instead of reference for easy comparision accross instances.

Limitations:
 - ScriptableObjectContainer required for iterating all the enumeration values.
 - CentralIdContainer requied for iterating all ScriptableObjectContainer values.

Screenshot:
![alt text]( https://i.ibb.co/stCqYLw/Scriptable-Enum.png )
