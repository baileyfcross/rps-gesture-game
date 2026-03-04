/// <summary>
/// MonoBehaviour representing a single finger in the 2D hand gesture system.
/// Handles key activation, mouse-drag positioning, and snap-zone detection.
/// </summary>
using System;
using UnityEngine;

namespace RPSGame.Hand
{
    using RPSGame.Core;

    /// <summary>
    /// Attached to each finger GameObject (2D sprite).  The player presses the
    /// assigned <see cref="activationKey"/> to "pick up" the finger, then drags
    /// with the mouse to move it toward one of two snap zones
    /// (<see cref="extendedTarget"/> or <see cref="curledTarget"/>).
    /// Once within <see cref="snapDistance"/>, the finger locks into position and
    /// raises <see cref="OnFingerPlaced"/>.
    /// </summary>
    public class FingerController : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>Which finger this controller represents.</summary>
        [Tooltip("The finger type this controller represents.")]
        [SerializeField] private FingerType fingerType;

        /// <summary>Keyboard key that activates this finger for dragging.</summary>
        [Tooltip("Press this key to begin dragging the finger.")]
        [SerializeField] private KeyCode activationKey;

        /// <summary>World-space snap zone for the extended finger position.</summary>
        [Tooltip("Transform marking the snap zone for the Extended finger position (2D world space).")]
        [SerializeField] private Transform extendedTarget;

        /// <summary>World-space snap zone for the curled finger position.</summary>
        [Tooltip("Transform marking the snap zone for the Curled finger position (2D world space).")]
        [SerializeField] private Transform curledTarget;

        /// <summary>
        /// Maximum distance (in world units) from a snap zone for automatic snapping to occur.
        /// </summary>
        [Tooltip("Distance threshold (world units) within which the finger snaps to a zone.")]
        [SerializeField] private float snapDistance = 0.5f;

        #endregion

        #region Private Fields

        private bool   _isActive;
        private bool   _isLocked;
        private Camera _mainCamera;

        #endregion

        #region Properties

        /// <summary>Gets the <see cref="FingerType"/> this controller represents.</summary>
        public FingerType FingerType => fingerType;

        /// <summary>Gets the current position state of this finger.</summary>
        public FingerPosition CurrentPosition { get; private set; } = FingerPosition.Curled;

        /// <summary>Gets a value indicating whether the finger is currently being dragged.</summary>
        public bool IsActive => _isActive;

        /// <summary>Gets a value indicating whether the finger has been locked into a snap zone.</summary>
        public bool IsLocked => _isLocked;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the finger snaps into a position.
        /// Parameters: the <see cref="FingerType"/> and the resulting <see cref="FingerPosition"/>.
        /// </summary>
        public event Action<FingerType, FingerPosition> OnFingerPlaced;

        #endregion

        #region Unity Lifecycle

        /// <summary>Caches the main camera reference to avoid per-frame lookups.</summary>
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Each frame: checks for activation key press, handles drag movement,
        /// and tests for snap-zone proximity.
        /// </summary>
        private void Update()
        {
            if (_isLocked)
                return;

            // Activate on key press
            if (Input.GetKeyDown(activationKey))
                _isActive = true;

            // Deactivate on key release
            if (Input.GetKeyUp(activationKey))
                _isActive = false;

            if (_isActive)
            {
                // Convert mouse screen position to 2D world position (z = 0)
                Vector3 mouseScreenPos = Input.mousePosition;
                mouseScreenPos.z = -_mainCamera.transform.position.z;
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
                worldPos.z = 0f;

                transform.position = worldPos;

                // Check proximity to snap zones
                TrySnap(worldPos);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets this finger to its default (unlocked, inactive, curled) state and
        /// moves it back to the curled target position if one is assigned.
        /// </summary>
        public void ResetFinger()
        {
            _isActive  = false;
            _isLocked  = false;
            CurrentPosition = FingerPosition.Curled;

            if (curledTarget != null)
            {
                Vector3 pos = curledTarget.position;
                pos.z = 0f;
                transform.position = pos;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tests whether <paramref name="worldPos"/> is within <see cref="snapDistance"/>
        /// of either snap zone.  If so, locks the finger into that zone and fires
        /// <see cref="OnFingerPlaced"/>.
        /// </summary>
        /// <param name="worldPos">The current world-space position of the finger.</param>
        private void TrySnap(Vector3 worldPos)
        {
            if (extendedTarget != null)
            {
                float distToExtended = Vector2.Distance(worldPos, extendedTarget.position);
                if (distToExtended <= snapDistance)
                {
                    SnapTo(extendedTarget.position, FingerPosition.Extended);
                    return;
                }
            }

            if (curledTarget != null)
            {
                float distToCurled = Vector2.Distance(worldPos, curledTarget.position);
                if (distToCurled <= snapDistance)
                {
                    SnapTo(curledTarget.position, FingerPosition.Curled);
                }
            }
        }

        /// <summary>
        /// Locks the finger to <paramref name="snapPos"/>, records the
        /// <paramref name="position"/>, and raises <see cref="OnFingerPlaced"/>.
        /// </summary>
        /// <param name="snapPos">World-space position to snap to.</param>
        /// <param name="position">The resulting finger position enum value.</param>
        private void SnapTo(Vector3 snapPos, FingerPosition position)
        {
            snapPos.z           = 0f;
            transform.position  = snapPos;
            CurrentPosition     = position;
            _isActive           = false;
            _isLocked           = true;

            OnFingerPlaced?.Invoke(fingerType, position);
        }

        #endregion
    }
}
