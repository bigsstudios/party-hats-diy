
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(MegaStretch))]
public class MegaStretchEditor : MegaModifierEditor
{
	public override string GetHelpString()	{ return "Stretch Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\stretch_help.png"); }

	public override bool Inspector()
	{
		MegaStretch mod = (MegaStretch)target;

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018 && !UNITY_2019 && !UNITY_2020 && !UNITY_2021
		EditorGUIUtility.LookLikeControls();
#endif
		mod.amount = EditorGUILayout.FloatField("Amount", mod.amount);
		mod.amplify = EditorGUILayout.FloatField("Amplify", mod.amplify);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);

		//mod.useheightaxis = EditorGUILayout.BeginToggleGroup("Use Height Axis", mod.useheightaxis);
		//mod.axis1 = (MegaAxis)EditorGUILayout.EnumPopup("Height Axis", mod.axis1);
		//EditorGUILayout.EndToggleGroup();

		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}