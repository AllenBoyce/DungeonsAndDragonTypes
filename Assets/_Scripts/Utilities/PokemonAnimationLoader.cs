using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEditor.Animations;
using System.Xml;

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

    [Tooltip("The number of game frames per second")]
    public float gameFramesPerSecond = 60f;
    
}

// Class to store animation data from AnimData.xml
public class AnimationFrameData
{
    public string Name;
    public List<float> Durations = new List<float>();

    public AnimationFrameData(string name)
    {
        Name = name;
    }
}

#if UNITY_EDITOR
// Editor script for generating animations
[CustomEditor(typeof(PokemonAnimationLoader))]
public class PokemonAnimationLoaderEditor : Editor
{
    // Dictionary to store animation data by name
    private Dictionary<string, AnimationFrameData> animationDataByName = new Dictionary<string, AnimationFrameData>();

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
        
        // Clear the generated animation clips list to prevent referencing destroyed objects
        loader.generatedAnimClips.Clear();
        
        string path = loader.spriteFolderPath;
        if (!path.StartsWith("Assets/"))
            path = "Assets/" + path;
            
        if (!Directory.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", $"Folder not found: {path}", "OK");
            return;
        }

        // Try to parse AnimData.xml before proceeding with animation generation
        string xmlFilePath = Path.Combine(path, loader.pokemonName, "AnimData.xml");
        if (File.Exists(xmlFilePath))
        {
            try
            {
                ParseAnimDataXml(xmlFilePath, loader);
                Debug.Log($"Successfully parsed AnimData.xml for {loader.pokemonName}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error parsing AnimData.xml: {e.Message}. Will use default frame durations.");
                animationDataByName.Clear(); // Ensure we don't use partial data
            }
        }
        else
        {
            Debug.LogWarning($"AnimData.xml not found at {xmlFilePath}. Will use default frame durations.");
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

        // Clear animation data after generation is complete
        animationDataByName.Clear();
    }

    /*
    * <summary> Parse the AnimData.xml file to extract animation frame durations. </summary>
    * <param name="xmlFilePath"> The path to the AnimData.xml file. </param>
    * <param name="loader"> The PokemonAnimationLoader component. </param>
    */
    private void ParseAnimDataXml(string xmlFilePath, PokemonAnimationLoader loader)
    {
        // Clear any existing animation data
        animationDataByName.Clear();
        
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);
        
        // Get all Anim nodes
        XmlNodeList animNodes = xmlDoc.SelectNodes("//Anim");
        
        if (animNodes != null)
        {
            foreach (XmlNode animNode in animNodes)
            {
                // Get animation name
                XmlNode nameNode = animNode.SelectSingleNode("Name");
                if (nameNode == null) continue;
                
                string animName = nameNode.InnerText;
                AnimationFrameData animData = new AnimationFrameData(animName);
                
                // Check if this animation copies another animation's data
                XmlNode copyOfNode = animNode.SelectSingleNode("CopyOf");
                if (copyOfNode != null)
                {
                    string copyOfName = copyOfNode.InnerText;
                    // We'll handle copied animations later once all original animations are parsed
                    animData.Durations = null; // Mark as a copied animation
                    animationDataByName[animName] = animData;
                    continue;
                }
                
                // Get duration nodes
                XmlNode durationsNode = animNode.SelectSingleNode("Durations");
                if (durationsNode == null) continue;
                
                XmlNodeList durationNodes = durationsNode.SelectNodes("Duration");
                if (durationNodes == null) continue;
                
                // Parse durations
                foreach (XmlNode durationNode in durationNodes)
                {
                    if (int.TryParse(durationNode.InnerText, out int frameDuration))
                    {
                        // Convert from game frames to seconds
                        float durationInSeconds = frameDuration / loader.gameFramesPerSecond;
                        animData.Durations.Add(durationInSeconds);
                    }
                }
                
                animationDataByName[animName] = animData;
            }
            
            // Handle animations that copy from other animations
            foreach (var animData in animationDataByName.Values.ToList())
            {
                if (animData.Durations == null)
                {
                    // Find the animation node again to get the CopyOf value
                    XmlNode animNode = xmlDoc.SelectSingleNode($"//Anim[Name='{animData.Name}']");
                    if (animNode == null) continue;
                    
                    XmlNode copyOfNode = animNode.SelectSingleNode("CopyOf");
                    if (copyOfNode == null) continue;
                    
                    string copyOfName = copyOfNode.InnerText;
                    if (animationDataByName.TryGetValue(copyOfName, out AnimationFrameData sourceAnimData) && 
                        sourceAnimData.Durations != null)
                    {
                        // Copy durations from the source animation
                        animData.Durations = new List<float>(sourceAnimData.Durations);
                    }
                }
            }
        }
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
            //Get state names for the current direction
            string hurtAnimationName = $"{loader.pokemonName}_Hurt_{direction}";
            string attackAnimationName = $"{loader.pokemonName}_Attack_{direction}";
            string idleAnimationName = $"{loader.pokemonName}_Idle_{direction}";

            // Find states directly from the controller instead of using clip references
            AnimatorState hurtState = FindState(controller, hurtAnimationName);
            AnimatorState idleState = FindState(controller, idleAnimationName);
            AnimatorState attackState = FindState(controller, attackAnimationName);
            
            // Create transition from hurt to idle
            if (hurtState != null && idleState != null) {
                var transition = hurtState.AddTransition(idleState);
                transition.hasExitTime = true;
                transition.exitTime = 0.9f; // Transition near the end of hurt animation
                transition.duration = 0.1f; // Quick transition
            }
            
            // Create transition from attack to idle
            if (attackState != null && idleState != null) {
                var transition = attackState.AddTransition(idleState);
                transition.hasExitTime = true;
                transition.exitTime = 0.9f;
                transition.duration = 0.1f;
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
        
        // Try to get animation data for this action
        bool useCustomDurations = animationDataByName.TryGetValue(actionName, out AnimationFrameData animData) && 
                                 animData.Durations != null && 
                                 animData.Durations.Count > 0;
        
        // Add sprite keyframes
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[files.Length];
        float currentTime = 0f;
        
        for (int i = 0; i < files.Length; i++)
        {
            string assetPath = files[i].Replace(Application.dataPath, "Assets");
            
            // Set import settings for the sprite
            SetSpriteImportSettings(assetPath);
            
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            
            if (sprite != null)
            {
                ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe();
                
                // Use custom duration if available, otherwise use default 0.1s
                float frameDuration = useCustomDurations && i < animData.Durations.Count 
                    ? animData.Durations[i] 
                    : 0.1f; // Default 10 FPS
                
                keyframe.time = currentTime;
                keyframe.value = sprite;
                keyframes[i] = keyframe;
                
                // Add this frame's duration to the current time for the next frame
                currentTime += frameDuration;
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
        
        // Add to generated clips list
        loader.generatedAnimClips.Add(clip);
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