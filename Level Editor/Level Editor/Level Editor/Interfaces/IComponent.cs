using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Leda.Core.Game_Objects.Behaviours;

namespace Level_Editor.Interfaces
{
    public interface IComponent : IWorldObject, ISimpleRenderable, ICameraRelative
    {
        bool Selected { get; set; }
        bool ReturningToPreviousLocation { get; set; }

        XElement Save();

        bool Contains(Vector2 worldPosition);
    }
}
