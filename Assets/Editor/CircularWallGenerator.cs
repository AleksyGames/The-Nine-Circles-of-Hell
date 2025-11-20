using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CircularWallGenerator : EditorWindow
{
    // ─────────────────────────────────────────────
    // Ustawienia podstawowe
    // ─────────────────────────────────────────────

    GameObject wallPrefab;
    Transform parentTransform;
    int segmentCount = 12;

    float segmentLength = 25f; // realna długość prefab przed skalowaniem
    float gap = -1.438f;

    enum LengthAxis { X, Z }
    LengthAxis lengthAxis = LengthAxis.X;

    bool autoRadius = true;
    float radius = 50f;

    bool autoFitLengthToRadius = false; // NOWA FUNKCJA

    float yOffset = 0f;
    bool alignToTangent = true;

    bool destroyExistingChildren = false;

    string generatedPrefix = "WallSeg_";


    // ─────────────────────────────────────────────
    // OKNO
    // ─────────────────────────────────────────────

    [MenuItem("Tools/Circular Wall Generator")]
    public static void ShowWindow()
    {
        var w = GetWindow<CircularWallGenerator>("Circular Wall Generator");
        w.minSize = new Vector2(350, 300);
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab muru", EditorStyles.boldLabel);
        wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", wallPrefab, typeof(GameObject), false);

        parentTransform = (Transform)EditorGUILayout.ObjectField("Parent", parentTransform, typeof(Transform), true);

        EditorGUILayout.Space();

        segmentCount = EditorGUILayout.IntSlider("Ilość segmentów", segmentCount, 1, 512);

        lengthAxis = (LengthAxis)EditorGUILayout.EnumPopup("Długość prefab-u w osi", lengthAxis);

        segmentLength = EditorGUILayout.FloatField("Obecna długość segmentu", segmentLength);
        gap = EditorGUILayout.FloatField("Przerwa między segmentami", gap);

        EditorGUILayout.Space();

        autoRadius = EditorGUILayout.Toggle("Oblicz promień automatycznie", autoRadius);
        using (new EditorGUI.DisabledScope(autoRadius))
        {
            radius = EditorGUILayout.FloatField("Promień", radius);
        }

        autoFitLengthToRadius = EditorGUILayout.Toggle("Dopasuj segmenty do promienia", autoFitLengthToRadius);

        EditorGUILayout.Space();

        yOffset = EditorGUILayout.FloatField("Wysokość (Y offset)", yOffset);
        alignToTangent = EditorGUILayout.Toggle("Ustaw rotację do okręgu", alignToTangent);

        destroyExistingChildren = EditorGUILayout.Toggle("Usuń poprzednie segmenty", destroyExistingChildren);

        EditorGUILayout.Space();

        if (GUILayout.Button("GENERUJ MUR", GUILayout.Height(32)))
        {
            if (wallPrefab == null)
            {
                EditorUtility.DisplayDialog("Brak prefab", "Wybierz prefab muru.", "OK");
            }
            else
            {
                Generate();
            }
        }
    }

    // ─────────────────────────────────────────────
    // OBLICZENIA
    // ─────────────────────────────────────────────

    float ComputeRadius()
    {
        float totalSegment = segmentLength + gap;
        return (segmentCount * totalSegment) / (2f * Mathf.PI);
    }

    float ComputeFittedSegmentLength()
    {
        float circumference = 2f * Mathf.PI * radius;
        return (circumference / segmentCount) - gap;
    }

    // ─────────────────────────────────────────────
    // GENEROWANIE
    // ─────────────────────────────────────────────

    void Generate()
    {
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        // Create parent if absent
        if (parentTransform == null)
        {
            GameObject p = new GameObject("CircularWallParent");
            parentTransform = p.transform;
            Undo.RegisterCreatedObjectUndo(p, "Create parent");
        }

        // Remove children
        if (destroyExistingChildren)
        {
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(parentTransform.GetChild(i).gameObject);
            }
        }

        // Auto radius?
        if (autoRadius)
            radius = ComputeRadius();

        // Auto length fit?
        float finalLength = segmentLength;
        if (autoFitLengthToRadius)
            finalLength = ComputeFittedSegmentLength();

        // Direction of scaling
        Vector3 scaleAxis = Vector3.one;
        switch (lengthAxis)
        {
            case LengthAxis.X: scaleAxis = new Vector3(finalLength / segmentLength, 1f, 1f); break;
            case LengthAxis.Z: scaleAxis = new Vector3(1f, 1f, finalLength / segmentLength); break;
        }

        Vector3 center = parentTransform.position;

        // Create segments
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            float angle = t * 360f;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
            pos.y += yOffset;

            GameObject seg = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parentTransform);
            Undo.RegisterCreatedObjectUndo(seg, "Create segment");

            seg.name = $"{generatedPrefix}{i:000}";
            seg.transform.position = pos;

            // Scale
            Vector3 newScale = seg.transform.localScale; // bazowa skala prefab-a
            switch (lengthAxis)
            {
                case LengthAxis.X:
                    newScale.x *= finalLength / segmentLength;
                    break;
                case LengthAxis.Z:
                    newScale.z *= finalLength / segmentLength;
                    break;
            }
            seg.transform.localScale = newScale;

            // Rotation
            if (alignToTangent)
            {
                Vector3 radialDir = (pos - center).normalized;
                Vector3 tangent = Vector3.Cross(Vector3.up, radialDir);

                Quaternion rot = Quaternion.LookRotation(tangent, Vector3.up);

                // If prefab is long in X, rotate by -90° so X aligns with tangent
                if (lengthAxis == LengthAxis.X)
                    rot *= Quaternion.Euler(0, -90, 0);

                seg.transform.rotation = rot;
            }
            else
            {
                seg.transform.rotation = Quaternion.identity;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Undo.CollapseUndoOperations(group);
    }
}
