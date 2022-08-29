// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    public class Hero : Character
    {
        public override void OnHit(float damage)
        {
            CurrentHp -= damage;
            // TODO: Notify me! 
        }
    }
}