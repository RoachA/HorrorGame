using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObserverModule))]
public class ObserverModuleEditor : Editor
{
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
