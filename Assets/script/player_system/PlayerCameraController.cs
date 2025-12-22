using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] PlayerSettingsSO player_settings_SO;
    [SerializeField] PlayerStatsSO player_stats_SO;
    [SerializeField] InputDataSO input_data_SO;
    [SerializeField] UserSettingsSO user_settings_SO;

    [SerializeField] Transform pivot_camera_base_link;      // root
    [SerializeField] Transform pivot_camera_tilt_link;      // branch
    [SerializeField] Transform pivot_camera_end_effector;   // leaf

    [SerializeField] float tilt_scalar = 0.5f;
    [SerializeField] float tilt_acceleration = 2f;
    private float tilt_current = 0f;

    private void Update()
    {
        // handle player camera X-axis pitch rotation
        float pitch = input_data_SO.lookInput.y * user_settings_SO.sensitivity;
        player_stats_SO.tempStats.curPitch -= pitch;
        player_stats_SO.tempStats.curPitch = Mathf.Clamp(player_stats_SO.tempStats.curPitch, -player_settings_SO.clampedPitch, player_settings_SO.clampedPitch);
        pivot_camera_end_effector.localRotation = Quaternion.Euler(player_stats_SO.tempStats.curPitch, 0, 0);

        // Handle player movement tilt.
        // Absolute Dot product between player velocity and camera pivot left direction.
        {
            // NOTE(Jazz): get player maxspeed for inverse lerp
            //float tilt_target = Mathf.InverseLerp(player_settings_SO.runSpeed, , player_stats_SO.tempStats.moveVelocity.magnitude);
            //if(Mathf.Abs(tilt_target) < 0.01f) tilt_target = 0f;

            //Debug.Log("tilt_dot: " + tilt_target);
            //tilt_current = Mathf.Lerp(tilt_current, tilt_target, tilt_acceleration * Time.deltaTime);
            //pivot_camera_tilt_link.localRotation = Quaternion.Euler(0, 0, tilt_current * tilt_scalar);
        }
    }
}
