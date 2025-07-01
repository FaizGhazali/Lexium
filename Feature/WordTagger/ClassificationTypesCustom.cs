using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexium.Feature.WordTagger
{
    public static class ClassificationTypesCustom
    {
        public static readonly IClassificationType InvisibleSquiggle =
            new ClassificationType("InvisibleSquiggle", "Invisible Squiggle");
    }
}
