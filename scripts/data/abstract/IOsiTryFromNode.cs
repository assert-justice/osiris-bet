using System;
using Prion.Node;

namespace Osiris.Data;

public interface IOsiTryFromNode<T>
{
    public abstract static bool TryFromNode(PrionNode prionNode, out T data);
}
