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
    public string spriteFolderPath = "Resources/Sprites/Pokemon/"; // e.g. "Pokemon/Sprites/Pikachu/"
    
    [Tooltip("The Pokémon species name (this will be used for file naming conventions)")]
    public string pokemonName = "";
    
    [Header("Animation References")]
    [Tooltip("Automatically populated animation clips")]
    public List<AnimationClip> generatedAnimClips = new List<AnimationClip>();
    
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
        path += $"/{loader.pokemonName}";
        string[] actionFolders = Directory.GetDirectories(path);

        foreach (string name in actionFolders)
        {
            Debug.Log(name);
        }
        
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
        
        Debug.Log($"Looking for actions in: {path}");
        Debug.Log($"Found {actionFolders.Length} action folders");
        
        // Assign controller to the GameObject
        //loader.GetComponent<Animator>().runtimeAnimatorController = controller;

        // Generate transitions between animation clips
        GenerateAnimationTransitions(loader, controller);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Success", "Animations generated successfully!\nBe sure to assign the newly generated controller to the PokemonModel Prefab!", "OK");
    }

    /*
    * <summary> Generates transitions between animation clips. </summary>
    * <param name="loader"> The PokemonAnimationLoader component. </param>
    * <param name="controller"> The AnimatorController to which the animation clips will be added. </param>
    * <remarks> 
    * This method is called after all animation clips have been generated.
    * It creates transitions between Hurt animations and Idle animations, as well as Attack animations and Idle animations.
     </remarks>
    */
    private void GenerateAnimationTransitions(PokemonAnimationLoader loader, AnimatorController controller)
    {
        string[] directions = {"North", "NorthEast", "East", "SouthEast", "South", "SouthWest", "West", "NorthWest"};
        foreach(string direction in directions)
        {
            //Get Hurt, Attack, and Idle animations for the current direction
            string hurtAnimationName = $"{loader.pokemonName}_Hurt_{direction}";
            string attackAnimationName = $"{loader.pokemonName}_Attack_{direction}";
            string idleAnimationName = $"{loader.pokemonName}_Idle_{direction}";

            //Get the animation clips
            AnimationClip hurtAnimation = loader.generatedAnimClips.Find(clip => clip.name == hurtAnimationName);
            AnimationClip attackAnimation = loader.generatedAnimClips.Find(clip => clip.name == attackAnimationName);
            AnimationClip idleAnimation = loader.generatedAnimClips.Find(clip => clip.name == idleAnimationName);

            //Create a transition between Hurt and Idle animations
            AnimatorStateTransition transition = new AnimatorStateTransition(); 
            
            // Need to find the AnimatorState objects rather than using AnimationClips directly
            AnimatorState hurtState = FindState(controller, hurtAnimationName);
            AnimatorState idleState = FindState(controller, idleAnimationName);
            
            if (hurtState != null && idleState != null) {
                // Create transition from hurt to idle
                transition = hurtState.AddTransition(idleState);
                transition.hasExitTime = true;
                transition.exitTime = 0.9f; // Transition near the end of hurt animation
                transition.duration = 0.1f; // Quick transition
            }
            
            // Create transition from attack to idle similarly
            AnimatorState attackState = FindState(controller, attackAnimationName);
            if (attackState != null && idleState != null) {
                transition = attackState.AddTransition(idleState);
                transition.hasExitTime = true;
                transition.exitTime = 0.9f; //0.7540984f
                transition.duration = 0.1f; //0.25f
            }
        }
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

    private AnimatorState FindState(AnimatorController controller, string stateName)
    {
        // Search through all states in the controller to find one with matching name
        return controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name == stateName).state;
    }
}
#endif