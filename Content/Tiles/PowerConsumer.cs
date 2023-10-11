using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
	public interface PowerConsumer
    {
        public virtual bool IsConsumer(int i, int j)
        {
            return true;
        }

        public abstract void InsertPower(int i, int j, int amount);
    }
}
