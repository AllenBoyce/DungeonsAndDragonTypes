using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEditor.Animations;

public class PokemonAnimationLoader : MonoBehaviour
{
    
    [Header("Animation Settings")]
    [Tooltip("Path to the folder containing animation sprite sheets (relative to Assets/Resources/)")]
    public string spriteFolderPath = "Pokemon/Sprites/Pokemon/"; // e.g. "Pokemon/Sprites/Pikachu/"
    
    [Tooltip("The Pokémon species name (this will be used for file naming conventions)")]
    public string pokemonName = "";
    
    [Header("Animation References")]
    [Tooltip("Automatically populated animation clips")]
    public List<AnimationClip> generatedAnimClips = new List<AnimationClip>();
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null)
        {
            Debug.LogError($"No Animator component found on Pokémon {pokemonName}!");
            return;
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"No SpriteRenderer component found on Pokémon {pokemonName}!");
            return;
        }
    }
    
    public void PlayAnimation(string animName, string direction)
    {
        if (animator != null)
        {
            animator.Play($"{pokemonName}_{animName}_{direction}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


#if UNITY_EDITOR
// Editor script for generating animations
[CustomEditor(typeof(PokemonAnimationLoader))]
public class PokemonAnimationLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        PokemonAnimationLoader loader = (PokemonAnimationLoader)target;
        
        if (GUILayout.Button("Generate Animations"))
        {
            GenerateAnimations(loader);
        }
    }
    
    private void GenerateAnimations(PokemonAnimationLoader loader)
    {
        if (string.IsNullOrEmpty(loader.pokemonName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a Pokémon name", "OK");
            return;
        }
        
        string path = loader.spriteFolderPath;
        if (!path.StartsWith("Assets/"))
            path = "Assets/" + path;
            
        if (!Directory.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", $"Folder not found: {path}", "OK");
            return;
        }
        
        // Make sure Pokemon-specific animation folder exists
        string pokemonFolder = $"Assets/Animations/Pokemon/{loader.pokemonName}";
        if (!Directory.Exists(pokemonFolder))
            Directory.CreateDirectory(pokemonFolder);
            
        // Create animator controller
        string controllerPath = $"{pokemonFolder}/{loader.pokemonName}_Controller.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        
        // Process action folders (Walk, Idle, Attack, etc.)
        string[] actionFolders = Directory.GetDirectories(path);
        
        foreach (string actionFolder in actionFolders)
        {
            string actionName = Path.GetFileName(actionFolder);
            
            // Process direction folders within each action folder
            string[] directionFolders = Directory.GetDirectories(actionFolder);
            
            foreach (string directionFolder in directionFolders)
            {
                string directionName = Path.GetFileName(directionFolder);
                CreateAnimationClip(directionFolder, actionName, directionName, loader, controller);
            }
        }
        
        // Assign controller to the GameObject
        loader.GetComponent<Animator>().runtimeAnimatorController = controller;
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Success", "Animations generated successfully!", "OK");
    }
    
    private void CreateAnimationClip(string folder, string actionName, string direction, PokemonAnimationLoader loader, AnimatorController controller)
    {
        string[] files = Directory.GetFiles(folder, "*.png").OrderBy(f => f).ToArray();
        if (files.Length == 0) return;
        
        // Create animation clip
        AnimationClip clip = new AnimationClip();
        clip.name = $"{loader.pokemonName}_{actionName}_{direction}";
            
        // Set loop time
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        
        // Create sprite curve
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        
        // Add sprite keyframes
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string assetPath = files[i].Replace(Application.dataPath, "Assets");
            
            // Set import settings for the sprite
            SetSpriteImportSettings(assetPath);
            
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            
            if (sprite != null)
            {
                ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe();
                keyframe.time = i * 0.1f; // 10 FPS
                keyframe.value = sprite;
                keyframes[i] = keyframe;
            }
        }
        
        // Apply keyframes
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyframes);
        
        // Save clip
        string pokemonFolder = $"Assets/Animations/Pokemon/{loader.pokemonName}";
        string clipPath = $"{pokemonFolder}/{clip.name}.anim";
        AssetDatabase.CreateAsset(clip, clipPath);
        
        // Add state to controller
        AnimatorState state = controller.layers[0].stateMachine.AddState(clip.name);
        state.motion = clip;
        
        // If this is the first state, set as default
        if (controller.layers[0].stateMachine.defaultState == null)
        {
            controller.layers[0].stateMachine.defaultState = state;
        }
    }
    
    private void SetSpriteImportSettings(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            // Set sprite import settings
            importer.spritePixelsPerUnit = 24;
            importer.filterMode = FilterMode.Point; // 'None' in the UI is 'Point' in code
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spriteImportMode = SpriteImportMode.Single;
            
            // Set texture settings
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteExtrude = 0;
            settings.spriteGenerateFallbackPhysicsShape = false;
            settings.alphaSource = TextureImporterAlphaSource.FromInput;
            settings.alphaIsTransparency = true;
            settings.wrapMode = TextureWrapMode.Clamp;
            importer.SetTextureSettings(settings);
            
            // Apply the changes
            importer.SaveAndReimport();
        }
    }
}
#endif