using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public enum CurveCellType {TR, TL, BL, BR, N, EL, ET, ER, EB, H, V}

public class CellularLevelGenerator : MonoBehaviour
{
    
	ArrayList curvePoints;
	struct CurvePoint {
		public int x;
		public int y;
		public CurveCellType type;
	}
	
	int[,] generatedMap;
  
    public int width;
    public int height;
 
    public string seed;
    private System.Random pseudoRandom;

	public bool useRandomSeed;
 
    [Range(0,100)]
    public int fillingPercentage;

 
// Hilbert's curve parameters
	Vector2[]  hilbertPoints;
	public int hilbertReps;
	public int hilbertScale;
	int        hilbertIdx = 0;
	
	
	int[,] hilbertPointsInt;

	public int pathGirth;

	int[,] hilbertNegativePointsInt;
	public int negativePathGirth;

	Dictionary<Vector2, ArrayList> segments;
	
	
	
	void CreateCriticalPath() {
		if (segments != null && segments.Count > 1) {
			Dictionary<string, List<KeyValuePair<Vector2Int, Vector2Int>>> list = new Dictionary<string, List<KeyValuePair<Vector2Int, Vector2Int>>>();
			for(int cnt = 0; cnt < segments.Count - 1; cnt++) {
				ArrayList cellsA = segments.ElementAt(cnt).Value;
				List<KeyValuePair<Vector2Int, Vector2Int>> segmentConnectors = new List<KeyValuePair<Vector2Int, Vector2Int>>();
				int i = 0;
				do {
					i++;
					ArrayList cellsB = segments.ElementAt(cnt+i).Value;
					segmentConnectors = FindConnectorsBetweenSegments(cellsA, cellsB, segmentConnectors);
				} while (segmentConnectors.Count == 0);

				string key =  cnt + "_" + i;
				list.Add(key, segmentConnectors);
			}
			foreach (KeyValuePair<string, List<KeyValuePair<Vector2Int, Vector2Int>>> item in list) {
				List <KeyValuePair<Vector2Int, Vector2Int>> connectors = item.Value;
				if (connectors.Count < 1) continue;

				int rand = pseudoRandom.Next(0, connectors.Count);
				Vector2Int pointFrom = (Vector2Int)connectors.ElementAt(rand).Key;
				Vector2Int pointTo = (Vector2Int)connectors.ElementAt(rand).Value;
				ImprintPathBetweenCells(pointFrom, pointTo, hilbertPointsInt);
			}
		}
	}

	void ImprintPathBetweenCells(Vector2 pointFrom, Vector2 pointTo, int[,] map)
	{
		// filling in all cells between hilbert curve nodes
		int x_curr = (int)pointFrom.x;
		int y_curr = (int)pointFrom.y;
		int x_next = (int)pointTo.x;
		int y_next = (int)pointTo.y;
		int x_dist = (int)Mathf.Abs(x_curr - x_next);
		int y_dist = (int)Mathf.Abs(y_curr - y_next);
	
		Vector2 dir = ((Vector2)pointTo - (Vector2)pointFrom).normalized;
	
		if (x_dist == 0 && y_dist > 0)
		{
			for (int y = 0; y < y_dist; y++)
			{
				if (x_curr >= 0 && x_curr < Mathf.Max(width, height) &&
					y_curr + ((int)dir.y * y) >= 0 &&
					y_curr + ((int)dir.y * y) < Mathf.Max(width, height))
				{
					map[x_curr, y_curr + ((int)dir.y * y)] = 1;
				}
			}
		}
		else if (x_dist > 0 && y_dist == 0)
		{
			for (int x = 0; x < x_dist; x++)
			{
				if (x_curr + ((int)dir.x * x) >= 0 &&
					x_curr + ((int)dir.x * x) < Mathf.Max(width, height) &&
					y_curr >= 0 && y_curr < Mathf.Max(width, height))
				{
					map[x_curr + ((int)dir.x * x), y_curr] = 1;
				}
			}
		}
	}

