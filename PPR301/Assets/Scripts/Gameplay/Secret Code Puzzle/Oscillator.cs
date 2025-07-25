// ==========================================================================
// Meowt of Tune - Oscillator
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This is a utility script that moves a GameObject back and forth between two
// points in a smooth, oscillating motion. It uses a sine wave to create a
// natural-looking repetitive movement, making it ideal for moving platforms,
// environmental hazards, or other dynamic elements.
//
// Core functionalities include:
// - Defining a movement path between two relative offset positions.
// - Customising the speed (period) of the oscillation.
// - Setting a starting offset within the movement cycle.
//
// Dependencies:
// - UnityEngine for core component and math functionality.
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// Moves the attached GameObject back and forth between two points using a sine wave.
/// </summary>
public class Oscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [SerializeField, Tooltip("The starting point of the oscillation, relative to the object's initial position.")]
    private Vector3 offsetPosition1;
    [SerializeField, Tooltip("The ending point of the oscillation, relative to the object's initial position.")]
    private Vector3 offsetPosition2;
    [SerializeField, Tooltip("An offset to the starting position within the oscillation cycle (in radians).")]
    private float startingOffset;
    [SerializeField, Tooltip("The time in seconds to complete one full back-and-forth cycle. Smaller is faster.")]
    private float period = 2f;

    // The absolute world-space positions for the oscillation path.
    private Vector3 position1;
    private Vector3 position2;

    /// <summary>
    /// Called by Unity on startup to cache the absolute start and end positions.
    /// </summary>
    void Start()
    {
        // Calculate the absolute positions based on the object's starting local position and the defined offsets.
        position1 = transform.localPosition + offsetPosition1;
        position2 = transform.localPosition + offsetPosition2;
    }

    /// <summary>
    /// Called every frame by Unity to calculate and apply the oscillating movement.
    /// </summary>
    void Update()
    {
        // Prevent division by zero if period is not set.
        if (period <= Mathf.Epsilon) return;

        // Calculate how far along the cycle we are.
        float cycles = Time.time / period;
        
        // Define a full circle in radians (tau = 2 * PI).
        const float tau = Mathf.PI * 2f;
        // Calculate the raw sine wave value (-1 to 1).
        float rawSinWave = Mathf.Sin(cycles * tau + startingOffset);

        // Remap the sine wave value from [-1, 1] to [0, 1] to use as a movement factor.
        float movementFactor = (rawSinWave + 1f) / 2f;

        // Linearly interpolate between the two positions using the movement factor.
        transform.localPosition = Vector3.Lerp(position1, position2, movementFactor);
    }
}