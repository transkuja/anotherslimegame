using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CanEditMultipleObjects]
[CustomEditor(typeof(MonsterShooter))]
public class MonsterShooterEditor : Editor
{
    SerializedProperty ObeyAlphaProp;
    SerializedProperty LeaderShooterProp;
    SerializedProperty AfterLeaderDelayProp;

    SerializedProperty BulletPerRafalesProp;
    SerializedProperty ChargeDelayProp;
    SerializedProperty RafalesChargeDelayProp;
    SerializedProperty BulletDistanceProp;
    SerializedProperty BulletSpeedProp;
    public void OnEnable()
    {
        ObeyAlphaProp = serializedObject.FindProperty("obeyAlpha");
        LeaderShooterProp = serializedObject.FindProperty("leaderShooter");
        AfterLeaderDelayProp = serializedObject.FindProperty("afterLeaderDelay");

        BulletPerRafalesProp = serializedObject.FindProperty("bulletPerRafales");
        ChargeDelayProp = serializedObject.FindProperty("chargeDelay");
        RafalesChargeDelayProp = serializedObject.FindProperty("rafalesChargeDelay");
        BulletDistanceProp = serializedObject.FindProperty("bulletDistance");
        BulletSpeedProp = serializedObject.FindProperty("bulletSpeed");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!ObeyAlphaProp.hasMultipleDifferentValues)
        {
            if (!ObeyAlphaProp.boolValue)
                EditorGUILayout.HelpBox("Le monstre comme un grand", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Le monstre tire après son leader + delay", MessageType.Info);
        }

        EditorGUILayout.PropertyField(ObeyAlphaProp, new GUIContent("obeyAlpha"));

        if (!ObeyAlphaProp.hasMultipleDifferentValues)
        {
            if (ObeyAlphaProp.boolValue)
            {
                EditorGUILayout.ObjectField(LeaderShooterProp);
                EditorGUILayout.Slider(AfterLeaderDelayProp, 0, 5, new GUIContent("AfterLeaderDelayProp"));
            }
            else
            {
                EditorGUILayout.IntSlider(BulletPerRafalesProp, 0, 5, new GUIContent("BulletPerRafalesProp"));
                EditorGUILayout.Slider(ChargeDelayProp, 0, 20, new GUIContent("ChargeDelayProp"));
                EditorGUILayout.Slider(RafalesChargeDelayProp, 0, 20, new GUIContent("RafalesChargeDelayProp"));
                EditorGUILayout.Slider(BulletDistanceProp, 0, 200, new GUIContent("BulletDistanceProp"));
                EditorGUILayout.Slider(BulletSpeedProp, 0, 100, new GUIContent("BulletSpeedProp"));
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Object MultiEditing not valid if Alpha and not alpha are selected", MessageType.Info);
        }
       
        serializedObject.ApplyModifiedProperties();
    }
    //public override void OnInspectorGUI()
    //{
    //    //MonsterShooter shooter = target as MonsterShooter;
    //    //if (shooter.ObeyAlpha)
    //    //    EditorGUILayout.HelpBox("Le monstre comme un grand", MessageType.Info);
    //    //else
    //    //    EditorGUILayout.HelpBox("Le monstre tire après son leader + delay", MessageType.Info);
    //    //shooter.ObeyAlpha = EditorGUILayout.Toggle("obeyAlpha", shooter.ObeyAlpha);

    //    //if (shooter.ObeyAlpha)
    //    //{
    //    //    shooter.LeaderShooter = (MonsterShooter)EditorGUILayout.ObjectField("leaderRef", shooter.LeaderShooter, typeof(MonsterShooter), true);
    //    //    shooter.AfterLeaderDelay = EditorGUILayout.FloatField("AfterLeaderShootDelay", shooter.AfterLeaderDelay);
    //    //}
    //    //else
    //    //{
    //    //    shooter.BulletPerRafales = EditorGUILayout.IntField("bulletPerRafalles", shooter.BulletPerRafales);
    //    //    shooter.ChargeDelay = EditorGUILayout.FloatField("bulletPerRafalles", shooter.ChargeDelay);
    //    //    shooter.RafalesChargeDelay = EditorGUILayout.FloatField("bulletPerRafalles", shooter.RafalesChargeDelay);
    //    //    shooter.BulletDistance = EditorGUILayout.FloatField("bulletPerRafalles", shooter.BulletDistance);
    //    //    shooter.BulletSpeed = EditorGUILayout.FloatField("bulletPerRafalles", shooter.BulletSpeed);
    //    //}
    //}
}
