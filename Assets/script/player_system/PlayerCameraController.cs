using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] PlayerSettingsSO player_settings_SO;
    [SerializeField] PlayerStatsSO player_stats_SO;
    [SerializeField] InputDataSO input_data_SO;
    [SerializeField] UserSettingsSO user_settings_SO;

    [SerializeField] Transform tilt_pivot;
    [SerializeField] Transform camera_pivot;

    private void Update()
    {
        // handle player camera X-axis pitch rotation
        float pitch = input_data_SO.lookInput.y * user_settings_SO.sensitivity;
        player_stats_SO.tempStats.curPitch -= pitch;
        player_stats_SO.tempStats.curPitch = Mathf.Clamp(player_stats_SO.tempStats.curPitch, -player_settings_SO.clampedPitch, player_settings_SO.clampedPitch);
        camera_pivot.localRotation = Quaternion.Euler(player_stats_SO.tempStats.curPitch, 0, 0);
    }
}
