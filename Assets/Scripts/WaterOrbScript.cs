using UnityEngine;

// this script will create a floating water orb pickup 
public class WaterOrb : MonoBehaviour
{
    public int waterAmount = 1;

    public float orbSize = 0.5f;
    public Color orbColor = new Color(0.2f, 0.6f, 1f, 0.3f); // transparent blue

    // controls how much the orb bobs up and down
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;
    public float rotateSpeed = 60f;

    public Color particleColor = new Color(0.4f, 0.8f, 1f, 0.8f);

    Vector3 _startPos;

    void Awake()
    {
        // create a sphere for the orb visual
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * orbSize;

        // remove the collider from the sphere so it doesn't interfere with our trigger
        Destroy(visual.GetComponent<Collider>());

        // apply a transparent blue material
        Renderer rend = visual.GetComponent<Renderer>();
        rend.material = MakeTransparentMaterial(orbColor);

        // add a trigger so we can detect when the player walks through it
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = orbSize * 1.1f;
        col.isTrigger = true;

        SetupParticles();

        _startPos = transform.position;
    }

    void Update()
    {
        // bob up and down using a sine wave
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // slowly spin
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // check if the thing that touched us has an EvolutionManager
        EvolutionManager evo = other.GetComponent<EvolutionManager>();
        if (evo == null) return;

        evo.AddWater(waterAmount);
        Destroy(gameObject);
    }

    Material MakeTransparentMaterial(Color color)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        if (mat != null)
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 0f);
            mat.SetFloat("_ZWrite", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        else
        {
            // fallback if not using URP
            mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 3f);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }

        mat.color = color;
        return mat;
    }

    void SetupParticles()
{
    GameObject psGO = new GameObject("Particles");
    psGO.transform.SetParent(transform);
    psGO.transform.localPosition = Vector3.zero;

    ParticleSystem ps = psGO.AddComponent<ParticleSystem>();

    var main = ps.main;
    main.loop = true;
    main.startLifetime = 1.2f;
    main.startSpeed = 0.3f;
    main.startSize = 0.05f;
    main.startColor = new Color(0.4f, 0.8f, 1f, 0.8f);
    main.gravityModifier = -0.15f;
    main.simulationSpace = ParticleSystemSimulationSpace.World;
    main.maxParticles = 40;

    var emission = ps.emission;
    emission.rateOverTime = 12f;

    var shape = ps.shape;
    shape.shapeType = ParticleSystemShapeType.Sphere;
    shape.radius = orbSize * 0.5f;
    shape.radiusThickness = 0f;

    // size shrinks over lifetime
    var sizeOL = ps.sizeOverLifetime;
    sizeOL.enabled = true;
    AnimationCurve sizeCurve = new AnimationCurve();
    sizeCurve.AddKey(0f, 1f);
    sizeCurve.AddKey(1f, 0f);
    sizeOL.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

    // fade out over lifetime
    var colOL = ps.colorOverLifetime;
    colOL.enabled = true;
    Gradient grad = new Gradient();
    grad.SetKeys(
        new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
    );
    colOL.color = new ParticleSystem.MinMaxGradient(grad);

    var psRenderer = psGO.GetComponent<ParticleSystemRenderer>();
    Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
    if (particleShader == null) particleShader = Shader.Find("Particles/Standard Unlit");
    if (particleShader != null)
    {
        psRenderer.material = new Material(particleShader);
        psRenderer.material.color = particleColor;
    }
}
}
