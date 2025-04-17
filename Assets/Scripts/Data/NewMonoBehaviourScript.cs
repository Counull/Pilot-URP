using UnityEngine;


[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "ScriptableObjects/PlayerAttributes", order = 1)]
public class PlayerAttributes : ScriptableObject {
    public float maxHealth;
    public float baseMoveSpeed;
    public float baseJumpForce;
    public float dashSpeed;
    public float dashDuration;
}