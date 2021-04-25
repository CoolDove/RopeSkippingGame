using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dove.Core;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Rope : Actor
{
    [Header("CurveParameters")]
    [SerializeField]
    [Range(-1, 0)]
    private float a;
    [SerializeField]
    private float range = 5;
    [SerializeField]
    [Range(0,10)]
    private float offsetY;
    [SerializeField]
    private float height;
    [SerializeField]
    private float platform;
    [SerializeField][Range(0,1)]
    private float defaultProgress;

    [Header("RopeParameters")]
    [SerializeField]
    private float lineWidth = 0.5f;
    [SerializeField]
    [Range(0.25f, 6f)]
    private float interp = 0.5f;

    [Header("Difficulty")]
    [SerializeField]
    private float weight = 0.1f;

    [SerializeField]
    private float progress = 0;
    [HideInInspector]
    private float targetSpeed;
    private float speed;
    private float crossZ;


    //这一帧检查点的位置
    private Vector3 checkPoint = new Vector3();
    //记录上一帧检查点的位置，用来与本帧的检查点形成线段作检测
    private Vector3 oldCheckPoint;
    private Vector3[] points;
    private LineRenderer line;
    //抛物线的另一个参数
    private float c;


    public bool Recovered { get; private set; }

    #region Mono
    void Awake()
    {
        Init();
        EventManager.Instance.SubscribeEvent("GameDie", ResetRope);
        EventManager.Instance.SubscribeEvent("GameStart", () => { targetSpeed = 1; Recovered = false; });
        EventManager.Instance.SubscribeEvent("BackToMenu", ResetRope);
        EventManager.Instance.SubscribeEvent("StartPrepare", ResetRope);
    }

    protected override void PUpdate()
    {
        Move();

        if (Physics.Linecast(oldCheckPoint, checkPoint))
        {
            //绳子绊到玩家
            //todo
            EventManager.Instance.SendEvent("RopeHitPlayer");
        }
        Debug.DrawLine(oldCheckPoint, checkPoint, Color.red);
        GlobalVar.ropeProgress = progress;
    }

    protected override void PFixedUpdate()
    {
    }

    private void LateUpdate()
    {
        oldCheckPoint = checkPoint;
    }

    private void OnValidate()
    {
        //Init();
    }
    #endregion


    public void ResetRope()
    {
        speed = 0;
        targetSpeed = 0;
        progress = defaultProgress;

        GetCurve();
        Draw();
        line.SetPositions(points);
        GlobalVar.ropeProgress = progress;
    }

    private void Move()
    {
        if (progress < 0.55f && (progress + Time.deltaTime * speed) >= 0.55f)
        {
            if (GlobalVar.diePreparing) 
            {
                speed = targetSpeed = 0;
                ResetRope();
                Recovered = true;
            }
            else
            {
                EventManager.Instance.SendEvent("RopeOneRound");
                targetSpeed = Difficulty.GetSpeed(GlobalVar.difficulty);
            }
        }
        if (progress + Time.deltaTime * speed > 1)
        {
            float add = progress + Time.deltaTime * speed - 1;
            progress = add;
        }
        else
        {
            progress += Time.deltaTime * speed;
        }
        speed = Mathf.Lerp(speed, targetSpeed, weight);

        GetCurve();
        Draw();
        line.SetPositions(points);
    }

    private float GetY(float x)
    {
        float y = a * x * x + c;
        y = y * Mathf.Cos(progress * 2 * Mathf.PI) + offsetY;
        y = y < platform ? platform : y;
        return y;
    }
    private float GetZ(float x)
    {
        float y = a * x * x + c;
        float rawY = y * Mathf.Cos(progress * 2 * Mathf.PI) + offsetY;
        float rawZ = y * Mathf.Sin(progress * 2 * Mathf.PI);
        if (rawY <= platform&& Mathf.Sin(progress * 2 * Mathf.PI) != 0)
        {
            float s = (rawZ - crossZ) / Mathf.Sin(progress * 2 * Mathf.PI);
            float z = crossZ + s;
            return z;
        }
        return rawZ;
    }
    private void GetCurve()
    {
        int count = 0;

        c= height - a * range * range;
        float k = 1 / Mathf.Tan(progress * 2 * Mathf.PI);
        crossZ = (platform - offsetY) / k;

        for (float i = -range; i < range; i += interp)
        {
            float x = i;
            float y = GetY(x);
            float z = GetZ(x);
            points[count] = new Vector3(x, y, z);
            count++;
        }
        count++;
        float ex = range;
        float ey = GetY(ex);
        float ez = GetZ(ex);
        points[points.Length - 1] = new Vector3(ex, ey, ez);

        //计算抛物线顶点坐标以检测
        checkPoint.x = 0;
        checkPoint.y = GetY(checkPoint.x);
        checkPoint.z = GetZ(checkPoint.x);

        line.positionCount = count;
        line.endWidth = line.startWidth = lineWidth;
    }
    private void Draw()
    {
        Debug.DrawLine(new Vector3(-range, offsetY, 0), new Vector3(range, offsetY, 0));
        Debug.DrawLine(new Vector3(-range, platform, crossZ), new Vector3(range, platform, crossZ));

        if (points != null && points.Length > 0)
        {
            for (int i = 0; i < points.Length - 1; i++) 
            {
                Debug.DrawLine(points[i], points[i + 1], Color.red);
            }
        }
    }
    private void Init()
    {
        line = GetComponent<LineRenderer>();
        progress = defaultProgress;
        int count = 0;
        for (float i = -range; i < range; i += interp)
        {
            count++;
        }
        count++;
        points = new Vector3[count];
        line.positionCount = count;
    }
}

public class Difficulty
{
    private static Difficulty _instance;
    public static Difficulty Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = new Difficulty();
            }
            return _instance;
        }
    }

    private static float DefaultLowSpd { get { return 0.8f; } }
    private static float DefaultHighSpd { get { return 1.0f; } }

    public static uint MaxDifficulty { get { return (uint)Instance.sheet.Count; } }

    public Dictionary<uint, Vector2> sheet = new Dictionary<uint, Vector2>();

    private Difficulty()
    {
        sheet.Add(0, new Vector2(1f,1.2f));
        sheet.Add(1, new Vector2(0.8f,1.4f));
        sheet.Add(2, new Vector2(0.7f,1.5f));
        sheet.Add(3, new Vector2(0.6f,1.8f));
        sheet.Add(4, new Vector2(0.6f,1.9f));
        sheet.Add(5, new Vector2(0.5f, 2.0f));
    }


    public static float GetLowSpeed(uint difficulty)
    {
        float d = DefaultLowSpd;
        if (Instance.sheet.ContainsKey(difficulty))
        {
            d = Instance.sheet[difficulty].x;
        }
        return d;
    }
    public static float GetHighSpeed(uint difficulty)
    {
        float d = DefaultLowSpd;
        if (Instance.sheet.ContainsKey(difficulty))
        {
            d = Instance.sheet[difficulty].y;
        }
        return d;
    }
    public static float GetSpeed(uint difficulty)
    {
        return Random.Range(GetLowSpeed(difficulty), GetHighSpeed(difficulty));
    }
}