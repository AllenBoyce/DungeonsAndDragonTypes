using UnityEngine;
using System;
using System.Collections.Generic;

public static class Constants
{
    public const float TILE_LAYER = 0f;
    public const float CAMERA_LAYER = -10f;
    public const float UNIT_LAYER = -4f;
    public const float TILE_HIGHLIGHT_LAYER = -1f;

    public const int TEAM_SIZE = 2;
    public const int PLAYER_COUNT = 2;

    public enum PokemonSpecies {
        Garchomp,
        Flapple,
        Flygon
    }

    public static List<Color> PLAYER_COLORS {
        get {
            return new List<Color> {
                new Color(1f, 165/255f, 0f),
                new Color(186/255f, 85/255f, 211/255f)
            };
        }
    }

    public const bool DebugMode = true;
}