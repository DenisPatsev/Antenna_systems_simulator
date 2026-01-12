using UnityEngine;
using System.Collections.Generic;

public class LampGlowMeshGenerator : MonoBehaviour
{
    [Header("Glow Settings")] public float zOffset = 3f;
    public float glowLength = 3f;
    public float startWidth = 0.1f;
    public float startHeight = 0.05f;
    public float endWidth = 2f;
    public float endHeight = 1f;
    public int widthSegments = 8;
    public int lengthSegments = 8;
    public int heightSegments = 3;

    [Header("Corner Rounding")] public float cornerRoundness = 0.5f;
    public float cornerRadius = 0.2f;

    [Header("Material")] public Material glowMaterial;

    private GameObject glowMeshObject;
    private Mesh mesh;

    void Start()
    {
        GenerateGlowMesh();
        glowMeshObject.transform.position = new Vector3(glowMeshObject.transform.position.x,
            glowMeshObject.transform.position.y + zOffset, glowMeshObject.transform.position.z);
    }

    // private void OnDrawGizmos()
    // {
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         GenerateGlowMesh();
    //         glowMeshObject.transform.position = new Vector3(glowMeshObject.transform.position.x,
    //             glowMeshObject.transform.position.y + zOffset, glowMeshObject.transform.position.z);
    //     }
    // }

    [ContextMenu("Generate Glow Mesh")]
    public void GenerateGlowMesh()
    {
        // Удаляем старый меш если есть
        if (glowMeshObject != null)
        {
            if (Application.isPlaying)
                Destroy(glowMeshObject);
            else
                DestroyImmediate(glowMeshObject);
        }

        // Создаем дочерний GameObject для меша
        glowMeshObject = new GameObject("LampGlowMesh");
        glowMeshObject.transform.SetParent(transform);
        glowMeshObject.transform.localPosition = Vector3.zero;
        glowMeshObject.transform.localRotation = Quaternion.identity;
        glowMeshObject.transform.localScale = Vector3.one;

        // Добавляем компоненты
        MeshFilter meshFilter = glowMeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = glowMeshObject.AddComponent<MeshRenderer>();

        // Создаем и назначаем материал
        if (glowMaterial != null)
        {
            meshRenderer.material = glowMaterial;
        }
        else
        {
            // Создаем временный материал если не назначен
            meshRenderer.material = CreateDefaultMaterial();
        }

        // Генерируем меш
        mesh = new Mesh();
        mesh.name = "GeneratedGlowMesh";

        CreateVertices();
        CreateTriangles();
        CreateUV();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

    }

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    void CreateVertices()
    {
        // Количество вершин: (ширина + 1) * (длина + 1) * (высота + 1)
        int vertexCount = (widthSegments + 1) * (lengthSegments + 1) * (heightSegments + 1);
        vertices = new Vector3[vertexCount];

        float lengthStep = glowLength / lengthSegments;

        int vertexIndex = 0;

        for (int lengthSegment = 0; lengthSegment <= lengthSegments; lengthSegment++)
        {
            float t_length = (float)lengthSegment / lengthSegments;
            float currentLength = lengthStep * lengthSegment;

            // Интерполируем размеры для трапеции
            float currentWidth = Mathf.Lerp(startWidth, endWidth, t_length);
            float currentHeight = Mathf.Lerp(startHeight, endHeight, t_length);

            // Текущий радиус скругления (интерполируем от начала к концу)
            float currentCornerRadius = Mathf.Lerp(cornerRadius * startWidth, cornerRadius * endWidth, t_length);

            for (int heightSegment = 0; heightSegment <= heightSegments; heightSegment++)
            {
                float t_height = (float)heightSegment / heightSegments;

                for (int widthSegment = 0; widthSegment <= widthSegments; widthSegment++)
                {
                    float t_width = (float)widthSegment / widthSegments;

                    // Применяем скругление углов
                    Vector3 vertex = ApplyCornerRounding(t_width, t_height, currentLength, currentWidth, currentHeight,
                        currentCornerRadius);

                    vertices[vertexIndex] = vertex;
                    vertexIndex++;
                }
            }
        }
    }