	//Checks if two points are on the same y or x level and returns True or False
	bool AreCellsLeveled(Vector2 pointA, Vector2 pointB) {
		if (pointA.x == pointB.x || pointA.y == pointB.y) {
			return true;
		} else {
			return false;
		}
	}


//Finds if two seperate segments can be connected and returns a list of of how/where this connection can occur
	List<KeyValuePair<Vector2Int, Vector2Int>> FindConnectorsBetweenSegments(
		ArrayList cellsA, ArrayList cellsB, List<KeyValuePair<Vector2Int, Vector2Int>> segmentConnectors) {
			for (int i = 0; i < cellsA.Count; i++) {
				for (int j = 0; j < cellsB.Count; j++) {
					Vector2Int posA = (Vector2Int)cellsA[i];
					Vector2Int posB = (Vector2Int)cellsB[j];
					int x_dist = (int)Mathf.Abs(posA.x -posB.x);
					int y_dist = (int)Mathf.Abs(posA.y -posB.y);
					Vector2 dir = ((Vector2)posB - (Vector2)posA).normalized;
					if (AreCellsLeveled(posA, posB)) {
						if (x_dist == 0 && y_dist > 0) {
							if (hilbertPointsInt[posA.x, posA.y + (int)dir.y] == 1) continue;
						} else if (x_dist > 0 && y_dist == 0) {
							if (hilbertPointsInt[posA.x + (int)dir.x, posA.y] == 1) continue;
						}
						segmentConnectors.Add(new KeyValuePair<Vector2Int, Vector2Int>(posA, posB));
					}

				}

			}
			return segmentConnectors;
		}
	
	
	void CreateCurveCorners() {
		curvePoints.Clear();
		for (int x = 1; x < width - 1; x++) {
			for (int y = 1; y < height - 1; y++) {
				CurvePoint cPoint = GetCurvePoint(x,y);
				if (cPoint.type.Equals(CurveCellType.N) || cPoint.type.Equals(CurveCellType.H) || cPoint.type.Equals(CurveCellType.V)) continue;
				curvePoints.Add(cPoint);
			}
		}
	}


	void CreateNegativePath() {
		for (int x = 1; x < width - 1; x++) {
			for (int y = 1; y < height - 1; y++) {
				hilbertNegativePointsInt[x,y] = (hilbertPointsInt[x,y]==1) ? 0:1;
			}
		}

		for (int x = 1; x < width - 1; x++) {
			for (int y = 1; y < height - 1; y++) {
				int neighbors = GetNeighboursNegativePathCellCount(x,y);
				hilbertNegativePointsInt[x,y] = (neighbors > 0) ? 0 : hilbertNegativePointsInt[x,y];
			}
		}

	}


	int GetNeighboursNegativePathCellCount(int x, int y) {
		int neighbors = 0;
		for (int i = -negativePathGirth; i <= negativePathGirth; i++) {
			for (int j = -negativePathGirth; j <= negativePathGirth; j++) {
				int i_x = i + x;
				int j_y = j + y;

				if (i_x <= 0 || j_y <= 0 || i_x > width - 1 || j_y > height - 1) neighbors += 1;
				else neighbors += hilbertPointsInt[i_x, j_y];
			}
		}
		neighbors -= hilbertPointsInt[x,y];

		return neighbors;
	}


	//Fills neighboring cells of a cell with corresponding val
	void FloodFillNeighboursWithValue(int x, int y, int val) {
		for (int i = -pathGirth; i <= pathGirth; i++) {
			for (int j = -pathGirth; j <= pathGirth; j++) {
				int i_x = i + x;
				int j_y = j + y;

				if (i_x < 0 || j_y < 0 || i_x > width - 1 || j_y > height - 1) {
					continue;
				}

				generatedMap[i_x, j_y] = val;
			}
		}
	}

//Effectively the main function, creates random shift values for x and y to ensure that the maps are different every time
// also creates the basic form of the map before relying on other functions
	void GenerateGuidelines() {
		hilbertPoints = new Vector2[(int)Mathf.Pow(4, hilbertReps)];
		hilbertPointsInt = new int[Mathf.Max(width, height), Mathf.Max(width, height)];
		hilbertIdx = 0;
		hilbertNegativePointsInt = new int[width, height];
		shiftX = pseudoRandom.Next(-Mathf.Max(width, height) * (hilbertScale - 1), 0);
		shiftY = pseudoRandom.Next(-Mathf.Max(width, height) * (hilbertScale - 1), 0);
		HilbertCurve(0.0f, 0.0f, 1.0f * Mathf.Max(width, height),
                 0.0f, 0.0f, 1.0f * Mathf.Max(width, height),
                 hilbertReps);
		for (int x = 1; x < width - 1; x++) {
			for (int y = 1; y < height - 1; y++) {
				hilbertPointsInt[x,y] = 0;
			}
		}

		for (int i = 0; i < hilbertPoints.Length; i++) {
			int x = (int) hilbertPoints[i].x;
			int y = (int) hilbertPoints[i].y;
			if (x < Mathf.Max(width, height) && x >= 0 && y < Mathf.Max(width, height) && y >= 0 ) {
				hilbertPointsInt[x,y] = 1;
			}


		}

		for (int i = 0; i < hilbertPoints.Length - 1; i++) {
			ImprintPathBetweenCells(hilbertPoints[i], hilbertPoints[i+1], hilbertPointsInt);

		}

		
		CreateCurveCorners();
        CreateSegments();
		CreateCriticalPath();
		CreateNegativePath();
	}

