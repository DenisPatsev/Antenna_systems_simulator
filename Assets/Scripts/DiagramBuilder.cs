using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;

public class DiagramBuilder : MonoBehaviour
{
    public enum PhiAngle
    {
        Phi_0 = 0,
        Phi_5 = 5,
        Phi_10 = 10,
        Phi_15 = 15,
        Phi_20 = 20,
        Phi_25 = 25,
        Phi_30 = 30,
        Phi_35 = 35,
        Phi_40 = 40,
        Phi_45 = 45,
        Phi_50 = 50,
        Phi_55 = 55,
        Phi_60 = 60,
        Phi_65 = 65,
        Phi_70 = 70,
        Phi_75 = 75,
        Phi_80 = 80,
        Phi_85 = 85,
        Phi_90 = 90,
        Phi_95 = 95,
        Phi_100 = 100,
        Phi_105 = 105,
        Phi_110 = 110,
        Phi_115 = 115,
        Phi_120 = 120,
        Phi_125 = 125,
        Phi_130 = 130,
        Phi_135 = 135,
        Phi_140 = 140,
        Phi_145 = 145,
        Phi_150 = 150,
        Phi_155 = 155,
        Phi_160 = 160,
        Phi_165 = 165,
        Phi_170 = 170,
        Phi_175 = 175,
        Phi_180 = 180
    }

    [Header("File Settings")]
    public TextAsset dataFile; // Изменено с string fileName на TextAsset
    public float scale = 0.1f;
    
    [Header("Visualization Settings")]
    public Material meshMaterial;
    public bool autoGenerateOnStart = true;
    
    [Header("Angle Selection")]
    public PhiAngle selectedPhiAngle = PhiAngle.Phi_0;
    public bool showAllAngles = true;
    public bool isManualChoice = true;
    
    [Header("Axis Settings")]
    public bool showAxes = true;
    public float axisLength = 2f;
    public float axisThickness = 0.02f;
    public Color xAxisColor = Color.red;
    public Color yAxisColor = Color.green;
    public Color zAxisColor = Color.blue;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private List<FarfieldPoint> parsedPoints;
    private Dictionary<float, List<FarfieldPoint>> phiAngleData;
    private bool isDataLoaded = false;
    
    private GameObject xAxis, yAxis, zAxis;
    private List<MeshRenderer> _labels;
    
