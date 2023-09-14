//三重射击
public class Buff_1 : Buff
{
        public Buff_1(BuffConfig config) : base(config) { }
        public override void OnApply()
        {
                base.OnApply();
                GameManager.instance.attackModel.numOfBulletsPerShot = (int)config.value;
        }
}