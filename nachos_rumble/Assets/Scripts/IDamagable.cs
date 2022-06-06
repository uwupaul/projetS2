using Photon.Realtime;

public interface IDamageable
{
    void TakeDamage(float damage, Player opponent, int gunIndex);
    
    void TakeDamage(float damage, string opponentName, int gunIndex);
}