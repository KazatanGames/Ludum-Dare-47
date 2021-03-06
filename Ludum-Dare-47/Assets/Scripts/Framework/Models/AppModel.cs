﻿using UnityEngine;

/**
 * App Model
 * 
 * Kazatan Games Framework - requires customization per game.
 * 
 * Data and models which should be accessible throughout the application.
 */
public class AppModel
{
    public AudioPreferencesPersistance audioPreferences;
    // Add persistances and models here

    public bool debugMode = false;

    public float playerY;
    public Vector3 playerVelocity;

    public AppModel()
    {
        // create instances of persistances and models here
        audioPreferences = new AudioPreferencesPersistance();
    }
}