using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObserverModule))]
public class ObserverModuleEditor : Editor
{
    private SerializedProperty _clueWeightsProp;
    private SerializedProperty _hearingRangeProp;
    private SerializedProperty _sightFurstrumProp;
    private SerializedProperty m_targetMask;
    private SerializedProperty m_obstructionMask;
    private SerializedProperty _inputOrgan;
    
    private void OnEnable()
    {
        _clueWeightsProp = serializedObject.FindProperty("_clueWeights");
        _hearingRangeProp = serializedObject.FindProperty("_hearingRange");
        _sightFurstrumProp = serializedObject.FindProperty("_sightFurstrum");
        m_targetMask = serializedObject.FindProperty("m_targetMask");
        m_obstructionMask = serializedObject.FindProperty("m_obstructionMask");
        _inputOrgan = serializedObject.FindProperty("_inputOrgan");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_inputOrgan);
        EditorGUILayout.PropertyField(m_obstructionMask);
        EditorGUILayout.PropertyField(m_targetMask);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sensing Parameters", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Clue Weights", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_clueWeightsProp);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.LabelField("Hearing", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_hearingRangeProp);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.LabelField("Sight", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_sightFurstrumProp);
            EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnSceneGUI()
    {
        ObserverModule observer = (ObserverModule) target;
        Transform inputOrganTransform = observer._inputOrgan.transform;
        Handles.color = Color.white;
        Handles.DrawWireArc(inputOrganTransform.position, Vector3.up, Vector3.forward, 360, observer._sightFurstrum.SightRange);

        Vector3 viewAngle01 = DirectionFromAngle(inputOrganTransform.eulerAngles.y, -observer._sightFurstrum.ConeAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(inputOrganTransform.eulerAngles.y, observer._sightFurstrum.ConeAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(inputOrganTransform.position, inputOrganTransform.position + viewAngle01 * observer._sightFurstrum.SightRange);
        Handles.DrawLine(inputOrganTransform.position, inputOrganTransform.position + viewAngle02 * observer._sightFurstrum.SightRange);

        if (observer._canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(inputOrganTransform.position, observer.m_playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
