using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] private int m_splineIndex;
    [SerializeField] private float m_width;
    [SerializeField] private float m_gizmoSize = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float m_time;
    [SerializeField] private bool showGizmos = true;

    private float3 position;
    private float3 tangent;
    private float3 upVector;

    private Vector3 p1, p2;
    
    public SplineContainer SplineContainer => m_splineContainer;
    public float Width => m_width;

    private void Update()
    {
        if (showGizmos && m_splineContainer != null && m_splineContainer.Splines.Count > m_splineIndex)
        {
            m_splineContainer.Splines[m_splineIndex].Evaluate(m_time, out position, out tangent, out upVector);
            Vector3 right = Vector3.Cross((Vector3)tangent, (Vector3)upVector).normalized;
            Vector3 worldPosition = transform.TransformPoint((Vector3)position);
            p1 = worldPosition + (right * m_width);
            p2 = worldPosition + (-right * m_width);
        }
    }
    
    public void SampleSplineWidth(int splineIndex, float t, out Vector3 point1, out Vector3 point2)
    {
        if (m_splineContainer != null && m_splineContainer.Splines.Count > splineIndex)
        {
            m_splineContainer.Splines[splineIndex].Evaluate(t, out position, out tangent, out upVector);
            Vector3 right = Vector3.Cross((Vector3)tangent, (Vector3)upVector).normalized;
            Vector3 worldPosition = transform.TransformPoint((Vector3)position);
            point1 = worldPosition + (right * m_width);
            point2 = worldPosition + (-right * m_width);
        }
        else
        {
            point1 = Vector3.zero;
            point2 = Vector3.zero;
        }
    }

    
    private void OnDrawGizmos()
    {
        if (showGizmos && m_splineContainer != null && m_splineContainer.Splines.Count > m_splineIndex)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(p1, m_gizmoSize);
            Gizmos.DrawLine(p1, transform.TransformPoint((Vector3)position));
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(p2, m_gizmoSize);
            Gizmos.DrawLine(p2, transform.TransformPoint((Vector3)position));
        }
    }

    void Start()
    {
       
    }
}