    Vector3 ApplyCornerRounding(float t_width, float t_height, float z, float currentWidth, float currentHeight,
        float currentCornerRadius)
    {
        if (cornerRoundness <= 0.001f)
        {
            // Без скругления - обычная трапеция
            float baseX = (t_width - 0.5f) * currentWidth;
            float baseY = (t_height - 0.5f) * currentHeight;
            return new Vector3(baseX, baseY, z);
        }

        // Нормализованные координаты от -1 до 1
        float normX = (t_width - 0.5f) * 2f;
        float normY = (t_height - 0.5f) * 2f;

        // Абсолютные значения для определения углов
        float absX = Mathf.Abs(normX);
        float absY = Mathf.Abs(normY);

        // Максимальное расстояние от центра в нормализованных координатах
        float maxDist = Mathf.Max(absX, absY);

        // Определяем, находимся ли мы в угловой области
        float cornerStart = 1f - currentCornerRadius / (currentWidth * 0.5f);

        if (maxDist > cornerStart)
        {
            // Мы в угловой области - применяем скругление

            // Угол точки относительно центра
            float angle = Mathf.Atan2(normY, normX);

            // Радиус скругления в нормализованных координатах
            float normalizedRadius = currentCornerRadius / (currentWidth * 0.5f);

            // Позиция на скругленном углу
            float roundedX = Mathf.Cos(angle) * (1f - normalizedRadius + normalizedRadius * cornerRoundness);
            float roundedY = Mathf.Sin(angle) * (1f - normalizedRadius + normalizedRadius * cornerRoundness);

            // Преобразуем обратно в локальные координаты
            float x = roundedX * currentWidth * 0.5f;
            float y = roundedY * currentHeight * 0.5f;

            return new Vector3(x, y, z);
        }
        else
        {
            // Мы не в угловой области - обычная позиция
            float x = normX * currentWidth * 0.5f;
            float y = normY * currentHeight * 0.5f;
            return new Vector3(x, y, z);
        }
    }

