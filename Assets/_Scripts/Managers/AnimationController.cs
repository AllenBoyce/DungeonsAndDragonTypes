using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject _unit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PokemonAnimationLoader animationLoader = GameObject.Find("Garchomp").GetComponent<PokemonAnimationLoader>();
            animationLoader.PlayAnimation("Walk", "North");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PokemonAnimationLoader animationLoader = GameObject.Find("Garchomp").GetComponent<PokemonAnimationLoader>();
            animationLoader.PlayAnimation("Walk", "South");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PokemonAnimationLoader animationLoader = GameObject.Find("Garchomp").GetComponent<PokemonAnimationLoader>();
            animationLoader.PlayAnimation("Walk", "West");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PokemonAnimationLoader animationLoader = GameObject.Find("Garchomp").GetComponent<PokemonAnimationLoader>();
            animationLoader.PlayAnimation("Walk", "East");
        }*/
    }
}