    public List<MeshRenderer> Labels => _labels;
    
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.RightArrow))
    //     {
    //         selectedPhiAngle += 5;
    //         GenerateMeshFromCurrentSelection();
    //     }
    // }

    // void OnValidate()
    // {
    //     if (Application.isPlaying && isDataLoaded)
    //     {
    //         GenerateMeshFromCurrentSelection();
    //     }
    // }

    public void InitializeData(TextAsset file, float diagramScale)
    {
        dataFile = file;
        scale = diagramScale;
        LoadDataAndGenerateMesh();
    }

    [ContextMenu("Load Data and Generate Mesh")]
    public void LoadDataAndGenerateMesh()
    {
        _labels = new List<MeshRenderer>();
        InitializeComponents();
        
        if (dataFile == null)
        {
            Debug.LogError("Data file is not assigned. Please assign a TextAsset to the dataFile field.");
            return;
        }

        try
        {
            ParseFarfieldData(dataFile.text);
            GenerateMeshFromCurrentSelection();
            
            if (showAxes)
            {
                CreateAxes();
            }
            
            Debug.Log($"Mesh generated successfully from file: {dataFile.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating mesh: {e.Message}");
        }
    }

    [ContextMenu("Generate Mesh for Current Angle")]
    public void GenerateMeshFromCurrentSelection()
    {
        if (!isDataLoaded)
        {
            Debug.LogWarning("Data not loaded. Please load data first.");
            return;
        }

        if (showAllAngles)
        {
            CreateMeshFromAllAngles();
        }
        else
        {
            float targetPhi;
            
                if(!isManualChoice)
                    targetPhi = PlayerSessionData.CurrentAntennaRotationZ;
                else
                    targetPhi = (float)selectedPhiAngle;
                
                if (phiAngleData.ContainsKey(targetPhi))
            {
                CreateMeshFromSingleAngle(phiAngleData[targetPhi], targetPhi);
            }
            else
            {
                Debug.LogWarning($"No data found for angle: {targetPhi}°");
            }
        }

    }

    [ContextMenu("Toggle Show All Angles")]
    public void ToggleShowAllAngles()
    {
        showAllAngles = !showAllAngles;
        if (isDataLoaded)
        {
            GenerateMeshFromCurrentSelection();
        }
    }

    [ContextMenu("Toggle Axes")]
    public void ToggleAxes()
    {
        showAxes = !showAxes;
        if (xAxis != null) xAxis.SetActive(showAxes);
        if (yAxis != null) yAxis.SetActive(showAxes);
        if (zAxis != null) zAxis.SetActive(showAxes);
    }

    private void InitializeComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (meshMaterial != null)
            meshRenderer.material = meshMaterial;
        else
            meshRenderer.material = new Material(Shader.Find("Standard"));

        mesh = new Mesh();
        mesh.name = "Farfield Diagram";
        meshFilter.mesh = mesh;
    }

    private void ParseFarfieldData(string fileContent)
    {
        parsedPoints = new List<FarfieldPoint>();
        phiAngleData = new Dictionary<float, List<FarfieldPoint>>();
        
        string[] lines = fileContent.Split('\n');

        bool foundDataHeader = false;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            if (line.Contains(">> Total #phi samples, total #theta samples"))
            {
                foundDataHeader = true;
                continue;
            }

            if (foundDataHeader && !string.IsNullOrEmpty(line) && !line.StartsWith("//"))
            {
                string cleanLine = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ");
                string[] parts = cleanLine.Split(' ');
                
                if (parts.Length >= 6)
                {
                    try
                    {
                        FarfieldPoint point = new FarfieldPoint();
                        point.phi = float.Parse(parts[0], CultureInfo.InvariantCulture);
                        point.theta = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        point.reTheta = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        point.imTheta = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        point.rePhi = float.Parse(parts[4], CultureInfo.InvariantCulture);
                        point.imPhi = float.Parse(parts[5], CultureInfo.InvariantCulture);
                        
                        parsedPoints.Add(point);

                        if (!phiAngleData.ContainsKey(point.phi))
                            phiAngleData[point.phi] = new List<FarfieldPoint>();
                        phiAngleData[point.phi].Add(point);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to parse line {i}: {line}. Error: {e.Message}");
                    }
                }
            }
        }

        isDataLoaded = true;
        Debug.Log($"Parsed {parsedPoints.Count} points from {phiAngleData.Count} different phi angles");
    }

    private void CreateMeshFromAllAngles()
    {
        if (parsedPoints == null || parsedPoints.Count == 0)
        {
            Debug.LogError("No parsed points available to create mesh");
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        float maxMagnitude = FindMaxMagnitude(parsedPoints);

        Dictionary<float, List<FarfieldPoint>> phiGroups = new Dictionary<float, List<FarfieldPoint>>();
        foreach (var point in parsedPoints)
        {
            if (!phiGroups.ContainsKey(point.phi))
                phiGroups[point.phi] = new List<FarfieldPoint>();
            phiGroups[point.phi].Add(point);
        }

        List<float> sortedPhis = new List<float>(phiGroups.Keys);
        sortedPhis.Sort();

        foreach (float phi in sortedPhis)
        {
            var thetaPoints = phiGroups[phi];
            thetaPoints.Sort((a, b) => a.theta.CompareTo(b.theta));

            foreach (var point in thetaPoints)
            {
                Vector3 sphericalPos = SphericalToCartesian(point.theta, point.phi);
                float magnitude = CalculateFieldMagnitude(point);
                Vector3 vertexPos = sphericalPos * magnitude * scale;
                
                vertices.Add(vertexPos);
                normals.Add(sphericalPos.normalized);
                uv.Add(new Vector2(point.phi / 360f, point.theta / 180f));
                colors.Add(CalculateColor(magnitude, maxMagnitude));
            }
        }

        int phiCount = sortedPhis.Count;
        if (phiCount < 2)
        {
            Debug.LogWarning("Not enough phi values to create triangles");
            return;
        }

        int thetaCount = phiGroups[sortedPhis[0]].Count;
        if (thetaCount < 2)
        {
            Debug.LogWarning("Not enough theta values to create triangles");
            return;
        }

        for (int phiIndex = 0; phiIndex < phiCount - 1; phiIndex++)
        {
            for (int thetaIndex = 0; thetaIndex < thetaCount - 1; thetaIndex++)
            {
                int current = phiIndex * thetaCount + thetaIndex;
                int next = (phiIndex + 1) * thetaCount + thetaIndex;

                triangles.Add(current);
                triangles.Add(next + 1);
                triangles.Add(current + 1);

                triangles.Add(current);
                triangles.Add(next);
                triangles.Add(next + 1);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        
        mesh.RecalculateBounds();

        Debug.Log($"Full mesh created with {vertices.Count} vertices and {triangles.Count / 3} triangles");
    }

    private void CreateMeshFromSingleAngle(List<FarfieldPoint> points, float phiAngle)
    {
        if (points == null || points.Count == 0)
        {
            Debug.LogError($"No data points for angle {phiAngle}°");
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        float maxMagnitude = FindMaxMagnitude(points);

        var sortedPoints = points.OrderBy(p => p.theta).ToList();

        foreach (var point in sortedPoints)
        {
            Vector3 sphericalPos = SphericalToCartesian(point.theta, point.phi);
            float magnitude = CalculateFieldMagnitude(point);
            Vector3 vertexPos = sphericalPos * magnitude * scale;
            
            vertices.Add(vertexPos);
            normals.Add(sphericalPos.normalized);
            uv.Add(new Vector2(point.phi / 360f, point.theta / 180f));
            colors.Add(CalculateColor(magnitude, maxMagnitude));
        }

        if (vertices.Count >= 2)
        {
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.colors = colors.ToArray();
        
        if (triangles.Count > 0)
        {
            mesh.SetIndices(triangles.ToArray(), MeshTopology.LineStrip, 0);
        }
        
        mesh.RecalculateBounds();

        Debug.Log($"Single angle mesh created for {phiAngle}° with {vertices.Count} vertices");
    }

    private void CreateAxes()
    {
        DestroyAxes();

        xAxis = CreateAxis("X_Axis", Vector3.right * axisLength, xAxisColor);
        
        yAxis = CreateAxis("Y_Axis", Vector3.up * axisLength, yAxisColor);
        
        zAxis = CreateAxis("Z_Axis", Vector3.forward * axisLength, zAxisColor);

        CreateAxisLabels();
    }

    private GameObject CreateAxis(string name, Vector3 direction, Color color)
    {
        GameObject axis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        axis.name = name;
        axis.transform.SetParent(transform);
        axis.transform.localPosition = direction * 0.5f;
        axis.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        axis.transform.localScale = new Vector3(axisThickness, direction.magnitude * 0.5f, axisThickness);

        Renderer renderer = axis.GetComponent<Renderer>();
        Material axisMaterial = new Material(Shader.Find("Standard"));
        axisMaterial.color = color;
        renderer.material = axisMaterial;

        DestroyImmediate(axis.GetComponent<Collider>());

        return axis;
    }

    private void CreateAxisLabels()
    {
        CreateTextLabel("X_Label", "X", Vector3.right * (axisLength + 0.3f), xAxisColor);
        CreateTextLabel("Y_Label", "Y", Vector3.up * (axisLength + 0.3f), yAxisColor);
        CreateTextLabel("Z_Label", "Z", Vector3.forward * (axisLength + 0.3f), zAxisColor);
    }

    private void CreateTextLabel(string name, string text, Vector3 position, Color color)
    {
        GameObject label = new GameObject(name);
        label.transform.SetParent(transform);
        label.transform.localPosition = position;
        label.transform.localScale = Vector3.one * 0.1f;

        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = color;
        textMesh.fontSize = 150;
        textMesh.characterSize = 0.1f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        MeshRenderer meshRenderer = label.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = textMesh.font.material;
            _labels.Add(meshRenderer);
        }
    }

    private void DestroyAxes()
    {
        if (xAxis != null) DestroyImmediate(xAxis);
        if (yAxis != null) DestroyImmediate(yAxis);
        if (zAxis != null) DestroyImmediate(zAxis);

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != transform && (child.name.EndsWith("_Axis") || child.name.EndsWith("_Label")))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private float FindMaxMagnitude(List<FarfieldPoint> points)
    {
        float maxMagnitude = 0f;
        foreach (var point in points)
        {
            float magnitude = CalculateFieldMagnitude(point);
            if (magnitude > maxMagnitude)
                maxMagnitude = magnitude;
        }
        return maxMagnitude;
    }

    private Vector3 SphericalToCartesian(float theta, float phi)
    {
        float thetaRad = theta * Mathf.Deg2Rad;
        float phiRad = phi * Mathf.Deg2Rad;

        float x = Mathf.Sin(thetaRad) * Mathf.Cos(phiRad);
        float z = Mathf.Sin(thetaRad) * Mathf.Sin(phiRad);
        float y = Mathf.Cos(thetaRad);

        return new Vector3(x, y, z);
    }

    private float CalculateFieldMagnitude(FarfieldPoint point)
    {
        if (point == null) return 0f;
        
        float magTheta = Mathf.Sqrt(point.reTheta * point.reTheta + point.imTheta * point.imTheta);
        float magPhi = Mathf.Sqrt(point.rePhi * point.rePhi + point.imPhi * point.imPhi);
        
        return Mathf.Sqrt(magTheta * magTheta + magPhi * magPhi);
    }

    private Color CalculateColor(float magnitude, float maxMagnitude)
    {
        if (maxMagnitude <= 0f) return Color.blue;
        
        float normalizedMag = magnitude / maxMagnitude;
        return Color.Lerp(Color.blue, Color.red, normalizedMag);
    }

    [System.Serializable]
    public class FarfieldPoint
    {
        public float phi;
        public float theta;
        public float reTheta;
        public float imTheta;
        public float rePhi;
        public float imPhi;
        
        public override string ToString()
        {
            return $"Phi: {phi}, Theta: {theta}, E_Theta: ({reTheta}, {imTheta}), E_Phi: ({rePhi}, {imPhi})";
        }
    }

    void OnDestroy()
    {
        DestroyAxes();
    }
}