using Bloody.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace CrimsonQuest.Structs;

public struct QuestArea
{
    public float3 Center { get; set; }
    public float Radius { get; set; }

    public QuestArea(float3 _center, float _radius)
    {
        Center = _center;
        Radius = _radius;
    }

    public static QuestArea FromPos(float3 pos, float radius)
    {
        return new QuestArea(pos, radius);
    }

    public readonly bool InQuestArea(float3 pos)
    {
        float _dx = pos.x - Center.x;
        float _dz = pos.z - Center.z;
        return (_dx * _dx + _dz * _dz) <= (Radius * Radius);
    }

    public readonly bool InQuestArea(Entity entity)
    {
        return InQuestArea(entity.Read<LocalToWorld>().Position);
    }
}
