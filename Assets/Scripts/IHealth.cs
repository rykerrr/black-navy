public interface IHealth
{
    int Health { get; }
    int MaxHealth { get; }

    void TakeDamage(IDamager damager);
}