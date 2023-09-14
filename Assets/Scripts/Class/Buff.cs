public abstract class Buff
{
        public BuffConfig config;
        public Buff(BuffConfig config)
        {
                this.config = config;
        }

        public abstract void Apply();
}