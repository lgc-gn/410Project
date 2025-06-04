using UnityEngine;

/*

Defines variables for character classes

*/

[CreateAssetMenu(fileName = "NewClassData", menuName = "RPG Objects/Class Data")]
public class ClassData : ScriptableObject
{

    public string className, classDescription, subclassName, classAction, resourceType;
    public Color resourceColor;
    public Sprite classIcon;


    // add skills further down the road
    // can add things like HP scaling per level and such
    // class specific resources (rage, sorcery points, ki, etc)

}