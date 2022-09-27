using GameDevLib.Args;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Characters
{
    public class Hero : Character
    {
        protected override void Start()
        {
            base.Start();
            Notify();
        }
        
        public override void OnHit(float damage)
        {
            base.OnHit(damage);
            Notify();
        }

        protected override void Notify()
        {
            (float min, float current, float max) hp = (CharacterStats.MinHp, CurrentHp, CharacterStats.MaxHp);
            var args = new UnitArgs(hp, false, CurrentSpeed, false, false, 0);
            UnitEvent.Notify(args);
        }
    }
}