	[SerializeField]
	int shiftX, shiftY;
	
	//Hilbert Curve for pathway shapes, scale and repitition of curve can be adjusted in UI
	void HilbertCurve(float x, float y, float xi, float xj, float yi, float yj, int n)
	{
	
		if (n <= 0)
		{
			float X = x + (xi + yi) / 2;
			float Y = y + (xj + yj) / 2;
			hilbertPoints[hilbertIdx] = new Vector2((int)X * hilbertScale + shiftX, (int)Y * hilbertScale + shiftY);
			hilbertIdx++;
		}
		else
		{
			HilbertCurve(x, y, yi / 2, yj / 2, xi / 2, xj / 2, n - 1);
			HilbertCurve(x + xi / 2, y + xj / 2, xi / 2, xj / 2, yi / 2, yj / 2, n - 1);
			HilbertCurve(x + xi / 2 + yi / 2, y + xj / 2 + yj / 2, xi / 2, xj / 2, yi / 2, yj / 2, n - 1);
			HilbertCurve(x + xi / 2 + yi, y + xj / 2 + yj, -yi / 2, -yj / 2, -xi / 2, -xj / 2, n - 1);
		}
	}


    void Start()
    {	
		segments = new Dictionary<Vector2, ArrayList>();
        curvePoints = new ArrayList();
		CellularAutomata();
    }

	
	//Creates path segments
	void CreateSegments(){
		// create path's segments
		segments.Clear();
		ArrayList endEdgePoints = new ArrayList();              // to contain ending edge point so that we can skip them
	
		for (int i = 0; i < curvePoints.Count; i++)
		{
			CurvePoint cPoint = (CurvePoint)curvePoints[i];
			Vector2Int ePoint = new Vector2Int(cPoint.x, cPoint.y);
	
			if (endEdgePoints.Contains(ePoint))
				continue;
	
			if (cPoint.type.Equals(CurveCellType.EL) ||
				cPoint.type.Equals(CurveCellType.EB))
			{
				ArrayList segmentPoints = new ArrayList();
				Vector2Int point = ePoint;
	
				while (!segments.ContainsKey(ePoint))
				{
					Vector2Int right = new Vector2Int(point.x + 1, point.y);
					Vector2Int left = new Vector2Int(point.x - 1, point.y);
					Vector2Int top = new Vector2Int(point.x, point.y + 1);
					Vector2Int bottom = new Vector2Int(point.x, point.y - 1);
	
					// go right!
					if (hilbertPointsInt[right.x, right.y] == 1 &&
						!segmentPoints.Contains(right))
					{
						point = right;
						segmentPoints.Add(point);
					}
					// go up!
					else if (hilbertPointsInt[top.x, top.y] == 1 && 
							!segmentPoints.Contains(top))
					{
						point = top;
						segmentPoints.Add(point);
					}
					// go left!
					else if (hilbertPointsInt[left.x, left.y] == 1 &&
							!segmentPoints.Contains(left))
					{
						point = left;
						segmentPoints.Add(point);
					}
					// go down!
					else if (hilbertPointsInt[bottom.x, bottom.y] == 1 &&
							!segmentPoints.Contains(bottom))
					{
						point = bottom;
						segmentPoints.Add(point);
					}
	
					CurvePoint curvePoint = GetCurvePoint(point.x, point.y);
	
					if (curvePoint.type.Equals(CurveCellType.EB) ||
						curvePoint.type.Equals(CurveCellType.EL) ||
						curvePoint.type.Equals(CurveCellType.ER) ||
						curvePoint.type.Equals(CurveCellType.ET) ||
						getNeighboursCellCount(point.x, point.y, hilbertPointsInt) == 1)
					{
						endEdgePoints.Add(point);   // last point in a segment
						segments.Add(ePoint, segmentPoints);
					}
				}
			}
			else if (cPoint.type.Equals(CurveCellType.ET) ||
					cPoint.type.Equals(CurveCellType.ER))
			{
				ArrayList segmentPoints = new ArrayList();
				Vector2Int point = ePoint;
	
				while (!segments.ContainsKey(ePoint))
				{
					Vector2Int right = new Vector2Int(point.x + 1, point.y);
					Vector2Int left = new Vector2Int(point.x - 1, point.y);
					Vector2Int top = new Vector2Int(point.x, point.y + 1);
					Vector2Int bottom = new Vector2Int(point.x, point.y - 1);
	
					// go left!
					if (hilbertPointsInt[left.x, left.y] == 1 &&
						!segmentPoints.Contains(left))
					{
						point = left;
						segmentPoints.Add(point);
					}
					// go down!
					else if (hilbertPointsInt[bottom.x, bottom.y] == 1 &&
							!segmentPoints.Contains(bottom))
					{
						point = bottom;
						segmentPoints.Add(point);
					}
	
					// go right!
					else if (hilbertPointsInt[right.x, right.y] == 1 &&
							!segmentPoints.Contains(right))
					{
						point = right;
						segmentPoints.Add(point);
					}
					// go up!
					else if (hilbertPointsInt[top.x, top.y] == 1 && !segmentPoints.Contains(top))
					{
						point = top;
						segmentPoints.Add(point);
					}
	
					CurvePoint curvePoint = GetCurvePoint(point.x, point.y);
	
					if (curvePoint.type.Equals(CurveCellType.EB) ||
						curvePoint.type.Equals(CurveCellType.EL) ||
						curvePoint.type.Equals(CurveCellType.ER) ||
						curvePoint.type.Equals(CurveCellType.ET))
					{
						endEdgePoints.Add(point);   // last point in a segment
						segments.Add(ePoint, segmentPoints);
					}
				}
			}
		}
		Debug.Log("Segments: " + segments.Count);
	}
 


