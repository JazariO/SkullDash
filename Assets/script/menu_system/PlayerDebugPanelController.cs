using TMPro;
using UnityEngine;

public class PlayerDebugPanelController : MonoBehaviour
{
    [SerializeField] PlayerSettingsSO player_settings_SO;
    [SerializeField] PlayerStatsSO player_stats_SO;
    [SerializeField] InputDataSO input_data_SO;

    [SerializeField] TMP_Text coyote_time_elapsed_TMP;
    [SerializeField] TMP_Text input_move_vector_TMP;
    [SerializeField] TMP_Text input_look_vector_TMP;
    [SerializeField] TMP_Text move_velocity_TMP;
    [SerializeField] TMP_Text move_direction_TMP;
    [SerializeField] TMP_Text current_target_speed_TMP;
    [SerializeField] TMP_Text is_grounded_TMP;
    [SerializeField] TMP_Text wish_jump_TMP;
    [SerializeField] TMP_Text last_jump_time_TMP;
    [SerializeField] TMP_Text look_dir_TMP;
    [SerializeField] TMP_Text slope_TMP;
    [SerializeField] TMP_Text current_state_TMP;

    private void Update()
    {
        coyote_time_elapsed_TMP.text    = $"coyote time | {player_stats_SO.tempStats.coyoteTimeElapsed:F2}";
        input_move_vector_TMP.text      = $"in | {input_data_SO.moveInput}";
        input_look_vector_TMP.text      = $"look | {input_data_SO.lookInput}";
        move_velocity_TMP.text          = $"move vel | ({player_stats_SO.tempStats.moveVelocity.x:F2}, " +
                                                     $"{player_stats_SO.tempStats.moveVelocity.y:F2}, " +
                                                     $"{player_stats_SO.tempStats.moveVelocity.z:F2})";
        move_direction_TMP.text         = $"move dir | ({player_stats_SO.tempStats.moveDirection.eulerAngles.x:F2}, " +
                                                     $"{player_stats_SO.tempStats.moveDirection.eulerAngles.y:F2}, " +
                                                     $"{player_stats_SO.tempStats.moveDirection.eulerAngles.z:F2})";
        current_target_speed_TMP.text   = $"target speed | {player_stats_SO.tempStats.curTargetSpeed:F2}";
        is_grounded_TMP.text            = $"grounded | {player_stats_SO.tempStats.isGrounded}";
        wish_jump_TMP.text              = $"wish jump | {player_stats_SO.tempStats.willJump}";
        last_jump_time_TMP.text         = $"last jump time | {player_stats_SO.tempStats.lastJumpTime:F2}";
        look_dir_TMP.text               = $"look dir | Pitch:{player_stats_SO.tempStats.curPitch:F2}, Yaw:{player_stats_SO.tempStats.curYaw:F2}";
        slope_TMP.text                  = $"slope | {player_stats_SO.tempStats.slope:F2}";
        current_state_TMP.text          = $"state | {player_stats_SO.tempStats.curState}";

    }
}
