using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimsonQuest.DB.Models;

internal class ConfigQuestGiverModel
{
    public bool IsEnabled { get; set; } = false;
    public float X { get; set; } = 0;
    public float Z { get; set; } = 0;
    public bool Immortal { get; set; } = false;
    public bool CanMove { get; set; } = false;
    public bool AutoRespawn { get; set; } = false;
}