    void CreateTriangles()
    {
        // Упрощаем - создаем только внешние грани объемной трапеции
        List<int> triangleList = new List<int>();

        int verticesPerLayer = (widthSegments + 1) * (heightSegments + 1);

        // 1. Передняя грань (Z = 0)
        for (int h = 0; h < heightSegments; h++)
        {
            for (int w = 0; w < widthSegments; w++)
            {
                int bottomLeft = h * (widthSegments + 1) + w;
                int bottomRight = bottomLeft + 1;
                int topLeft = (h + 1) * (widthSegments + 1) + w;
                int topRight = topLeft + 1;

                // Два треугольника для квадрата
                triangleList.Add(bottomLeft);
                triangleList.Add(topLeft);
                triangleList.Add(bottomRight);

                triangleList.Add(bottomRight);
                triangleList.Add(topLeft);
                triangleList.Add(topRight);
            }
        }

        // 2. Задняя грань (Z = glowLength)
        int backLayerStart = lengthSegments * verticesPerLayer;
        for (int h = 0; h < heightSegments; h++)
        {
            for (int w = 0; w < widthSegments; w++)
            {
                int bottomLeft = backLayerStart + h * (widthSegments + 1) + w;
                int bottomRight = bottomLeft + 1;
                int topLeft = backLayerStart + (h + 1) * (widthSegments + 1) + w;
                int topRight = topLeft + 1;

                // Обратный порядок для задней грани
                triangleList.Add(bottomLeft);
                triangleList.Add(bottomRight);
                triangleList.Add(topLeft);

                triangleList.Add(bottomRight);
                triangleList.Add(topRight);
                triangleList.Add(topLeft);
            }
        }

        // 3. Верхняя грань (Y+)
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int w = 0; w < widthSegments; w++)
            {
                int currentLayer = l * verticesPerLayer;
                int nextLayer = (l + 1) * verticesPerLayer;

                int bottomLeft = currentLayer + heightSegments * (widthSegments + 1) + w;
                int bottomRight = bottomLeft + 1;
                int topLeft = nextLayer + heightSegments * (widthSegments + 1) + w;
                int topRight = topLeft + 1;

                triangleList.Add(bottomLeft);
                triangleList.Add(topLeft);
                triangleList.Add(bottomRight);

                triangleList.Add(bottomRight);
                triangleList.Add(topLeft);
                triangleList.Add(topRight);
            }
        }

        // 4. Нижняя грань (Y-)
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int w = 0; w < widthSegments; w++)
            {
                int currentLayer = l * verticesPerLayer;
                int nextLayer = (l + 1) * verticesPerLayer;

                int bottomLeft = currentLayer + w;
                int bottomRight = bottomLeft + 1;
                int topLeft = nextLayer + w;
                int topRight = topLeft + 1;

                // Обратный порядок для нижней грани
                triangleList.Add(bottomLeft);
                triangleList.Add(bottomRight);
                triangleList.Add(topLeft);

                triangleList.Add(bottomRight);
                triangleList.Add(topRight);
                triangleList.Add(topLeft);
            }
        }

        // 5. Левая грань (X-)
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int h = 0; h < heightSegments; h++)
            {
                int currentLayer = l * verticesPerLayer;
                int nextLayer = (l + 1) * verticesPerLayer;

                int bottomLeft = currentLayer + h * (widthSegments + 1);
                int topLeft = currentLayer + (h + 1) * (widthSegments + 1);
                int bottomRight = nextLayer + h * (widthSegments + 1);
                int topRight = nextLayer + (h + 1) * (widthSegments + 1);

                triangleList.Add(bottomLeft);
                triangleList.Add(topLeft);
                triangleList.Add(bottomRight);

                triangleList.Add(topLeft);
                triangleList.Add(topRight);
                triangleList.Add(bottomRight);
            }
        }

        // 6. Правая грань (X+)
        for (int l = 0; l < lengthSegments; l++)
        {
            for (int h = 0; h < heightSegments; h++)
            {
                int currentLayer = l * verticesPerLayer;
                int nextLayer = (l + 1) * verticesPerLayer;

                int bottomLeft = currentLayer + h * (widthSegments + 1) + widthSegments;
                int topLeft = currentLayer + (h + 1) * (widthSegments + 1) + widthSegments;
                int bottomRight = nextLayer + h * (widthSegments + 1) + widthSegments;
                int topRight = nextLayer + (h + 1) * (widthSegments + 1) + widthSegments;

                // Обратный порядок для правой грани
                triangleList.Add(bottomLeft);
                triangleList.Add(bottomRight);
                triangleList.Add(topLeft);

                triangleList.Add(topLeft);
                triangleList.Add(bottomRight);
                triangleList.Add(topRight);
            }
        }

        triangles = triangleList.ToArray();
    }

    void CreateUV()
    {
        uv = new Vector2[vertices.Length];

        int vertexIndex = 0;

        for (int lengthSegment = 0; lengthSegment <= lengthSegments; lengthSegment++)
        {
            for (int heightSegment = 0; heightSegment <= heightSegments; heightSegment++)
            {
                for (int widthSegment = 0; widthSegment <= widthSegments; widthSegment++)
                {
                    float u = (float)widthSegment / widthSegments;
                    float v = (float)lengthSegment / lengthSegments;

                    uv[vertexIndex] = new Vector2(u, v);
                    vertexIndex++;
                }
            }
        }
    }

    [ContextMenu("Destroy Glow Mesh")]
    public void DestroyGlowMesh()
    {
        if (glowMeshObject != null)
        {
            if (Application.isPlaying)
                Destroy(glowMeshObject);
            else
                DestroyImmediate(glowMeshObject);

            glowMeshObject = null;
        }
    }

    void OnDestroy()
    {
        DestroyGlowMesh();
    }

    private Material CreateDefaultMaterial()
    {
        // Создаем простой материал для свечения
        Shader shader = Shader.Find("Standard");
        Material material = new Material(shader);
        material.color = new Color(1f, 1f, 0.8f, 0.3f);
        material.SetFloat("_Mode", 2); // Fade mode
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        return material;
    }

    void OnDrawGizmosSelected()
    {
        if (glowMeshObject != null && vertices != null && vertices.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = glowMeshObject.transform.localToWorldMatrix;

            // Рисуем только угловые вершины для наглядности
            int verticesPerLayer = (widthSegments + 1) * (heightSegments + 1);

            // Углы переднего слоя (начало)
            Gizmos.DrawSphere(vertices[0], 0.02f); // нижний левый
            Gizmos.DrawSphere(vertices[widthSegments], 0.02f); // нижний правый
            Gizmos.DrawSphere(vertices[heightSegments * (widthSegments + 1)], 0.02f); // верхний левый
            Gizmos.DrawSphere(vertices[heightSegments * (widthSegments + 1) + widthSegments], 0.02f); // верхний правый

            // Углы заднего слоя (конец)
            int lastLayerStart = lengthSegments * verticesPerLayer;
            Gizmos.DrawSphere(vertices[lastLayerStart], 0.02f); // нижний левый
            Gizmos.DrawSphere(vertices[lastLayerStart + widthSegments], 0.02f); // нижний правый
            Gizmos.DrawSphere(vertices[lastLayerStart + heightSegments * (widthSegments + 1)], 0.02f); // верхний левый
            Gizmos.DrawSphere(vertices[lastLayerStart + heightSegments * (widthSegments + 1) + widthSegments],
                0.02f); // верхний правый

            // Рисуем каркас трапеции
            Gizmos.color = Color.green;

            // Переднее основание (малое)
            Gizmos.DrawLine(vertices[0], vertices[widthSegments]);
            Gizmos.DrawLine(vertices[0], vertices[heightSegments * (widthSegments + 1)]);
            Gizmos.DrawLine(vertices[widthSegments], vertices[heightSegments * (widthSegments + 1) + widthSegments]);
            Gizmos.DrawLine(vertices[heightSegments * (widthSegments + 1)],
                vertices[heightSegments * (widthSegments + 1) + widthSegments]);

            // Заднее основание (большое)
            Gizmos.DrawLine(vertices[lastLayerStart], vertices[lastLayerStart + widthSegments]);
            Gizmos.DrawLine(vertices[lastLayerStart], vertices[lastLayerStart + heightSegments * (widthSegments + 1)]);
            Gizmos.DrawLine(vertices[lastLayerStart + widthSegments],
                vertices[lastLayerStart + heightSegments * (widthSegments + 1) + widthSegments]);
            Gizmos.DrawLine(vertices[lastLayerStart + heightSegments * (widthSegments + 1)],
                vertices[lastLayerStart + heightSegments * (widthSegments + 1) + widthSegments]);

            // Боковые ребра
            Gizmos.DrawLine(vertices[0], vertices[lastLayerStart]);
            Gizmos.DrawLine(vertices[widthSegments], vertices[lastLayerStart + widthSegments]);
            Gizmos.DrawLine(vertices[heightSegments * (widthSegments + 1)],
                vertices[lastLayerStart + heightSegments * (widthSegments + 1)]);
            Gizmos.DrawLine(vertices[heightSegments * (widthSegments + 1) + widthSegments],
                vertices[lastLayerStart + heightSegments * (widthSegments + 1) + widthSegments]);
        }
    }

    // Методы для динамического изменения параметров
    public void SetGlowLength(float newLength)
    {
        glowLength = newLength;
        GenerateGlowMesh();
    }

    public void SetStartSize(float width, float height)
    {
        startWidth = width;
        startHeight = height;
        GenerateGlowMesh();
    }

    public void SetEndSize(float width, float height)
    {
        endWidth = width;
        endHeight = height;
        GenerateGlowMesh();
    }

    public void SetCornerRoundness(float roundness)
    {
        cornerRoundness = Mathf.Clamp01(roundness);
        GenerateGlowMesh();
    }

    public void SetCornerRadius(float radius)
    {
        cornerRadius = Mathf.Max(0, radius);
        GenerateGlowMesh();
    }

    public void SetQuality(int widthSegs, int lengthSegs, int heightSegs)
    {
        widthSegments = widthSegs;
        lengthSegments = lengthSegs;
        heightSegments = heightSegs;
        GenerateGlowMesh();
    }

    public void SetMaterial(Material newMaterial)
    {
        glowMaterial = newMaterial;
        if (glowMeshObject != null)
        {
            MeshRenderer renderer = glowMeshObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = glowMaterial;
            }
        }
    }
}