    void CellularAutomata()
    {
        if (useRandomSeed) {
			seed = System.DateTime.Now.Ticks.ToString();
		}
        pseudoRandom = new System.Random(seed.GetHashCode());

		hilbertPoints = new Vector2[(int)Mathf.Pow(4, hilbertReps)];
		hilbertIdx = 0;
	
		GenerateGuidelines();	
        GenerateMap();
        for (int i = 0; i < 5; i++)
            SmoothMap();
 
        RemoveSecludedCells();
        RecoverEdgeCells();
    }
 
    void GenerateMap()
    {
        generatedMap = new int[width, height];
        
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                generatedMap[x, y] = (pseudoRandom.Next(0, 100) < fillingPercentage) ? 1 : 0;
            }
        }
    }
 
    int getNeighboursCellCount(int x, int y, int[,] map)
    {
        int neighbors = 0;
        for(int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                neighbors += map[i + x, j + y];
             
        neighbors -= map[x, y];
 
        return neighbors;
    }
 
    void SmoothMap()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                int neighbors = getNeighboursCellCount(x, y, generatedMap);
 
 
                if (neighbors > 4)
                    generatedMap[x, y] = 1;
                else if (neighbors < 4)
                    generatedMap[x, y] = 0;
				
				generatedMap[x,y] = (hilbertNegativePointsInt[x,y] == 1) ? 0 : generatedMap[x,y];

				if (hilbertPointsInt[x, y] == 1) {
					generatedMap[x,y] = 1;
					if (neighbors > 4) FloodFillNeighboursWithValue(x, y, 1);
				}
            }


        } 
    }
 
    void RecoverEdgeCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    generatedMap[x, y] = 0;
            }
        }
    }
 
    void RemoveSecludedCells()
    {
        for (int x = 1; x < width-1; x++)
        {
            for (int y = 1; y < height-1; y++)
            {
                generatedMap[x, y] = (getNeighboursCellCount(x, y, generatedMap) <= 0) ? 0 : generatedMap[x, y];
            }
        }
 
    }
 
