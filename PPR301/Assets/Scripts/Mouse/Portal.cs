// ==========================================================================
// Meowt of Tune - Portal
// Owned and contributed to by: brookcoli (Itch.io) - Aurorafume (GitHub), donkzilla03 (Itch.io) - AliAK03 (GitHub), JaydenFielderTorrens (Itch.io) - Squib35 (GitHub), komorebimoriko (Itch.io), TheRealCrizz (GitHub), LordVGahn (GitHub)
// itch.io listing: https://brookcoli.itch.io/meowt-of-tune - GitHub Repository: https://github.com/Aurorafume/PPR301
// ==========================================================================
//
// WHAT DOES THIS DO:
// This script defines a one-way portal that teleports specific AI characters.
// When a GameObject tagged "Mouse" enters the portal's trigger volume, it is
// instantly transported to a destination point. The destination is determined
// by the portal's assigned type, which maps to an entry in a list of points.
//
// Core functionalities include:
// - Teleporting GameObjects tagged "Mouse" upon entering a trigger.
// - Using an enum (PortalTypes) to define different portal identities.
// - Mapping each portal type to a specific destination Transform in an array.
// - Preserving the mouse's vertical (Y) position during teleportation to
//   prevent it from falling through floors or getting stuck.
//
// Dependencies:
// - Requires a Collider component on the same GameObject with "Is Trigger" enabled.
// - The 'portalPoints' array must be assigned in the Inspector with enough
//   destination points to match the portal types being used.
// - Mouse GameObjects must have a Collider and be tagged as "Mouse".
//
// ==========================================================================

using UnityEngine;

/// <summary>
/// A portal that teleports GameObjects tagged as "Mouse" to a predefined destination.
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Portal Destinations")]
    [Tooltip("An array of destination points. Each portal type corresponds to an index in this array.")]
    public Transform[] portalPoints;
    
    [Header("Portal Configuration")]
    [Tooltip("The type of this portal, which determines its destination from the portalPoints array.")]
    public PortalTypes portalType;

    /// <summary>
    /// Defines the different types of portals, used to select a destination.
    /// </summary>
    public enum PortalTypes
    {
        portal1, // Corresponds to portalPoints[0]
        portal2, // Corresponds to portalPoints[1]
        portal3, // Corresponds to portalPoints[2]
        portal4  // Corresponds to portalPoints[3]
    }
    
    /// <summary>
    /// Handles the teleportation logic when a collider enters the portal's trigger.
    /// </summary>
    /// <param name="collider">The collider that entered the trigger volume.</param>
    void OnTriggerEnter(Collider collider)
    {
        // Only interact with objects tagged as "Mouse".
        if(collider.gameObject.CompareTag("Mouse"))
        {
            // Teleport the mouse based on this portal's specific type.
            switch(portalType)
            {
                case PortalTypes.portal1:
                    // Move the mouse to the destination's X/Z coordinates, but keep its current Y height.
                    collider.gameObject.transform.position = new Vector3(portalPoints[0].position.x, collider.gameObject.transform.position.y, portalPoints[0].position.z);
                    break;

                case PortalTypes.portal2:
                    collider.gameObject.transform.position = new Vector3(portalPoints[1].position.x, collider.gameObject.transform.position.y, portalPoints[1].position.z);
                    break;

                case PortalTypes.portal3:
                    collider.gameObject.transform.position = new Vector3(portalPoints[2].position.x, collider.gameObject.transform.position.y, portalPoints[2].position.z);
                    break;

                case PortalTypes.portal4:
                    collider.gameObject.transform.position = new Vector3(portalPoints[3].position.x, collider.gameObject.transform.position.y, portalPoints[3].position.z);
                    break;
            }
        }
    }
}