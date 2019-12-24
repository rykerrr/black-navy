public interface IHealth
{
    int Health { get; }
    int MaxHealth { get; }

    void ChangeHealth(int newHealth);   
}