using UnityEngine;

public static class ClampedPosition{
    /// <summary>
    /// This function will clamp a position inside the boundaries provided and return the new position
    /// </summary>
    /// <param name="targetPosition">The position to be clamped</param>
    /// <param name="boundsPosition">The center position of the boundaries</param>
    /// <param name="boundsSize">Half the size of the boundaries</param>
    /// <param name="offsetX">Offset appended onto the boundaries e.g. Camera size</param>
    /// <param name="offsetY">Offset appended onto the boundaries e.g. Camera size</param>
    /// <param name="offsetZ">Offset appended onto the boundaries e.g. Camera size</param>
    /// <returns>A position clamped inside the boundaries</returns>
    public static Vector3 GetClampedPos(Vector3 targetPosition, Vector3 boundsPosition, Vector3 boundsSize, float offsetX = 0, float offsetY = 0, float offsetZ = 0){
        Vector3 clampedPos = new Vector3(
            Mathf.Clamp(targetPosition.x, (boundsPosition.x - boundsSize.x) + offsetX, (boundsPosition.x + boundsSize.x) - offsetX),
            Mathf.Clamp(targetPosition.y, (boundsPosition.y - boundsSize.y) + offsetY, (boundsPosition.y + boundsSize.y) - offsetY),
            Mathf.Clamp(targetPosition.z, (boundsPosition.z - boundsSize.z) + offsetZ, (boundsPosition.z + boundsSize.z) - offsetZ)
        );
        return clampedPos;
    }
}
