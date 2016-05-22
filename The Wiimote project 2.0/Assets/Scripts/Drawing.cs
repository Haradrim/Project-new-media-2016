using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Drawing : MonoBehaviour
{

    public Transform pointer;
    public Material brush;
    public float brushWidth = 0.25f;
    public bool isDrawing;
    private bool hasDrawn;

    private int lineCount;
    private LineRenderer currentRenderer;
    private List<Vector3> path;

    void Start()
    {
        lineCount = 0;
        isDrawing = false;
    }

    void Update()
    {

        if (isDrawing)
        {
            if (path == null)
            {
                NewLine();
            }
            if (!path.Contains(pointer.position))
            {
                path.Add(pointer.position);
                currentRenderer.SetVertexCount(path.Count);
                for (int i = 0; i < path.Count; i++)
                {
                    currentRenderer.SetPosition(i, path[i]);
                }
            }

        }
    }

    public void StartDrawing()
    {
        if (GameObject.Find("Line" + lineCount))
        {
            Debug.Log("Destroy");
            Destroy(GameObject.Find("Line" + lineCount));
        }
        isDrawing = true;
        hasDrawn = true;
    }

    public void StopDrawing()
    {
        if (hasDrawn)
        {
            NewLine();
            hasDrawn = false;
        }
        isDrawing = false;
    }

    void NewLine()
    {
        GameObject Line = new GameObject();
        Line.name = "Line" + lineCount;
        Line.AddComponent<LineRenderer>();
        path = new List<Vector3>();

        currentRenderer = GameObject.Find(Line.name).GetComponent<LineRenderer>();
        currentRenderer.SetWidth(brushWidth,brushWidth);
        currentRenderer.material = brush;
        lineCount++;
    }

    public void Undo()
    {
        if (lineCount != 0)
        {
            GameObject.Find("Line" + (lineCount - 1)).GetComponent<LineRenderer>().enabled = false;
            lineCount--;
        }

    }

    public void Redo()
    {
        if (GameObject.Find("Line" + lineCount))
        {
            GameObject.Find("Line" + lineCount).GetComponent<LineRenderer>().enabled = true;
            lineCount++;
        }

    }

    public void SetBrush(Material newMaterial)
    {
        brush = newMaterial;
    }
    public Material GetBrush()
    {
        return brush;
    }
}