CurvePoint GetCurvePoint(int x, int y)
{
    int x_left  = x - 1;
    int x_right = x + 1;
 
    int y_bottom = y - 1;
    int y_top    = y + 1;
 
    CurvePoint cell = new CurvePoint();
    cell.x = x;
    cell.y = y;
    cell.type = CurveCellType.N;
 
    if(y_bottom < 0)
    {
        // EB - edge-bottom
        if (y_bottom == -1                    &&
            hilbertPointsInt[x, y]       == 1 &&
            hilbertPointsInt[x_right, y] == 0 &&
            hilbertPointsInt[x, y_top]   == 1 &&
            hilbertPointsInt[x_left, y]  == 0)
        {
            cell.type = CurveCellType.EB;
        }
    }
    else if(x_left < 0)
    {
        // EL - edge-left
        if (x_left == -1 &&
            hilbertPointsInt[x, y]        == 1 &&
            hilbertPointsInt[x_right, y]  == 1 &&
            hilbertPointsInt[x, y_top]    == 0 &&
            hilbertPointsInt[x, y_bottom] == 0)
        {
            cell.type = CurveCellType.EL;
        }
    }
    else if (x_right > width - 1)
    {
        // ER - edge-right
        if (x_right == width                   &&
            hilbertPointsInt[x, y]        == 1 &&
            hilbertPointsInt[x_left, y]   == 1 &&
            hilbertPointsInt[x, y_top]    == 0 &&
            hilbertPointsInt[x, y_bottom] == 0)
        {
            cell.type = CurveCellType.ER;
        }
    }
    else if (y_top > height - 1)
    {
        // ET - edge-top
        if (y_top == height                    &&
            hilbertPointsInt[x, y]        == 1 &&
            hilbertPointsInt[x_left, y]   == 0 &&
            hilbertPointsInt[x, y_top]    == 0 &&
            hilbertPointsInt[x, y_bottom] == 1)
        {
            cell.type = CurveCellType.ET;
        }
    }
    // We are dealing with 'edge' cell
    else if (x_left   == 0 || x_right == width  - 1 ||
             y_bottom == 0 || y_top   == height - 1)
    {
        // ER - edge-right
        if (x_right + 1 == width                &&
            hilbertPointsInt[x_right, y]   == 1 &&
            hilbertPointsInt[x, y]         == 1 &&
            hilbertPointsInt[x, y_top]     == 0 &&
            hilbertPointsInt[x, y_bottom]  == 0 )
        {
            cell.type = CurveCellType.ER;
        }
        // EL - edge-left
        else if (x_left                        == 0 &&
                 hilbertPointsInt[x_left, y]   == 1 &&
                 hilbertPointsInt[x, y]        == 1 &&
                 hilbertPointsInt[x, y_top]    == 0 &&
                 hilbertPointsInt[x, y_bottom] == 0)
        {
            cell.type = CurveCellType.EL;
        }
        // ET - edge-top
        else if (y_top + 1                     == height &&
                 hilbertPointsInt[x_left, y]   == 0 &&
                 hilbertPointsInt[x_right, y]  == 0 &&
                 hilbertPointsInt[x, y]        == 1 &&
                 hilbertPointsInt[x, y_bottom] == 1)
        {
            cell.type = CurveCellType.ET;
        }
        // EB - edge-bottom
        else if (y_bottom                      == 0 &&
                 hilbertPointsInt[x_left,  y]  == 0 &&
                 hilbertPointsInt[x_right, y]  == 0 &&
                 hilbertPointsInt[x, y_top]    == 1 &&
                 hilbertPointsInt[x, y]        == 1)
        {
            cell.type = CurveCellType.EB;
        }
    }
    else if (x_left   > 0 && x_right < width  - 1 &&
             y_bottom > 0 && y_top   < height - 1)
    {
        // H - horizontal
        if (hilbertPointsInt[x_left, y]        == 1 &&
            hilbertPointsInt[x_right, y]       == 1 &&
            hilbertPointsInt[x, y_top]         == 0 &&
            hilbertPointsInt[x, y_bottom]      == 0)
        {       
            cell.type = CurveCellType.H;
        }
        // V - vertical
        else if (hilbertPointsInt[x_left, y]   == 0 &&
                 hilbertPointsInt[x_right, y]  == 0 &&
                 hilbertPointsInt[x, y_top]    == 1 &&
                 hilbertPointsInt[x, y_bottom] == 1)
        {
            cell.type = CurveCellType.V;
        }
        // TL - top-left
        else if (hilbertPointsInt[x_left, y]   == 1 &&
                 hilbertPointsInt[x_right, y]  == 0 &&
                 hilbertPointsInt[x, y_top]    == 0 &&
                 hilbertPointsInt[x, y_bottom] == 1)
        {
            cell.type = CurveCellType.TL;
        }
        // TR - top-right
        else if (hilbertPointsInt[x_left, y]   == 0 &&
                 hilbertPointsInt[x_right, y]  == 1 &&
                 hilbertPointsInt[x, y_top]    == 0 &&
                 hilbertPointsInt[x, y_bottom] == 1)
        {
            cell.type = CurveCellType.TR;
        }
        // BL - bottom-left
        else if (hilbertPointsInt[x_left, y]   == 1 &&
                 hilbertPointsInt[x_right, y]  == 0 &&
                 hilbertPointsInt[x, y_top]    == 1 &&
                 hilbertPointsInt[x, y_bottom] == 0)
        {
            cell.type = CurveCellType.BL;
        }
        // BR - bottom-right
        else if (hilbertPointsInt[x_left, y]   == 0 &&
                 hilbertPointsInt[x_right, y]  == 1 &&
                 hilbertPointsInt[x, y_top]    == 1 &&
                 hilbertPointsInt[x, y_bottom] == 0)
        {
            cell.type = CurveCellType.BR;
        }
    }
 
    return cell;
 }

    void OnDrawGizmos()
    {
        if (generatedMap != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (generatedMap[x, y] == 1) ? Color.white : Color.black;
                    Vector2 pos = new Vector2(x + .5f, y + .5f);
                    Gizmos.DrawCube(pos, Vector2.one);
                }
            }
        }
		if (hilbertPointsInt != null)
    {
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Gizmos.color = (hilbertPointsInt[x, y] == 1) ? Color.cyan : Color.clear;
					Vector2 pos = new Vector2(x + .5f, y + .5f);
					Gizmos.DrawCube(pos, Vector2.one);
				}
			}
    	}
		if (hilbertNegativePointsInt != null){
			for (int x = 0; x < width; x++)	{
				for (int y = 0; y < height; y++)
				{
					Gizmos.color = (hilbertNegativePointsInt[x, y] == 1) ? Color.green : Color.clear;
					Vector2 pos = new Vector2(x + .5f, y + .5f);
					Gizmos.DrawCube(pos, Vector2.one);
				}
			}
		}



		if (hilbertPoints != null){
			for (int i = 0; i < hilbertPoints.Length - 1; i++)
			{
				if (hilbertPoints[i].Equals(Vector2.zero) ||
					hilbertPoints[i + 1].Equals(Vector2.zero))
					continue;
	
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(new Vector2(hilbertPoints[i].x + .5f, hilbertPoints[i].y + .5f),
								new Vector2(hilbertPoints[i + 1].x + .5f, hilbertPoints[i + 1].y + .5f));
			}
    	}
		if (curvePoints != null){
			for (int i = 0; i < curvePoints.Count; i++)
			{
				CurvePoint cPoint = (CurvePoint)curvePoints[i];
				Gizmos.color = (hilbertPointsInt[cPoint.x, cPoint.y] == 1) ? Color.red : Color.clear;
	
				Vector2 pos = new Vector2(cPoint.x + .5f, cPoint.y + .5f);
				Gizmos.DrawCube(pos, Vector2.one * 2);
			}
		}
		// display segments
		if (segments != null)
		{
			foreach (KeyValuePair<Vector2, ArrayList> segment in segments)
			{
				for (int i = 0; i < segment.Value.Count; i++)
				{
					Vector2Int pos = (Vector2Int)segment.Value[i];
					Gizmos.color = Color.yellow;
					Vector2 pos3 = new Vector2(pos.x + .5f, pos.y + .5f);
					Gizmos.DrawCube(pos3, Vector3.one * 2);
	
					if (i == segment.Value.Count - 1)
					{
						Vector2 lastPoint = new Vector2(pos.x + .5f, pos.y + .5f);
						Gizmos.color = Color.red;
						Gizmos.DrawSphere(lastPoint, 2f);
					}
				}
	
				Vector2 posKey = (Vector2)segment.Key;
				Vector2 posKey3 = new Vector2(posKey.x + .5f, posKey.y + .5f);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(posKey3, 2f);
			}
		}
    }
}