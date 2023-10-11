using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
	public interface IPowerConsumer
    {
        public virtual bool IsConsumer(int i, int j)
        {
            return true;
        }

        public void InsertPower(int i, int j, int amount);
    }
}
