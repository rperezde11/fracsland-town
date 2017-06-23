using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Creates a fraction with the shape of a pie
/// </summary>
public class PieFraction : MonoBehaviour {

    SpriteRenderer _renderer;
    private int _full = 5; //Nuber of regions wich will be full
    private int _divisions = 6; //Max regions
    public const int MAX_DIVISIONS = 17; // Maximum number of Divisions (human eye in an IPAD has limitations)
    public Color linesColor, fullColor, emptyColor; //Colors to fill
    public const int BASE_TEXTURE_DIM = 512; //Our working texture dimensions will be 512 x 512 and then will be resized.

    /// <summary>
    /// Creates our Pie texture by calling a bunch of methods
    /// </summary>
    public void GenerateTexture()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _full = DataManager.instance.ActualQuest.solution.down - DataManager.instance.ActualQuest.solution.top;
        _divisions = DataManager.instance.ActualQuest.solution.down;

        //Prevent some errors and make sure that the programmer's know that he is doing something wrong with tne numbers
        if (_divisions > MAX_DIVISIONS)
        {
            Debug.LogError("PieFraction only can handle numbers less than " + MAX_DIVISIONS + " and you have entered " + _divisions + "!");
            _divisions = MAX_DIVISIONS;
        }

        if(_full > _divisions)
        {
            Debug.LogError("PieFraction can't fill more parts than divisions!");
            _full = _divisions;
        }

        if(_full < 0 || _divisions < 1)
        {
            Debug.LogError("Divisions must be greather than 1 and full must be more than 0");
            _divisions = 1;
            _full = 0;
        }

        int desiredWidth = _renderer.sprite.texture.width;
        int desiredHeigth = _renderer.sprite.texture.height;

        Texture2D texture = new Texture2D(BASE_TEXTURE_DIM, BASE_TEXTURE_DIM, TextureFormat.ARGB32, false);

        DrawCircularTextureWithDivisions(texture, _divisions, linesColor);

        TextureScale.Bilinear(texture, desiredWidth, desiredHeigth);

        RemoveNonCirclePixels(texture, desiredWidth / 2);
        
