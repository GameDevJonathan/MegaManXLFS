using UnityEngine;
using UnityEditor;


public class SetTags : MonoBehaviour
{

    static SerializedObject  tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
    SerializedProperty tagsProp = tagManager.FindProperty("tags");
    [SerializeField] public string s; 
    // Adding a Tag
    

    public void AddTags()
    {
        // First check if it is not already present
        bool found = false;

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(s)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
            tagManager.ApplyModifiedPropertiesWithoutUndo();
        }

    }



}
