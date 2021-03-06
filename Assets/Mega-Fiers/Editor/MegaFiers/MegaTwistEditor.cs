
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(MegaTwist))]
public class MegaTwistEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Twist Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\twist_help.png"); }

	public override bool Inspector()
	{
		MegaTwist mod = (MegaTwist)target;

#if !UNITY_5 && !UNITY_2017 && !UNITY_2018 && !UNITY_2019 && !UNITY_2020 && !UNITY_2021
		EditorGUIUtility.LookLikeControls();
#endif

		mod.angle = EditorGUILayout.FloatField("Angle", mod.angle);
		mod.Bias = EditorGUILayout.FloatField("Bias", mod.Bias);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);
		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}