        Sprite sp = Sprite.Create(texture, _renderer.sprite.rect, new Vector2(0.5f, 0.5f), _renderer.sprite.pixelsPerUnit);
        _renderer.sprite = null;
        _renderer.sprite = sp;
    }

    /// <summary>
    /// Main method to draw the structure
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="numDivisions"></param>
    /// <param name="divisionsColor"></param>
    void DrawCircularTextureWithDivisions(Texture2D texture, int numDivisions, Color divisionsColor)
    {
        //Calc the radius of the circle
        int radius = texture.width / 2;

        //Get the tecture center
        Vector2 textureCenter = new Vector2(texture.width / 2, texture.height / 2);

        if (_full == _divisions)
        {
            emptyColor = fullColor;
        }

        //Draw the Circle
        DrawCircle(texture, radius);
        
        if(_full == 0 || _full == _divisions)
        {
            return;
        }



        //Container to get the lines center pixel + offset (this will be used to colorize the divisions)
        List<Vector2> linesCentralPoints = new List<Vector2>();
        
        // The portion of the angle that every section will have
        float angleDivision = (360 / numDivisions);
        for (int i = 0; i < numDivisions; i++)
        {
            //Get the exterior point of the radius with an angle
            Vector2 endpoint = GetRectWithPointAndAngle(textureCenter, angleDivision * i, radius);
            
            //Get the vector to draw
            var lineToDraw = SupercoverLine(textureCenter, endpoint);
            linesCentralPoints.Add(lineToDraw[Mathf.FloorToInt(lineToDraw.Count / 2) + 5]);

            //Draw pixels
            drawPixels(lineToDraw, divisionsColor, texture);
        }

        ColorizeRegions(texture, linesCentralPoints);

        //Clean exterior lines
        for (int i = 0; i < texture.width; i++)
        {
            texture.SetPixel(i, texture.height - 1, new Color(0, 0, 0, 0));
            texture.SetPixel(i, texture.height - 2, new Color(0, 0, 0, 0));
            texture.SetPixel(i, texture.height - 3, new Color(0, 0, 0, 0));
            texture.SetPixel(i, texture.height - 4, new Color(0, 0, 0, 0));
            texture.SetPixel(i, texture.height - 5, new Color(0, 0, 0, 0));
            texture.SetPixel(i, texture.height - 6, new Color(0, 0, 0, 0));
        }

    }

    
    /// <summary>
    /// COlorize the regions of our Pie
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="exteriorLinesPoints"></param>
    void ColorizeRegions(Texture2D texture, List<Vector2> exteriorLinesPoints)
    {
        List<Vector2> diffusePoints = new List<Vector2>();
        if (_divisions > 2)
        {
            //Calc the difuse starting points
            for (int i = 0; i < exteriorLinesPoints.Count - 1; i++)
            {
                diffusePoints.Add(GetCentralPointBetween2Points(exteriorLinesPoints[i], exteriorLinesPoints[i + 1]));
            }

            //Get the last diffuse point
            diffusePoints.Add(GetCentralPointBetween2Points(exteriorLinesPoints[exteriorLinesPoints.Count - 1], diffusePoints[0]));
        }
        else
        {
            diffusePoints.Add(GetRectWithPointAndAngle(exteriorLinesPoints[0], 90, 20));
            diffusePoints.Add(GetRectWithPointAndAngle(exteriorLinesPoints[0], 270, 20));

        }


        //Used to draw the diffuse points (only for debug purposes, enabling this line means that no diffuse will be performed)
        //drawPixels(diffusePoints, fullColor, texture);

        //Diffuse the colors
        int filledZonesCounter = 0;
        do
        {
            DiffuseColor(diffusePoints[filledZonesCounter], texture, fullColor);
            filledZonesCounter++;
        } while (filledZonesCounter < _full);
    }

    /// <summary>
    /// Given a point diffuses it's color to all the neigbourds that has a particular color
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="texture"></param>
    /// <param name="c"></param>
    void DiffuseColor(Vector2 origin, Texture2D texture, Color c)
    {
        Dictionary<float, Dictionary<float, bool>> visitedNodes = new Dictionary<float, Dictionary<float, bool>>();
        Dictionary<float, Dictionary<float, bool>> nodesToVisit = new Dictionary<float, Dictionary<float, bool>>();
        AddVector(origin, nodesToVisit);

        List<Vector2> nodesToPaint = new List<Vector2>();
        Vector2 up, right, down, left;//directions to diffuse
        while (nodesToVisit.Count > 0)
        {
            Vector2 px = PopVector(nodesToVisit);
            AddVector(px, visitedNodes);
            Color pixelColor = texture.GetPixel((int)px.x, (int)px.y);
            var empty = emptyColor;
            if( Mathf.Approximately(pixelColor.r, emptyColor.r) &&
                Mathf.Approximately(pixelColor.g, emptyColor.g) &&
                Mathf.Approximately(pixelColor.b, emptyColor.b) &&
                Mathf.Approximately(pixelColor.a, emptyColor.a) )
            {
                nodesToPaint.Add(px);
                up = px + Vector2.up;
                if (!ContainsVector(up, visitedNodes) && !ContainsVector(up, nodesToVisit))
                    AddVector(up, nodesToVisit);
                right = px + Vector2.right;
                if (!ContainsVector(right, visitedNodes) && !ContainsVector(right, nodesToVisit))
                    AddVector(right, nodesToVisit);
                down = px + Vector2.down;
                if (!ContainsVector(down, visitedNodes) && !ContainsVector(down, nodesToVisit))
                    AddVector(down, nodesToVisit);
                left = px + Vector2.left;
                if (!ContainsVector(left, visitedNodes) && !ContainsVector(left, nodesToVisit))
                    AddVector(left, nodesToVisit);
            }
        }

        drawPixels(nodesToPaint, fullColor, texture);
    }

    float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    /// <summary>
    /// Check if a vector it's in our dictionary inside dictionary structure
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    bool ContainsVector(Vector2 vec, Dictionary<float, Dictionary<float, bool>> container)
    {
        if (container.ContainsKey(vec.x))
        {
            return container[vec.x].ContainsKey(vec.y);
        }
        return false;
    }

    /// <summary>
    /// Adds a vector to our structure
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="container"></param>
    void AddVector(Vector2 vec, Dictionary<float, Dictionary<float, bool>> container)
    {
        if (container.ContainsKey(vec.x))
        {
            container[vec.x].Add(vec.y, true);
        }
        else
        {
            Dictionary<float, bool> yComponentDic = new Dictionary<float, bool>();
            yComponentDic.Add(vec.y, true);
            container.Add(vec.x, yComponentDic);
        }
    }

    /// <summary>
    /// USed to Pop a Vector to our dictionary inside dictionary structure
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    Vector2 PopVector(Dictionary<float, Dictionary<float, bool>> container)
    {
        float x = container.Keys.First();
        Dictionary<float, bool> yDic = container[x];
        float y = yDic.Keys.First();
        yDic.Remove(y);
        if(yDic.Count() == 0)
        {
            container.Remove(x);
        }

        return new Vector2(x, y);
    }

    /// <summary>
    /// "Draws" a line into a matrix by using "Bresenham-based supercover line algorithm" and returns you the middle point
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Vector2 GetCentralPointBetween2Points(Vector2 origin, Vector2 end)
    {
        var line = SupercoverLine(origin, end);
        return line[Mathf.FloorToInt(line.Count / 2)];
    }


    /// <summary>
    /// Given a point, angle and radius/distance tells you what pixel of the matrix are you pointing
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="angle"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    Vector2 GetRectWithPointAndAngle(Vector2 origin, float angle, int radius)
    {
        angle = angle * Mathf.Deg2Rad;
        Vector2 end = new Vector2();
        end.x = origin.x + radius * Mathf.Cos(angle);
        end.y = origin.y + radius * Mathf.Sin(angle);
        return end;
    }

    /// <summary>
    /// Draws a circle
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="radius"></param>
    void DrawCircle(Texture2D texture, int radius)
    {
        var voidColor = emptyColor;
        var transparentColor = new Color(0, 0, 0, 0);

        for (int i = 0; i < radius * 2; i++)
            for (int j = 0; j < radius * 2; j++)
            {
                int d = (int)Mathf.Sqrt((i - radius) * (i - radius) + (j - radius) * (j - radius));
                if (d < radius) texture.SetPixel(i, j, voidColor);
                else texture.SetPixel(i, j, transparentColor);
            }
        texture.Apply();
        //Unity assigns an aproximation of our empy color to the texture, so we need to extract the color that 
        //has been assigned as our empty color
        emptyColor = texture.GetPixel(Mathf.RoundToInt(texture.width / 4), Mathf.RoundToInt(texture.width / 4));
    }
    
    /// <summary>
    /// Used to clean all the noisy pixels out of our circle
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="radius"></param>
    void RemoveNonCirclePixels(Texture2D texture, int radius)
    {
        for (int i = 0; i < radius * 2; i++)
            for (int j = 0; j < radius * 2; j++)
            {
                int d = (int)Mathf.Sqrt((i - radius) * (i - radius) + (j - radius) * (j - radius));
                if (d > radius) texture.SetPixel(i, j, new Color(0, 0, 0, 0));
            } 
        texture.Apply();
    }

    /// <summary>
    /// Draws an array of pixels into a texture
    /// </summary>
    /// <param name="pixelsPositions"></param>
    /// <param name="color"></param>
    /// <param name="texture"></param>
    void drawPixels(List<Vector2> pixelsPositions, Color color, Texture2D texture)
    {
        foreach(Vector2 vec in pixelsPositions)
        {
            texture.SetPixel((int)vec.x, (int)vec.y, color);
        }
        texture.Apply();
    }

    /// <summary>
    /// Bresenham-based supercover line algorithm
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    List<Vector2> SupercoverLine(Vector2 p0, Vector2 p1)
    {
        var dx = p1.x - p0.x;
        var dy = p1.y - p0.y;
        var nx = Mathf.Abs(dx);
        var ny = Mathf.Abs(dy);
        var sign_x = dx > 0 ? 1 : -1;
        var sign_y = dy > 0 ? 1 : -1;

        var p = new Vector2(p0.x, p0.y);
        var points = new List<Vector2>();
        points.Add(new Vector2(p.x, p.y));
        var ix = 0;
        var iy = 0;
        for (ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            if ((0.5 + ix) / nx == (0.5 + iy) / ny)
            {
                p.x += sign_x;
                p.y += sign_y;
                ix++;
                iy++;
            }
            else if ((0.5 + ix) / nx < (0.5 + iy) / ny)
            {
                p.x += sign_x;
                ix++;
            }
            else
            {
                p.y += sign_y;
                iy++;
            }
            points.Add(new Vector2(p.x, p.y));
        }
        return points;
    }
}