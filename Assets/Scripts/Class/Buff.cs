using UnityEngine;

public abstract class Buff
{
        public BuffConfig config;
        public Buff(BuffConfig config)
        {
                this.config = config;
        }

        public virtual void OnApply()
        {
                Debug.Log(config.description + "activated");
                BuffManager.instance.AllBuffs.Remove(config.id);
                BuffManager.instance.ActivatedBuffs.Add(config.id, this);
        }
}