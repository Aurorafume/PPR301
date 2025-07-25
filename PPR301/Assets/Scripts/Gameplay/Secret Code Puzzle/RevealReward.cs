// ==========================================================================
// Meowt of Tune - Reveal Reward
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script handles the animation and activation of a reward object, such as a
// key or a special item, that appears after a puzzle is solved. It moves the
// object from a hidden start point to a visible end point using a smooth,
// curve-based animation.
//
// Core functionalities include:
// - Animating an object along a defined path.
// - Using an AnimationCurve for custom movement easing (e.g. ease-in/out).
// - Keeping the object non-interactable during its animation.
// - Activating the object's physics and collider after the animation completes,
//   making it tangible and ready for player interaction.
//
// Dependencies:
// - UnityEngine for core component and math functionality.
// - A Rigidbody and a Collider component on this GameObject or its children.
//
// ==========================================================================

using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the reveal animation of a reward item after a puzzle is solved.
/// </summary>
public class RevealReward : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("The speed of the reveal animation.")]
    public float moveSpeed;
    [Tooltip("An animation curve to control the easing of the movement.")]
    public AnimationCurve moveCurve;
    [Tooltip("The final position of the object, relative to its starting position.")]
    public Vector3 localEndPosition;

    // The absolute world-space positions for the movement path.
    private Vector3 startPostion;
    private Vector3 endPostion;

    // Cached component references.
    private Rigidbody rb;
    private Collider myCollider;

    /// <summary>
    /// Caches essential component references on awake.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponentInChildren<Collider>();
    }

    /// <summary>
    /// Calculates the movement path and ensures the reward is initially non-interactable.
    /// </summary>
    void Start()
    {
        // Define the absolute start and end points of the animation.
        startPostion = transform.position;
        endPostion = transform.position + localEndPosition;

        // Ensure the object is non-physical and cannot be interacted with at the start.
        SetRewardInteractable(false);
    }

    /// <summary>
    /// Enables or disables the reward's physics and collider to make it interactable.
    /// </summary>
    /// <param name="isActive">True to make the object interactable, false to make it non-physical.</param>
    private void SetRewardInteractable(bool isActive)
    {
        if (isActive)
        {
            rb.isKinematic = false;
            myCollider.enabled = true;
        }
        else
        {
            rb.isKinematic = true;
            myCollider.enabled = false;
        }
    }

    /// <summary>
    /// Public method to begin the reveal animation sequence.
    /// </summary>
    public void Reveal()
    {
        StartCoroutine(MoveCoroutine());
    }

    /// <summary>
    /// Coroutine that moves the object along its path over time using the defined animation curve.
    /// </summary>
    private IEnumerator MoveCoroutine()
    {
        float progress = 0f;

        // Loop until the progress tracker reaches 1 (representing 100% completion).
        while (progress < 1f)
        {
            // Increment the progress based on speed and time.
            progress = Mathf.MoveTowards(progress, 1f, Time.deltaTime * moveSpeed);

            // Evaluate the animation curve at the current progress point to get the eased value.
            float easedProgress = moveCurve.Evaluate(progress);

            // Interpolate the position using the eased progress value for smooth movement.
            transform.position = Vector3.Lerp(startPostion, endPostion, easedProgress);

            yield return null; // Wait for the next frame.
        }

        // Once the animation is complete, make the reward interactable.
        SetRewardInteractable(true);
    }
}